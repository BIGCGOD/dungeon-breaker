/// <summary>
/// Damage skill.
/// will ApplayDamage() to <CharacterStatus>Object by around the radius
/// </summary>
using UnityEngine;
using System.Collections;

public class DamageSkill : MonoBehaviour {

	public int Force;
	public string TagDamage;
	public int Damage;
	public float Radius;
	
	void Start () {
        var colliders = Physics.OverlapSphere(this.transform.position, Radius);
        foreach(var hit in colliders)
		{
            if(!hit)
            	continue;
				
			if(hit.tag == TagDamage){	
				if(hit.gameObject.GetComponent<CharacterStatus>()){
					hit.gameObject.GetComponent<CharacterStatus>().ApplayDamage(Damage,Vector3.zero);
				}
			}
			
			if (hit.GetComponent<Rigidbody>()){
                hit.GetComponent<Rigidbody>().AddExplosionForce(Force, transform.position, Radius, 3.0f);
			}
        }
	}

}
