/// <summary>
/// Item type. is an Item Collector Structure 
/// using for show up in the Inventory GUI System
/// included
/// - Price
/// - ItemType
/// - Description
/// - Icon
/// - Etc..
/// </summary>


using UnityEngine;
using System.Collections;

public enum ItemType
{
	Weapon	= 0,
	Edible	= 1,
}

public struct ItemCollector
{
	public GameObject ItemPrefab;
	public GameObject ItemPrefabDrop;
	public int Price;
	public ItemType ItemType;
	public string Name;
	public string Description;
	public Texture2D Icon;
}
