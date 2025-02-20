namespace Sandbox;

[GameResource("Item Definition", "item", "something to kill sausig wit", Category = "Item", Icon = "inventory_2")]
public class Item : GameResource
{
	[Property] public Texture ItemIcon { get; set; }
	[Property] public GameObject Prefab { get; set; }
}
