using Sandbox;
using Sandbox.Services;

namespace TortureTerry;
public class GameManager : Component
{
	public static PlayerData Player = new();
	private RealTimeSince LastSave = 0f;
	private RealTimeSince TimePlayed = 0f;
	protected override void OnStart()
	{
		Player = PlayerData.Load();
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
}
