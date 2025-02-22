using System;
using Sandbox;
using Sandbox.Physics;

public sealed class InteractionManager : Component
{
    [Property] Vector3 Gravity { get; set; }
    private PhysicsBody HeldBody;
    private GameObject HeldObject;
    private PhysicsBody CursorBody;
    private Sandbox.Physics.FixedJoint GrabJoint;
    
	protected override void OnEnabled()
	{
		Scene.PhysicsWorld.Gravity = Gravity;
		
		Clear();
		
		CursorBody = new PhysicsBody( Scene.PhysicsWorld ) { BodyType = PhysicsBodyType.Keyframed };
	}
	
	protected override void OnFixedUpdate()
	{
		var tr = Scene.Trace.Ray( Scene.Camera.ScreenPixelToRay( Mouse.Position ), 1000 ).Run();

		if ( Input.Down( "attack1" ) ) Pickup( tr );
		else if ( HeldBody.IsValid() ) Drop( tr );

		CursorBody.Position = tr.EndPosition.WithX( 0 );
	}

	public void Pickup(SceneTraceResult tr)
	{
		if ( !tr.Hit || tr.Body is null || tr.Body.BodyType == PhysicsBodyType.Static ) return;

		HeldBody = tr.Body;
		HeldObject = tr.GameObject;

		var localOffset = HeldBody.Transform.PointToLocal( tr.HitPosition );

		GrabJoint?.Remove();
		GrabJoint = PhysicsJoint.CreateFixed( new PhysicsPoint( CursorBody ), new PhysicsPoint( HeldBody ) );
		GrabJoint.Point1 = new PhysicsPoint( CursorBody );
		GrabJoint.Point2 = new PhysicsPoint( HeldBody, localOffset );

		var maxForce = 100.0f * tr.Body.Mass * Scene.PhysicsWorld.Gravity.Length;
		GrabJoint.SpringLinear = new PhysicsSpring( 25, 5, maxForce );
		GrabJoint.SpringAngular = new PhysicsSpring( 0, 0, 0 );
	}

	public void Drop(SceneTraceResult tr)
	{
		GrabJoint?.Remove();
		GrabJoint = null;
		HeldBody = null;
		HeldObject = null;
			
		if(tr.GameObject.Tags.Has("trash") && Input.Released("attack1"))
		{
			GrabJoint?.Body2.GetGameObject().Destroy();
		}
	}
	
	private void Clear()
	{
		GrabJoint?.Remove();
		GrabJoint = null;

		CursorBody?.Remove();
		CursorBody = null;

		HeldBody = null;
		HeldObject = null;
	}
	
	protected override void OnDisabled()
	{
		Clear();
	}
	
	protected override void OnDestroy()
	{
		Clear();
	}
}
