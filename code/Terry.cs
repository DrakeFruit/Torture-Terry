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

	}

	public void OnCollisionUpdate( Collision other )
	{
		Gizmo.Draw.Color = Color.White;
		Gizmo.Draw.SolidSphere( other.Contact.Point, 4f );
		Gizmo.Draw.Color = Gizmo.Colors.Red;
		Gizmo.Draw.Line( other.Contact.Point, other.Contact.Point + other.Contact.NormalSpeed * 50f );
		Log.Info(other.Contact.Point);
	}

	public void OnCollisionStop( CollisionStop other )
	{

	}
}