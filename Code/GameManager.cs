namespace Sandbox;

public class GameManager : Component
{
	private PlayerData player = new();
	private RealTimeSince LastSave = 0f;
	protected override void OnStart()
	{
		player = PlayerData.Load();
	}
	
	protected override void OnUpdate()
	{
		if ( LastSave > 1f )
		{
			player.Save();
		}
	}
}
