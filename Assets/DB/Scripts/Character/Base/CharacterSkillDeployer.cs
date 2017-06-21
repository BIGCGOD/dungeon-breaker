/// <summary>
/// Character skill deployer.
/// Basic Skill Launcher system
/// </summary>

using UnityEngine;
using System.Collections;


public class CharacterSkillDeployer : MonoBehaviour {

	public GameObject[] Skill;//List of Launch object
	public int[] ManaCost;// List of Mana cost
	public Texture2D[] SkillIcon;// List of Skill image
	
	private CharacterStatus character;
	private CharacterAttack characterAttack;
	private bool attackingSkill;
	
	void Start()
	{
		if(this.gameObject.GetComponent<CharacterStatus>()){
			character = this.gameObject.GetComponent<CharacterStatus>();
		}
		if(this.gameObject.GetComponent<CharacterAttack>()){
			characterAttack = this.gameObject.GetComponent<CharacterAttack>();
		}	
	}

	public int indexSkill;// Current skill index
	public void DeploySkill(int index)
	{
		indexSkill = index;
		DeploySkill();
	}
	
	
	
	public void DeploySkill()
	{
		// Launch an ojbect sync with Animation Attacking
		if(Skill.Length > 0 && Skill[indexSkill] != null)
		{
			if(character != null && character.SP >= ManaCost[indexSkill])
			{
				var skill = (GameObject)GameObject.Instantiate(Skill[indexSkill],this.transform.position,this.transform.rotation);
				skill.transform.forward	= this.transform.forward;
				character.SP	-= ManaCost[indexSkill];
			}
		}
	}
	
	
	public void DeployWithAttacking(int index){
		indexSkill = index;
		attackingSkill = true;
	}
	
	public void DeployWithAttacking(){
		attackingSkill = true;
	}
	

	void Update()
	{
		if(attackingSkill && characterAttack.Activated)
		{
			DeploySkill();

			attackingSkill = false;
			characterAttack.Activated = false;
		}
	}
}
