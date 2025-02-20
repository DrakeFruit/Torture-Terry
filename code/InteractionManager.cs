using System;
using System.Diagnostics;
using System.Linq;
using Sandbox;
using Sandbox.Utility.Svg;

public sealed class InteractionManager : Component
{
    [Property] Vector3 gravity { get; set; }
    [Property] float damping = 1f;

    GameObject heldObject = null;
    ModelPhysics terryPhys;
    PhysicsBody body;
    Vector3 cursorPosition;
    Vector3 grabOffset;

	protected override void OnEnabled()
	{
		Scene.PhysicsWorld.Gravity = gravity;
	}
	protected override void OnFixedUpdate()
    {
        //trace a ray at the mouse position
        var tr = Scene.Trace.Ray((Scene.Camera.ScreenPixelToRay( Mouse.Position )), 1000f).Run();
        cursorPosition = tr.EndPosition.WithX(0);

        //get some references when mouse1 is held down
        if(Input.Down("attack1") && tr.Hit && tr.GameObject.Tags.Has("pickup") && heldObject == null && body == null){
            heldObject = tr.GameObject;
            terryPhys = tr.GameObject.Components.Get<ModelPhysics>();
            body = tr.Body;
            grabOffset = cursorPosition - body.Transform.Position;

            if(body.PhysicsGroup != null){
                body.PhysicsGroup.LinearDamping = Math.Clamp( damping * (cursorPosition - body.Transform.Position).Length, 0, 2);
            }
        }

        //move the object around
		if(body != null){
            body.Velocity = 0;
            body.AngularVelocity = 0;

            //move the held object
            Transform heldTransform = new Transform(cursorPosition, new Rotation(0, 0, 0, 0));
            body.SmoothMove( heldTransform.Position - grabOffset, .1f, Time.Delta );
        }
        if(Input.Released("attack1") && heldObject != null && body != null){
            TrashObjects(tr, heldObject);
            if(body.PhysicsGroup != null){
            body.PhysicsGroup.LinearDamping = 0f;
            }

            body.MotionEnabled = true;
            heldObject = null;
            body = null;
        }

        //TODO: play some oof sounds
    }

    static void TrashObjects(SceneTraceResult tr, GameObject heldObject){
        if(tr.GameObject.Tags.Has("trash") && Input.Released("attack1"))
        {
            heldObject.Destroy();
        }
            
    }
}
