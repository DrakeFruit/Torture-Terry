using Sandbox;

namespace TortureTerry;
public class Gun : Component
{
	[Property] public float Damage { get; set; } = 5;
	[Property] public float PushForce { get; set; } = 1;
	[Property] public float Cooldown { get; set; } = 0.1f;
	[Property] public int Capacity { get; set; } = 8;
	
	[Property] public GameObject Barrel { get; set; }
	[Property] public GameObject MuzzleFlash { get; set; }
	[Property] public SoundEvent FireSound { get; set; }
	[Property] public SoundEvent ReloadSound { get; set; }

	[Property, ToggleGroup( "ManualChambering" )] public bool ManualChambering { get; set; } = false;
	[Property, Group( "ManualChambering" )] public SoundEvent ChamberSound { get; set; }
	
	public DamageInfo DamageInf { get; set; } = new();
	public TimeSince Time { get; set; }
	public SceneTraceResult Tr { get; set; }
	public SoundHandle ReloadHandle { get; set; }
	public bool Reloading { get; set; }
	public int Ammo { get; set; }

	protected override void OnStart()
	{
		Ammo = Capacity;
		DamageInf.Damage = Damage;
		DamageInf.Attacker = GameObject;
		DamageInf.Weapon = GameObject;
	}
	protected override void OnFixedUpdate()
	{
		if ( !ReloadHandle.IsValid() && Reloading )
		{
			Ammo = Capacity;
			Reloading = false;
		}
		
		if ( Input.Down( "attack2" ) && Time >= Cooldown) Shoot();
	}

	void Shoot()
	{
		if ( Reloading ) return;
		if ( Ammo <= 0 )
		{
			Reloading = true;
			ReloadSound.UI = true;
			ReloadHandle = Sound.Play( ReloadSound );
			return;
		}
		
		Ammo--;
		
		var tr = Scene.Trace
			.Ray( Barrel.WorldPosition, Barrel.WorldPosition + Barrel.WorldRotation.Left * 1000f )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();
		
		//Effects
		var flash = MuzzleFlash.Clone( Barrel.WorldPosition + Barrel.WorldRotation.Left * 8f );
		flash.LocalScale = 4;
		GameManager.Destroy( flash, 2 );
		FireSound.UI = true;
		var snd = Sound.Play( FireSound );

		if ( ManualChambering )
		{
			ChamberSound.UI = true;
			Sound.Play( ChamberSound );
		}
		
		//Damage
		if ( tr.Body.IsValid() )
		{
			DamageInf.Position = tr.Body.Position;
			tr.Body.ApplyImpulse( tr.Direction.Normal * PushForce * 25000 );
			var terry = tr.GameObject.GetComponent<Terry>();
			
			if ( terry.IsValid() ) terry.OnDamage( DamageInf );
		}
		
		Time = 0;
	}

	protected override void OnPreRender()
	{
		DebugOverlay.Line( Barrel.WorldPosition, Barrel.WorldPosition + Barrel.WorldRotation.Left * 1000f );
	}
}
