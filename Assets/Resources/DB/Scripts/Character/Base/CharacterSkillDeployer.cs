/// <summary>
/// Character skill deployer.
/// Basic Skill Launcher system
/// </summary>

using UnityEngine;
using System.Collections;

// 使用的攻击动作的抬手系统，等待修改(施法动作，动作延迟还是即刻抬手)
// DeploySkill(int)                 设置技能
// DeploySkill()                    创建技能对象，计算sp值
// DeployWithAttacking(int)         部署并释放技能
// DeployWithAttacking()            释放技能

public class CharacterSkillDeployer : MonoBehaviour {

    //******** skill message ********//
    public GameObject[] Skill;// List of Launch object
    public string[] PoseSkillName;// List of Skill pose animation
    public float[] PoseASkillTime;// list of time damage marking using to sync with skill animation
    public bool[] isCurrent;// wait other animation or play immediate
    public float[] SpeedSkill;// Skill speed
    public int[] ManaCost;// List of Mana cost
	public Texture2D[] SkillIcon;// List of Skill image
    //****************************//
	
	private CharacterStatus character;
	private CharacterAttack characterAttack;
	private bool attackingSkill;
    private int currentSkill;
	
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
                var skill = (GameObject)GameObject.Instantiate(Skill[indexSkill], this.transform.position, this.transform.rotation);
                skill.transform.forward = this.transform.forward;
                character.SP -= ManaCost[indexSkill];
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
