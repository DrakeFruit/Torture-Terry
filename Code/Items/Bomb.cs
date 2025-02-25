using System.Threading.Tasks;
using Sandbox;
using Sandbox.Audio;
using Sandbox.Services;
using TortureTerry;
using GameManager = TortureTerry.GameManager;

public sealed class Bomb : Component
{
	[Property] private RangedFloat FuseTime { get; set; }
	[Property] private GameObject ExplosionPrefab { get; set; }
	[Property] SoundEvent ExplosionSound { get; set; }
	[Property] float Volume { get; set; }
	[Property] private float Radius { get; set; } = 150;
	[Property] private float Power { get; set; } = 5000;
	private TimeSince Timer { get; set; }
	
	protected override void OnStart()
	{
		Timer = 0;
		Stats.Increment( "bombs", 1 );
	}
	protected override void OnFixedUpdate()
	{
		if ( Timer.Relative >= FuseTime.GetValue() )
		{
			foreach ( var i in Scene.FindInPhysics( new Sphere( WorldPosition, Radius ) ) )
			{
				var impulse = (i.WorldPosition - WorldPosition).Normal * Power;
				var rb = i.Components.Get<Rigidbody>();
				var mp = i.Components.Get<ModelPhysics>();
				if ( i == GameObject ) continue;
				if ( rb != null ) rb.ApplyImpulse( impulse );
				if ( mp != null )
				{
					GameManager.Player.Score += 50 / Leaderboard.Terries.Count;
					foreach ( var x in mp.PhysicsGroup.Bodies )
					{ 
						x.ApplyImpulse( impulse );
					}
				}
			}

			ExplosionSound.UI = true;
			ExplosionSound.Volume = Volume;
			var sound = Sound.Play( ExplosionSound, WorldPosition );
			sound.TargetMixer = Mixer.FindMixerByName( "Game" );
			Destroy( ExplosionPrefab.Clone( WorldPosition ) );
			GameObject.Destroy();
		}
	}

	static async void Destroy( GameObject obj )
	{
		await GameTask.DelayRealtimeSeconds( 5 );
		obj.Destroy();
	}
}
