using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.Services;

namespace TortureTerry;
public class GameManager : Component
{
	public static PlayerData Player { get; set; } = new();
	public static List<Terry> Terries { get; set; } = [];
	RealTimeSince LastSave = 0f;
	RealTimeSince TimePlayed = 0f;
	protected override void OnStart()
	{
		try
		{
			Player = new PlayerData();
			Player = Player.Load();
		
			Player.Unlocks ??= new Dictionary<string, bool>();
		
			Player.Save();
		}
		catch ( Exception e )
		{
			Log.Error( e );
			Log.Error( "Report this error to @DrakeFruit on Discord" );
		}
	}

	protected override void OnFixedUpdate()
	{
		foreach ( var rb in Scene.GetAllComponents<Rigidbody>() )
		{
			rb.Locking = new PhysicsLock { X = true, Yaw = true, Pitch = true };
		}

		foreach ( var mp in Scene.GetAllComponents<ModelPhysics>() )
		{
			foreach ( var body in mp.Bodies )
			{
				body.Component.Locking = new PhysicsLock { X = true, Yaw = true, Pitch = true };
			}
		}
	}
	
	protected override void OnUpdate()
	{
		if ( LastSave.Relative > 1 )
		{
			Player.Save();
			LastSave = 0;
		}

		if ( TimePlayed.Relative >= 60 )
		{
			Stats.Increment( "minutes_played", 1 );
			TimePlayed = 0;
		}

		if ( Scene.GetAllComponents<Terry>().Count() == 0 )
		{
			var terryDef = ResourceLibrary.GetAll<ItemDefinition>().FirstOrDefault( i => i.Name == "terry" );
			if ( terryDef?.Prefab != null )
			{
				terryDef.Prefab.Clone( Vector3.Zero.WithZ( 196 ) );
			}
		}
	}
	
	public async static void Destroy( GameObject obj, float time )
	{
		if ( obj is null ) return;
		await GameTask.DelayRealtimeSeconds( time );
		if ( obj.IsValid() ) obj.Destroy();
	}
}
