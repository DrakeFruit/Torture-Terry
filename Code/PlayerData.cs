using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Sandbox.Services;

namespace Sandbox;

public class PlayerData 
{
	public int Score { get; set; } = 0;

	public Dictionary<string, bool> Unlocks { get; set; }
	
	public void Save()
	{
		Stats.SetValue( "score", Score );
		
		foreach ( var i in TortureTerry.Inventory.ItemsAccessor )
		{
			Unlocks.TryAdd( i.Name, !i.Locked );
		}
		
		FileSystem.Data.WriteJson( Game.SteamId.ToString(), this );
	}

	public static PlayerData Load()
	{
		var data = FileSystem.Data.ReadJson<PlayerData>( Game.SteamId.ToString() );
		return data ?? new PlayerData(); //new if null
	}
}
