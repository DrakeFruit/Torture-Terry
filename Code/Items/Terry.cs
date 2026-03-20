using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.Services;

namespace TortureTerry;

[Serializable]
public class LimbGib
{
	[Property, KeyProperty] public string BoneName { get; set; }
	[Property, KeyProperty] public GameObject Prefab { get; set; }
	[Property, KeyProperty] public int MaxHealth { get; set; }
	[Property, KeyProperty] public bool Vital { get; set; }
}

public class Terry : Component, Component.ICollisionListener, Component.IDamageable
{
	[RequireComponent] ModelPhysics ModelPhysics { get; set; }
	[Property] public GameObject BloodPrefab { get; set; }
	[Property] public List<LimbGib> LimbGibs { get; set; } = new();
	Dictionary<string, int> LimbHealth { get; set; } = new();
	Dictionary<int, string> BodyToBone { get; set; } = new();
	public bool IsAlive { get; private set; } = true;
	List<GameObject> SpawnedGibs { get; set; } = new();

	[Button("Explode Head")]
	public void DebugExplodeHead() => DamageLimb( "head", 100 );

	[Button("Explode Left Arm")]
	public void DebugExplodeLeftArm() => DamageLimb( "clavicle_L", 100 );

	[Button("Damage Random Limb")]
	public void DebugDamageRandomLimb()
	{
		if ( LimbGibs.Count == 0 ) return;
		var limb = LimbGibs[Random.Shared.Int( 0, LimbGibs.Count - 1 )];
		DamageLimb( limb.BoneName, 10 );
	}

	TimeSince TimeSinceLastDamage { get; set; }

	public static Terry ActiveTerry => Game.ActiveScene.GetComponent<Terry>();
	protected override void OnStart()
	{
		GameManager.Terries.Add( this );

		foreach ( var limb in LimbGibs )
		{
			LimbHealth[limb.BoneName] = limb.MaxHealth;
		}

		foreach ( var body in ModelPhysics.Bodies )
		{
			BodyToBone[body.Component.PhysicsBody.GetHashCode()] = body.Bone.ToString();
		}
	}

	public int GetLimbHealth( string boneName )
	{
		return LimbHealth.TryGetValue( boneName, out var health ) ? health : 0;
	}

	public bool IsLimbDestroyed( string boneName )
	{
		return !LimbHealth.ContainsKey( boneName ) || LimbHealth[boneName] <= 0;
	}

	string GetBoneFromBody( PhysicsBody body )
	{
		if ( body != null && BodyToBone.TryGetValue( body.GetHashCode(), out var boneName ) )
			return boneName;
		return null;
	}

	public void OnCollisionStart( Collision collision )
	{
		var hitBone = GetBoneFromBody( collision.Other.Body );
		
		float minImpactSpeed = 1000;
		float speed = collision.Contact.Speed.Length;
		float impactDamage = ModelPhysics.Mass / 10;
		if ( impactDamage <= 0.0f ) impactDamage = 10;

		if ( speed >= minImpactSpeed && TimeSinceLastDamage >= 0.1f )
		{
			var damage = (int)( (speed / minImpactSpeed * impactDamage) / 15 ) / GameManager.Terries.Count;
			if ( damage < 1 ) damage = 1;
			
			if ( hitBone != null )
			{
				DamageLimb( hitBone, damage );
			}
			
			Stats.Increment( "score", damage );
			GameManager.Player.Score += damage;
			
			GameManager.Destroy( Bleed( collision.Self.Body.Position ), 5 );
			
			TimeSinceLastDamage = 0;
		}
	}

	public GameObject Bleed( Vector3 position )
	{
		if ( BloodPrefab is null ) return null;
		var blood = BloodPrefab.Clone( position.WithX( -25 ) );
		if ( !blood.IsValid() ) return null;
		var r = new Random();
		blood.LocalRotation = blood.LocalRotation.Angles().WithRoll( r.Int( 0, 360 ) );

		return blood;
	}

	void SpawnGib( string boneName, Vector3 position, Rotation rotation )
	{
		var limb = LimbGibs.FirstOrDefault( l => l.BoneName == boneName );
		if ( limb?.Prefab is null ) return;

		var gib = limb.Prefab.Clone( position, rotation );
		if ( !gib.IsValid() ) return;

		SpawnedGibs.Add( gib );

		var body = ModelPhysics.Bodies.FirstOrDefault( b => b.Bone.ToString() == boneName );
		var velocity = body.Component?.Velocity ?? Vector3.Zero;
		var rb = gib.Components.Get<Rigidbody>();
		if ( rb.IsValid() )
		{
			rb.ApplyImpulse( velocity * 500 );
			rb.ApplyTorque( new Vector3( Random.Shared.Float( -1, 1 ), Random.Shared.Float( -1, 1 ), Random.Shared.Float( -1, 1 ) ) * 100 );
		}
	}

	public void DamageLimb( string boneName, int damage )
	{
		if ( !IsAlive ) return;
		if ( !LimbHealth.ContainsKey( boneName ) )
		{
			Log.Info( $"Bone {boneName} not found in LimbHealth" );
			return;
		}
		if ( LimbHealth[boneName] <= 0 ) return;

		LimbHealth[boneName] -= damage;
		Log.Info( $"Damaged {boneName}: {LimbHealth[boneName]} HP remaining" );

		if ( LimbHealth[boneName] <= 0 )
		{
			var limb = LimbGibs.FirstOrDefault( l => l.BoneName == boneName );
			
			var boneIdx = ModelPhysics.Model.Bones.GetBone( boneName );
			if ( ModelPhysics.Renderer.TryGetBoneTransform( boneIdx, out var transform ) )
			{
				SpawnGib( boneName, WorldPosition, WorldRotation );
			}

			ScaleBoneToZero( boneIdx );
			DetachBonePhysics( boneName );

			if ( limb?.Vital == true )
			{
				Kill();
			}
		}
	}

	public void Kill()
	{
		if ( !IsAlive ) return;
		IsAlive = false;

		foreach ( var gib in SpawnedGibs )
		{
			GameManager.Destroy( gib, 5f );
		}

		GameManager.Destroy( GameObject, 5f );
	}

	void ScaleBoneToZero( BoneCollection.Bone bone )
	{
		var boneObj = ModelPhysics.Renderer.GetBoneObject( bone );
		if ( boneObj.IsValid() )
		{
			boneObj.LocalScale = Vector3.Zero;
		}
	}

	void DetachBonePhysics( string boneName )
	{
		var body = ModelPhysics.Bodies.FirstOrDefault( b => b.Bone.ToString() == boneName );
		if ( body.Component == null ) return;

		var bodyGo = body.Component.GameObject;
		if ( !bodyGo.IsValid() ) return;

		var joint = ModelPhysics.Joints.FirstOrDefault( j => j.Body2.Bone.ToString() == boneName );
		if ( joint.Component != null )
		{
			joint.Component.Destroy();
		}

		bodyGo.SetParent( null );
	}

	protected override void OnDestroy()
	{
		GameManager.Terries.Remove( this );
	}

	public void OnDamage( in DamageInfo damage )
	{
		if ( damage.Damage < 1 ) damage.Damage = 1;
			
		Stats.Increment( "score", damage.Damage );
		GameManager.Player.Score += damage.Damage.CeilToInt() / ( damage.Tags.Has( "Explosion" ) ? GameManager.Terries.Count : 1 );
			
		GameManager.Destroy( Bleed( damage.Position ), 5 );
			
		TimeSinceLastDamage = 0;
	}
}
