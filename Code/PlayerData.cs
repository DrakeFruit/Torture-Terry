using System.Collections.Generic;
using System.Text.Json;
using Sandbox.Services;

namespace Sandbox;

public class PlayerData 
{
	public int Score { get; set; } = 0;

	public static bool BombUnlocked { get; set; } = false;
	
	public void Save()
	{
		Stats.SetValue( "score", Score );
		
		FileSystem.Data.WriteJson( Game.SteamId.ToString(), this );
	}

	public static PlayerData Load()
	{
		var data = FileSystem.Data.ReadJson<PlayerData>( Game.SteamId.ToString() );
		return data ?? new PlayerData(); //new if null
	}
}
