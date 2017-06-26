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

    public float Speed	= 2; // Move speed
	public float TurnSpeed	= 5; // turning speed

    public float PoseSpeed;
    public float PoseTime;
    public string PoseName;
    public Action doSomething;
    public Action doWhenStop;

    public string[] PoseHitNames;// pose animation when character got hit
	public string PoseIdle = "Idle";
	public string PoseRun = "Run";
	public bool IsHero;

    //private variable
    public float frozetime;
    public enum STATUS { NORMAL, ATTACKPREPARE, ATTACKING, SKILLPREPARE, SKILLING, NOTMOVE, HITED };
    public STATUS status;

    CharacterMotor motor;
    CharacterAttack attack;

    void Start()
	{
		motor = gameObject.GetComponent<CharacterMotor>();
        attack = gameObject.GetComponent<CharacterAttack>();
        // Play pose Idle first
        gameObject.GetComponent<Animation>().CrossFade(PoseIdle);

        status = STATUS.NORMAL;
    }

    void Update()
    {
        // Animation combo system
        if (status == STATUS.ATTACKPREPARE || status == STATUS.ATTACKING ||
            status == STATUS.SKILLPREPARE || status == STATUS.SKILLING)
        {
            // checking index of PoseAttackNames list

            AnimationState attackState = this.gameObject.GetComponent<Animation>()[PoseName]; // get animation PoseAttackNames[poseIndex]
            attackState.layer = 2;
            attackState.blendMode = AnimationBlendMode.Blend;
            attackState.speed = PoseSpeed;

            // set attacking to True when time of attack animation is running to 10% of animation
            if (attackState.time >= attackState.length * 0.1f)
            {
                status = status == STATUS.ATTACKPREPARE ? STATUS.ATTACKING : STATUS.SKILLING;
            }
            // if the time of attack animation is running to marking point (PoseAttackTime[poseIndex]) 
            // calling CharacterAttack.cs to push a damage out
            if (attackState.time >= PoseTime)
            {
                doSomething();
            }
            // if the time of attack animation is running to 80% of animation. It's should be Finish this pose.
            if (attackState.time >= attackState.length * 0.8f)
            {
                attackState.normalizedTime = attackState.length;
                status = STATUS.NORMAL;
                doWhenStop();
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
        if (status != STATUS.NOTMOVE)
        {
            if (status != STATUS.ATTACKING)
                moveDirection = dir;
            else
                moveDirection = dir / 2f;
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
			direction *= Speed * 0.5f * (Vector3.Dot(gameObject.transform.forward,direction) + 1);
			
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
