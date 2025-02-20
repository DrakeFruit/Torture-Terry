using System.Linq;
using Sandbox;

public sealed class Terry : Component, Component.ICollisionListener
{
	int score = 0;
	protected override void OnStart()
	{
		ModelPhysics physics = this.Components.Get<ModelPhysics>();
	}

	void ICollisionListener.OnCollisionStart( Collision other)
	{
		float damage = other.Contact.NormalSpeed / 2000;
		score += (int)damage;
		if(damage >= 1) Log.Info(score);
	}

	void ICollisionListener.OnCollisionUpdate( Collision other )
	{
		Gizmo.Draw.Color = Color.White;
		Gizmo.Draw.SolidSphere( other.Contact.Point, 4f );
		Gizmo.Draw.Color = Gizmo.Colors.Red;
		Gizmo.Draw.Line( other.Contact.Point, other.Contact.Point + other.Contact.NormalSpeed * 50f );
		Log.Info(other.Contact.Point);
	}

	void ICollisionListener.OnCollisionStop( CollisionStop other )
	{

	}
}