using Sandbox;

namespace TortureTerry;
public class GameManager : Component
{
	public static PlayerData Player = new();
	private RealTimeSince LastSave = 0f;
	protected override void OnStart()
	{
		Player = PlayerData.Load();
	}
	
	protected override void OnUpdate()
	{
		if ( LastSave > 1 )
		{
			Player.Save();
			LastSave = 0;
		}
	}
}
