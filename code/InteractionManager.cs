using System;
using Sandbox;
using Sandbox.Physics;

public sealed class InteractionManager : Component
{
    [Property] Vector3 Gravity { get; set; }
    [Property] float Damping = 1f;

    GameObject HeldObject;
    PhysicsBody Body;
    PhysicsBody CursorBody;
    Sandbox.Physics.SpringJoint Spring;
    Vector3 GrabOffset;

	protected override void OnEnabled()
	{
		Scene.PhysicsWorld.Gravity = Gravity;
		CursorBody = new( Scene.PhysicsWorld );
	}
	protected override void OnFixedUpdate()
	{
        //trace a ray at the mouse position
        var tr = Scene.Trace.Ray( Scene.Camera.ScreenPixelToRay( Mouse.Position ), 1000 ).Run();
        CursorBody.Position = tr.EndPosition.WithX(0);

        //Pickup
        if(Input.Down("attack1") && tr.Hit && tr.GameObject.Tags.Has("pickup") && HeldObject == null && Body == null){
            HeldObject = tr.GameObject;
            Spring = PhysicsJoint.CreateSpring( CursorBody, tr.Body, 0, 1 );
            Spring.SpringLinear = Spring.SpringLinear with { Damping = Damping };
        }

        //Drop / Trashing
        if( !Input.Down( "attack1" ) ){
	        if( Spring.IsValid() ) Spring.Remove();
	        if(tr.GameObject.Tags.Has("trash") && Input.Released("attack1"))
	        {
		        Spring.Body2.GetGameObject().Destroy();
	        }

	        HeldObject = null;
	        Body = null;
        }

        //TODO: play some oof sounds
    }
}
