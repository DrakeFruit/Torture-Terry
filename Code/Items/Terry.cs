using System;
using System.Linq;
using Sandbox;
using Sandbox.Services;

namespace TortureTerry;
public class Terry : Component, Component.ICollisionListener, Component.IDamageable
{
	[RequireComponent] private ModelPhysics Physics { get; set; }
	[Property] public static GameObject BloodPrefab { get; set; }
	private TimeSince TimeSinceLastDamage { get; set; }
	protected override void OnStart()
	{
		ModelPhysics physics = this.Components.Get<ModelPhysics>();
		GameManager.Terries.Add( this );
	}

	public void OnCollisionStart( Collision collision )
	{
		if ( collision.Other.Body.PhysicsGroup == Physics.PhysicsGroup ) return;
		
		float minImpactSpeed = 1000;
		float speed = collision.Contact.Speed.Length;
		float impactDamage = Physics.PhysicsGroup.Mass / 10;
		if ( impactDamage <= 0.0f ) impactDamage = 10;

		if ( speed >= minImpactSpeed && TimeSinceLastDamage >= 0.1f )
		{
			var damage = (int)( (speed / minImpactSpeed * impactDamage) / 15 ) / GameManager.Terries.Count;
			if ( damage < 1 ) damage = 1;
			
			Stats.Increment( "score", damage );
			GameManager.Player.Score += damage;
			
			GameManager.Destroy( Bleed( collision.Self.Body.Position ), 5 );
			
			TimeSinceLastDamage = 0;
		}
	}

	public static GameObject Bleed( Vector3 position )
	{
		var blood = Terry.BloodPrefab.Clone( position.WithX( -25 ) );
		var r = new Random();
		blood.LocalRotation = blood.LocalRotation.Angles().WithRoll( r.Int( 0, 360 ) );

		return blood;
	}

	protected override void OnDestroy()
	{
		GameManager.Terries.Remove( this );
	}

	public void OnDamage( in DamageInfo damage )
	{
		if ( damage.Damage < 1 ) damage.Damage = 1;
			
		Stats.Increment( "score", damage.Damage );
		GameManager.Player.Score += damage.Damage.CeilToInt() / GameManager.Terries.Count;
			
		GameManager.Destroy( Bleed( damage.Position ), 5 );
			
		TimeSinceLastDamage = 0;
	}
}
