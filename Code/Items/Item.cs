namespace Sandbox;

[GameResource("Item Definition", "item", "something to kill sausig wit", Category = "Item", Icon = "inventory_2")]
public class ItemDefinition : GameResource
{
	[Property] public Texture ItemIcon { get; set; }
	[Property] public GameObject Prefab { get; set; }
	[Property] public bool Locked { get; set; }
	[Property] public string Name { get; set; }
	[Property] public int Price { get; set; }
}
