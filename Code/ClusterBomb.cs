using Sandbox;

public sealed class ClusterBomb : Component
{
	[Property] float Speed { get; set; }
	protected override void OnStart()
	{
		foreach ( var i in Components.GetAll<Rigidbody>( FindMode.EverythingInChildren ) )
		{
			i.Velocity += Vector3.Random.Normal.WithX( 0 ) * Speed;
			i.LocalRotation = Rotation.Random;
		}
	}
}
