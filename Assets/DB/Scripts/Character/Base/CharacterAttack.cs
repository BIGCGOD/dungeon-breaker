/// <summary>
/// Character attack.
/// this class contail a DoDamage() function to Push a damage out
/// using for called by <CharacterSystem>
/// </summary>

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CharacterAttack : MonoBehaviour
{
	public float Direction = 0.5f;// Direction of object can hit 
	public float Radius	= 1;
	public int Force = 500;
	public AudioClip[] SoundHit;
	public bool Activated;
	public GameObject FloatingText;

	HashSet<GameObject> listObjHitted = new HashSet<GameObject>();// list of hited objects
	public void StartDamage()
	{
		listObjHitted.Clear();// Clear list of hited objects
		Activated = false;
	}
	
	
	private void AddObjHitted(GameObject obj){
		listObjHitted.Add(obj);
	}
	
	void Update()
	{

	}
	
	
	public void AddFloatingText(Vector3 pos,string text){
		// Adding Floating Text Effect
		if(FloatingText){
			var floattext = (GameObject)Instantiate(FloatingText,pos,transform.rotation);
			if(floattext.GetComponent<FloatingText>()){
				floattext.GetComponent<FloatingText>().Text = text;
			}
			GameObject.Destroy(floattext,1);	
		}
	}
	
	
	
	public void DoDamage()
	{
		Activated = true;
	    var explosionPos	= transform.position;
	    var colliders	= Physics.OverlapSphere(explosionPos,Radius);
		foreach(var hit in colliders)
		{
	        if (!hit || hit.gameObject == this.gameObject || hit.gameObject.tag == this.gameObject.tag)
	            continue;

			if(listObjHitted.Contains(hit.gameObject))
				continue;

	        var dir	= (hit.transform.position - transform.position).normalized;
	        var direction = Vector3.Dot(dir,transform.forward);
			if(direction < Direction)// Only hit an object in the direction.
				continue;

			var orbit	= Camera.main.gameObject.GetComponent<OrbitGameObject>();
			if(orbit != null && orbit.Target == hit.gameObject)
				ShakeCamera.Shake(0.5f,0.5f);

			var dirforce = (this.transform.forward + transform.up) * Force;
			if(hit.gameObject.GetComponent<CharacterStatus>())
			{
				if(SoundHit.Length>0){
					int randomindex = Random.Range(0,SoundHit.Length);
					if(SoundHit[randomindex]!=null){
						AudioSource.PlayClipAtPoint(SoundHit[randomindex],this.transform.position);	
					}	
				}
				
				int damage = this.gameObject.GetComponent<CharacterStatus>().Damage;
				int damageCal = (int)Random.Range(damage/2.0f,damage)+1;
				var status	= hit.gameObject.GetComponent<CharacterStatus>();
				int takedamage = status.ApplayDamage(damageCal,dirforce);

				// Add Particle Effect
				AddFloatingText(hit.transform.position + Vector3.up,takedamage.ToString());
				status.AddParticle(hit.transform.position + Vector3.up);
			}
			if(hit.GetComponent<Rigidbody>()){
				// push rigidbody object
				hit.GetComponent<Rigidbody>().AddForce(dirforce);
			}
			// add this object to the list. We wont make a multiple hit in the same object
			AddObjHitted(hit.gameObject);
    	}
	}
}

