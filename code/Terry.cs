using System.Linq;
using Sandbox;

public sealed class Terry : Component, Component.ICollisionListener
{
	[RequireComponent] private ModelPhysics Physics { get; set; }
	public Inventory Inventory;
	protected override void OnStart()
	{
		ModelPhysics physics = this.Components.Get<ModelPhysics>();
	}

	protected override void OnFixedUpdate()
	{
		
	}

	protected override void OnDisabled()
	{
		
	}
}
