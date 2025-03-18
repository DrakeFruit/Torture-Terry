using System;
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
		foreach ( var i in TortureTerry.Inventory.ItemsAccessor.Where( i => i.IsValid() ) )
		{
			Unlocks.TryAdd( i.Name, !i.Locked );
		}
		
		FileSystem.Data.WriteJson( Game.SteamId + " - 1.0", this );
	}

	public PlayerData Load()
	{
		var data = FileSystem.Data.ReadJson<PlayerData>( Game.SteamId + " - 1.0" );
		Unlocks = data.Unlocks;
		
		if ( Unlocks != null )
		{
			foreach ( var i in Unlocks.Where( i => i.Value ) )
			{
				try { Achievements.Unlock( "unlock_" + i.Key ); }
				catch ( Exception e ) { Log.Info("no achievement for unlock_" + i.Key); Log.Info( e ); }
			}
		}
		
		if ( data.Unlocks == null || data.Unlocks.Count == 0 || data.Score == 0 ) return new PlayerData();
		return data;
	}
}
