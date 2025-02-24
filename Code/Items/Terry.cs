using System.Linq;
using Sandbox;
using Sandbox.Services;

namespace TortureTerry;
public class Terry : Component, Component.IDamageable, Component.ICollisionListener
{
	[RequireComponent] private ModelPhysics Physics { get; set; }
	public Inventory Inventory;
	protected override void OnStart()
	{
		ModelPhysics physics = this.Components.Get<ModelPhysics>();
		Leaderboard.Terries.Add( this );
	}

	public void OnCollisionStart( Collision collision )
	{
		if ( collision.Other.Body.PhysicsGroup == Physics.PhysicsGroup ) return;
		
		float minImpactSpeed = 1000;
		float speed = collision.Contact.Speed.Length;
		float impactDamage = Physics.PhysicsGroup.Mass / 10;
		if ( impactDamage <= 0.0f ) impactDamage = 10;

		if ( speed >= minImpactSpeed )
		{
			var damage = (int)( (speed / minImpactSpeed * impactDamage) / 20 ) / Leaderboard.Terries.Count();
			if ( damage < 1 ) damage = 1;
			Stats.Increment( "score", damage );
			PlayerData.Score += damage;
		}
	}
	
	public void OnDamage( in DamageInfo damage )
	{
		
	}
}
