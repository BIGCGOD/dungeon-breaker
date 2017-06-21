/// <summary>
/// AI character controller.
/// Just A basic AI Character controller 
/// will looking for a Target and moving to and Attacking
/// </summary>

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterSystem))]

public class AICharacterController : MonoBehaviour {

	public GameObject ObjectTarget;
	public string TargetTag = "Player";
	private CharacterSystem character;
	private int aiTime = 0;
	private int aiState = 0;
	void Start () {
		character = gameObject.GetComponent<CharacterSystem>();
	}
	
	
	void Update () {
		var direction = Vector3.zero;
		if(aiTime<=0){
			aiState = Random.Range(0,4);
			aiTime = Random.Range(10,100);
		}else{
			aiTime--;
		}
		if(ObjectTarget){
			float distance = Vector3.Distance(ObjectTarget.transform.position,this.gameObject.transform.position);
			
			if(distance<=2){
				transform.LookAt(ObjectTarget.transform.position);
				if(aiTime<=0){
					if(aiState == 1){
						character.Attack();
					}
				}
			}else{
				if(aiState == 1){
					transform.LookAt(ObjectTarget.transform.position);
					direction = this.transform.forward;
					direction.Normalize();
					character.Move(direction);
				}
			}
			
		}else{
			ObjectTarget = GameObject.FindGameObjectWithTag(TargetTag);	
		}
	}
}
