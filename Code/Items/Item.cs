namespace Sandbox;

[AssetType(Name = "Item Definition", Extension = "item")]
public class ItemDefinition : GameResource
{
	[Property] public Texture ItemIcon { get; set; }
	[Property] public GameObject Prefab { get; set; }
	[Property] public bool Locked { get; set; }
	[Property] public bool ShowInShop { get; set; } = true;
	[Property] public bool Held { get; set; }
	[Property] public string Name { get; set; }
	[Property] public int Price { get; set; }
}
