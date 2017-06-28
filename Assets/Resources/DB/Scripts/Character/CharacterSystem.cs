using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public delegate void Action();

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(CharacterStatus))]
[RequireComponent(typeof(CharacterAttack))]
[RequireComponent(typeof(CharacterInventory))]

// 1.����frozetime�뱻������ 2.��ӵ����ж�������ӵ�е������������ر����������ܻ������׶�

// Start()                  ��ʼ����ض���
// Update()                 �������״̬(�˺��ж��׶Σ����������׶ε�)
// GotHit()                 public �����ܻ�״̬��ͣ��ʱ��
// resetCombo()             ���ù���״̬
// fightAnimation()         ���Ź���������������diddamaged�����˺��ж�
// Attack()                 public ����ָ��
// Move()                   public ��Ŀ�귽���ƶ�������moveDirectionʱת�򣬸���ת������ƶ����룬�޸��ܲ������ٶȣ�����վ���������ܲ�����

public class CharacterSystem : MonoBehaviour
{
    public float MoveSpeed = 0.2f; // Move speed
	public float TurnSpeed = 5; // turning speed

    public int actionIndex;
    public float attackSpeed;
    public Action doSomething;
    public Action doWhenStop;

    public string[] PoseHitNames;// pose animation when character got hit
	public string PoseIdle = "Idle";
	public string PoseRun = "Run";
	public bool IsHero;

    //private variable
    public float frozetime;
    public enum STATUS { NORMAL, PREPARE, ATTACK, FINISH, HITED };
    public STATUS status;
    public bool notMove;

    CharacterMotor motor;
    CharacterAttack attack;
    ActionManager actionManager;

    void Start()
	{
		motor = gameObject.GetComponent<CharacterMotor>();
        attack = gameObject.GetComponent<CharacterAttack>();
        // Play pose Idle first
        gameObject.GetComponent<Animation>().CrossFade(PoseIdle);
        actionManager = (ActionManager)FindObjectOfType(typeof(ActionManager));

        status = STATUS.NORMAL;
        notMove = false;
    }

    void Update()
    {
        // Animation combo system
        if (status == STATUS.PREPARE || status == STATUS.ATTACK ||
            status == STATUS.FINISH)
        {
            // checking index of PoseAttackNames list

            AnimationState attackState = this.gameObject.GetComponent<Animation>()[
                ((BaseAction)actionManager.actionHash[actionIndex]).animationName]; // get animation PoseAttackNames[poseIndex]
            attackState.layer = 2;
            attackState.blendMode = AnimationBlendMode.Blend;
            attackState.speed = attackSpeed;

            if (attackState.time >= attackState.length * 0.9)
            {
                status = STATUS.NORMAL;
            }
            // if the time of attack animation is running to 80% of animation. It's should be Finish this pose.
            else if (attackState.time >= ((BaseAction)actionManager.actionHash[actionIndex]).finishTime)
            {
                //attackState.normalizedTime = attackState.length;
                status = STATUS.FINISH;
                doWhenStop();
                doWhenStop = () => { };
            }
            // if the time of attack animation is running to marking point (PoseAttackTime[poseIndex]) 
            // calling CharacterAttack.cs to push a damage out
            else if (attackState.time >= ((BaseAction)actionManager.actionHash[actionIndex]).prepareTime)
            {
                status = STATUS.ATTACK;
                doSomething();
            }
        }

        // Freeze when got hit
        if (status == STATUS.HITED){
			if(frozetime > 0){
				frozetime--;
			}else{
                status = STATUS.NORMAL;
				this.gameObject.GetComponent<Animation>().Play(PoseIdle);
			}
		}
	}
	
	public void GotHit(float time){
		if(!IsHero){
			if(PoseHitNames.Length > 0){
				// play random Hit animation
				this.gameObject.GetComponent<Animation>().Play(PoseHitNames[Random.Range(0,PoseHitNames.Length)], PlayMode.StopAll);
			}
			frozetime = time * Time.deltaTime;// froze time when got hit
            status = STATUS.NORMAL;
		}
	}
	
	public void Attack()
	{
		if(frozetime <= 0){
            attack.attackStackTimeTemp = Time.time;
            attack.fightAnimation();
            attack.attackStack += 1;
		}
	}
    
	public void Move(Vector3 dir){
        if (!notMove )
        {
            if (status == STATUS.PREPARE)
                moveDirection = dir * ((BaseAction)actionManager.actionHash[actionIndex]).reducedMoveSpeed[0];
            else if (status == STATUS.ATTACK)
                moveDirection = dir * ((BaseAction)actionManager.actionHash[actionIndex]).reducedMoveSpeed[1];
            else if (status == STATUS.FINISH)
                moveDirection = dir * ((BaseAction)actionManager.actionHash[actionIndex]).reducedMoveSpeed[2];
            else if (status == STATUS.HITED)
                moveDirection = dir * 0.2f;
            else
                moveDirection = dir;
        }
	}
	
	Vector3 direction;
	
	private Vector3 moveDirection
	{
		get { return direction; }
		set
		{
			direction = value;
			if(direction.magnitude > 0.1f)
	    	{
	    		var newRotation	= Quaternion.LookRotation(direction);
				transform.rotation	= Quaternion.Slerp(transform.rotation,newRotation,Time.deltaTime * TurnSpeed);
			}
			direction *= MoveSpeed * 0.5f * (Vector3.Dot(gameObject.transform.forward,direction) + 1);
			
			if(direction.magnitude > 0.001f)
			{
				// Play Runing Animation when moving
				float speedaimation = direction.magnitude * 3;
				gameObject.GetComponent<Animation>().CrossFade(PoseRun);
				if(speedaimation < 1){
					speedaimation = 1;
				}
				// Speed animation sync to Move Speed
				gameObject.GetComponent<Animation>()[PoseRun].speed	= speedaimation;
			}
			else{
				// Play Idle Animation when stoped
				gameObject.GetComponent<Animation>().CrossFade(PoseIdle);
			}
			if(motor){
				motor.inputMoveDirection = direction;
			}
		}
	}
	
	float pushPower = 2.0f;

	void OnControllerColliderHit(ControllerColliderHit hit)// Character can push an object.
	{
	    var body = hit.collider.attachedRigidbody;
	    if(body == null || body.isKinematic){
			return;
		}
	    if(hit.moveDirection.y < -0.3){
			return;
		}
		
	    var pushDir = Vector3.Scale(hit.moveDirection,new Vector3(1,0,1));
	    body.velocity = pushDir * pushPower;
	}
}
