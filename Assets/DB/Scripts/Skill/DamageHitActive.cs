/// <summary>
/// Damage hit active.
/// Spawing an Object when got hit by TriggerEnter
/// </summary>

using UnityEngine;
using System.Collections;

public class DamageHitActive : MonoBehaviour {
	
	public GameObject explosiveObject;
	public string TagDamage;
	void OnTriggerEnter(Collider other) {
		if(other.tag == TagDamage){
			if(explosiveObject){
				GameObject.Instantiate(explosiveObject,this.transform.position,this.transform.rotation);
				GameObject.Destroy(this.gameObject);
			}
			GameObject.Destroy(this.gameObject);
		}
	 	
	}
}
