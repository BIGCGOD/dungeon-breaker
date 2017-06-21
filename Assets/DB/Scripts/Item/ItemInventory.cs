/// <summary>
/// Item inventory.
/// this file is Strcture of Embedded item using for adding into the Embedded prefab
/// Such as Weapon , Sword , Shield
/// </summary>

using UnityEngine;
using System.Collections;

public class ItemInventory : MonoBehaviour
{	
	public int Damage = 0;
	public int Defend = 0;
	
	public int ItemEmbedSlotIndex = 0; // index of Inventorys embeded
	public AudioClip[] SoundHit; // Soung when hit
	public float SpeedAttack = 1; // Attack Speed

	
	void Start () {
	
	}
	
	
	void Update () {
	
	}
}
