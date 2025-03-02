using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Sandbox.Services;
using TortureTerry;
using Inventory = Sandbox.Services.Inventory;

namespace Sandbox;

public class PlayerData 
{
	public int Score { get; set; } = 0;

	public Dictionary<string, bool> Unlocks { get; set; }
	
	public void Save()
	{
		Stats.SetValue( "score", Score );
		if( TortureTerry.Inventory.ItemsAccessor == null ) return;
		foreach ( var i in TortureTerry.Inventory.ItemsAccessor )
		{
			if( !i.IsValid() ) break;
			Unlocks.TryAdd( i.Name, !i.Locked );
		}
		
		FileSystem.Data.WriteJson( Game.SteamId.ToString(), this );
	}

	public static PlayerData Load()
	{
		var data = FileSystem.Data.ReadJson<PlayerData>( Game.SteamId.ToString() );
		
		if ( data != null )
		{
			foreach ( var i in data.Unlocks )
			{
				if ( i.Value ) continue;
				Achievements.Unlock( "unlock_" + i.Key );
			}
		}
		
		return data ?? new PlayerData(); //new if null
	}
}
