/// <summary>
/// Character skill deployer.
/// Basic Skill Launcher system
/// </summary>

using UnityEngine;
using System.Collections;

// ʹ�õĹ���������̧��ϵͳ���ȴ��޸�(ʩ�������������ӳٻ��Ǽ���̧��)
// DeploySkill(int)                 ���ò��������ܶ���
// DeploySkill()                    �������ܶ���
// DeployWithAttacking(int)         �ͷż���
// DeployWithAttacking()            ����spֵ,�����ͷż���

public class CharacterSkillDeployer : MonoBehaviour {

    //******** skill message ********//
    public int[] skillIndexList = new int[2] { 2, 3 };

    private CharacterSystem system;
    private CharacterStatus character;
    private CharacterAttack characterAttack;
    public ActionManager actionManager;

    void Start()
    {
        if (this.gameObject.GetComponent<CharacterStatus>())
            character = this.gameObject.GetComponent<CharacterStatus>();
        if (this.gameObject.GetComponent<CharacterAttack>())
            characterAttack = this.gameObject.GetComponent<CharacterAttack>();
        if (this.gameObject.GetComponent<CharacterSystem>())
            system = this.gameObject.GetComponent<CharacterSystem>();

        actionManager = (ActionManager)FindObjectOfType(typeof(ActionManager));
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
        var skill = (GameObject)GameObject.Instantiate(
            ((BaseAction)actionManager.actionHash[skillIndexList[indexSkill]]).prefab, this.transform.position, this.transform.rotation);
        skill.transform.forward = this.transform.forward;
        character.SP -= ((BaseAction)actionManager.actionHash[skillIndexList[indexSkill]]).manaCost;

        system.doSomething = () => { };
    }

    public void DeployWithAttacking(int index) {
        indexSkill = index;
        DeployWithAttacking();
    }

    public void DeployWithAttacking()
    {
        if (indexSkill < skillIndexList.Length)
        {
            if (character != null && character.SP >= ((BaseAction)actionManager.actionHash[skillIndexList[indexSkill]]).manaCost)
            {
                characterAttack.resetCombo();
                characterAttack.StartDamage();
                giveSystemMessage();
                this.gameObject.GetComponent<Animation>().Play("Idle", PlayMode.StopAll);

                this.gameObject.GetComponent<Animation>().Play(
                    ((BaseAction)actionManager.actionHash[skillIndexList[indexSkill]]).animationName, PlayMode.StopAll);
                system.status = CharacterSystem.STATUS.PREPARE;
            }
        }
    }

     void giveSystemMessage()
    {
        system.attackSpeed = ((BaseAction)actionManager.actionHash[skillIndexList[indexSkill]]).speed;
        system.actionIndex = skillIndexList[indexSkill];
        system.doSomething = DeploySkill;
        system.doWhenStop = () => { };
    }
}
