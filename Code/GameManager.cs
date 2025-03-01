using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Services;

namespace TortureTerry;
public class GameManager : Component
{
	public static PlayerData Player { get; set; } = new();
	public static List<Terry> Terries { get; set; } = [];
	private RealTimeSince LastSave = 0f;
	private RealTimeSince TimePlayed = 0f;
	protected override void OnStart()
	{
		Player = PlayerData.Load();
		if ( Player.Unlocks == null ) Player.Unlocks = new();
		
		Player.Save();
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
