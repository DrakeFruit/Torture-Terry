using System.Diagnostics;
using System.Linq;
using Sandbox;

public sealed class InteractionManager : Component
{
    bool terrySelected = false;
    GameObject terry = null;
    ModelPhysics terryPhys;
    PhysicsBody limb;
    Vector3 cursorPosition;

	protected override void OnFixedUpdate()
    {
        //trace a ray at the mouse position
        var tr = Scene.Trace.Ray((Scene.Camera.ScreenPixelToRay( Mouse.Position )), 1000f).WithoutTags("ignore").Run();
        cursorPosition = tr.EndPosition.WithX(0);

        //get some references when mouse1 is held down
        if(Input.Down("attack1") && tr.Hit && tr.GameObject.Tags.Has("terry") && terry == null){
            terry = tr.GameObject;
            terryPhys = tr.GameObject.Components.Get<ModelPhysics>();
            limb = tr.Body;
        }
        //move terry's limb to the cursor's position
		if(limb != null){
            limb.MotionEnabled = false;
            limb.Velocity = 0;
            limb.AngularVelocity = 0;

            limb.SmoothMove( cursorPosition, .1f, Time.Delta );
        }
        if(Input.Released("attack1") && terry != null && limb != null){
            limb.MotionEnabled = true;
            terry = null;
            limb = null;
        }

        //play some impact sounds
    }
}