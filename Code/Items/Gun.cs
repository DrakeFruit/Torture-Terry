using Sandbox;

namespace TortureTerry;
public class Gun : Component
{
	[Property] public float Damage { get; set; } = 5;
	[Property] public float PushForce { get; set; } = 1;
	[Property] public float Cooldown { get; set; } = 0.1f;
	[Property] public bool Automatic { get; set; } = false;
	
	[Property] public GameObject Barrel { get; set; }
	[Property] public GameObject MuzzleFlash { get; set; }
	[Property] public SoundEvent FireSound { get; set; }

	[Property, ToggleGroup( "ManualChambering" )] public bool ManualChambering { get; set; } = false;
	[Property, Group( "ManualChambering" )] public SoundEvent ChamberSound { get; set; }
	
	public DamageInfo DamageInf { get; set; } = new();
	public TimeSince Time { get; set; }
	public SceneTraceResult Tr { get; set; }

	protected override void OnStart()
	{
		DamageInf.Damage = Damage;
		DamageInf.Attacker = GameObject;
		DamageInf.Weapon = GameObject;
	}
	protected override void OnFixedUpdate()
	{
		if ( Input.Pressed( "attack2" ) && Time >= Cooldown || Input.Down( "attack2" ) && Automatic && Time >= Cooldown )
		{
			Shoot();
		}
	}

	void Shoot()
	{
		var tr = Scene.Trace
			.Ray( Barrel.WorldPosition, Barrel.WorldPosition + Barrel.WorldRotation.Left * 1000f )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();
		
		//Effects
		var flash = MuzzleFlash.Clone( Barrel.WorldPosition + Barrel.WorldRotation.Left * 8f );
		flash.LocalScale = 4;
		GameManager.Destroy( flash, 2 );
		FireSound.UI = true;
		Sound.Play( FireSound );

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
