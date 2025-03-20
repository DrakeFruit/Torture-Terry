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

	public Dictionary<string, bool> Unlocks { get; set; } = new();
	
	public void Save()
	{
		if ( Unlocks == null )
		{
			Log.Error( "Couldn't save data, please report to @DrakeFruit on Discord" );
			return;
		}
		
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
		
		if ( Unlocks != null )
		{
			foreach ( var i in Unlocks.Where( i => i.Value ) )
			{
				try { Achievements.Unlock( "unlock_" + i.Key ); }
				catch ( Exception e ) { Log.Warning("no achievement for unlock_" + i.Key); Log.Warning( e ); }
			}
		}

		if ( data == null ) return new PlayerData();
		if ( data.Unlocks == null || data.Unlocks.Count == 0 || data.Score == 0 ) return new PlayerData();
		return data;
	}
}
