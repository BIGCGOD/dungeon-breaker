/// <summary>
/// Player character controller.
/// Player Controller by Keyboard and Mouse
/// </summary>

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterSystem))]
public class PlayerCharacterController : MonoBehaviour
{
	private CharacterSystem character;
	void Start()
	{
		character = gameObject.GetComponent<CharacterSystem>();

		Screen.lockCursor = true;
	}
	
	void Update()
	{
		var direction	= Vector3.zero;
		var forward	= Quaternion.AngleAxis(-90,Vector3.up) * Camera.main.transform.right;
		
		if(Screen.lockCursor)
		{
			if(Input.GetKey(KeyCode.W))
				direction	+= forward;
			if(Input.GetKey(KeyCode.S))
				direction	-= forward;
			if(Input.GetKey(KeyCode.A))
				direction	-= Camera.main.transform.right;
			if(Input.GetKey(KeyCode.D))
				direction	+= Camera.main.transform.right;
				
			if(Input.GetMouseButtonDown(0))
			{
				character.Attack();
			}

			var orbit	= Camera.main.gameObject.GetComponent<Orbit>();
			if(orbit != null)
			{
				orbit.Data.Azimuth	+= Input.GetAxis("Mouse X") / 100;
				orbit.Data.Zenith	+= Input.GetAxis("Mouse Y") / 100;
				orbit.Data.Zenith	= Mathf.Clamp(orbit.Data.Zenith,-0.8f,0f);
				orbit.Data.Length	+= (-6 - orbit.Data.Length) / 10;
			}

			if(Input.GetMouseButtonDown(1))
			{
				character.Attack();
				var skillDeployer	= this.gameObject.GetComponent<CharacterSkillDeployer>();
				if(skillDeployer != null)
					skillDeployer.DeployWithAttacking();	
			}
		}

		direction.Normalize();
		character.Move(direction);
	}
}

