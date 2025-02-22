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
    private float HeldAngularDamping;
    
	protected override void OnEnabled()
	{
		Scene.PhysicsWorld.Gravity = Gravity;
		
		Clear();
		
		CursorBody = new PhysicsBody( Scene.PhysicsWorld ) { BodyType = PhysicsBodyType.Keyframed };
	}
	
	protected override void OnFixedUpdate()
	{
		var tr = Scene.Trace.Ray( Scene.Camera.ScreenPixelToRay( Mouse.Position ), 1000 ).Run();

		if ( Input.Down( "attack1" ) && !HeldBody.IsValid() ) Pickup( tr );
		if ( !Input.Down( "attack1" ) && HeldBody.IsValid() ) Drop( tr );

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

		float maxForce;
		var phys = HeldObject.Components.GetInChildrenOrSelf<ModelPhysics>();
		if ( phys.IsValid() )
		{
			maxForce = 100.0f * phys.PhysicsGroup.Mass * Scene.PhysicsWorld.Gravity.Length;
		} else maxForce = 100.0f * tr.Body.Mass * Scene.PhysicsWorld.Gravity.Length;
		
		GrabJoint.SpringLinear = new PhysicsSpring( 15, 1, maxForce );
		GrabJoint.SpringAngular = new PhysicsSpring( 15, 1, 0 );

		HeldAngularDamping = HeldBody.AngularDamping; //Keep track of angular damping value before pickup
	}

	public void Drop(SceneTraceResult tr)
	{
		if(tr.GameObject.Tags.Has("trash"))
		{
			GrabJoint?.Body2.GetGameObject().Destroy();
		}
		
		HeldBody.AngularDamping = HeldAngularDamping; //Reset angular damping
		
		GrabJoint?.Remove();
		GrabJoint = null;
		HeldBody = null;
		HeldObject = null;
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
