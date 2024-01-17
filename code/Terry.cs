using System.Linq;
using Sandbox;

public sealed class Terry : Component
{
	protected override void OnStart()
	{
		ModelPhysics physics = this.Components.Get<ModelPhysics>();
	}
}