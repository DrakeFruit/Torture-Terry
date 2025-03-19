using System;
using System.Collections.Generic;
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
	}
	
	public async static void Destroy( GameObject obj, float time )
	{
		await GameTask.DelayRealtimeSeconds( time );
		obj.Destroy();
	}
}
