using System.Linq;
using Sandbox;

public sealed class Terry : Component, Component.ICollisionListener
{
	protected override void OnStart()
	{
		ModelPhysics physics = this.Components.Get<ModelPhysics>();
	}

	public void OnCollisionStart( Collision other )
	{
		if(other.Contact.NormalSpeed > 10 ){

		}
		Log.Info(other.Contact.NormalSpeed);
	}

	public void OnCollisionUpdate( Collision other )
	{

	}

	public void OnCollisionStop( CollisionStop other )
	{

	}
}