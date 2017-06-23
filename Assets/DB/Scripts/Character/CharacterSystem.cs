using UnityEngine;

using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(CharacterStatus))]
[RequireComponent(typeof(CharacterAttack))]
[RequireComponent(typeof(CharacterInventory))]

// 1.完善frozetime与被击动作 2.添加倒地判定，倒地拥有倒地连击，倒地保护，起身受击三个阶段

// Start()                  初始化相关对象
// Update()                 更新相关状态(伤害判定阶段，攻击动作阶段等)
// GotHit()                 public 更改受击状态和停滞时间
// resetCombo()             重置攻击状态
// fightAnimation()         播放攻击动画，并重置diddamaged攻击伤害判定
// Attack()                 public 攻击指令
// Move()                   public 向目标方向移动，设置moveDirection时转向，根据转向调整移动距离，修改跑步动画速度，管理站立动画和跑步动画

public class CharacterSystem : MonoBehaviour
{
	public float Speed	= 2; // Move speed
	public float SpeedAttack = 1.5f; // Attack speed
	public float TurnSpeed	= 5; // turning speed

	public float[] PoseAttackTime;// list of time damage marking using to sync with attack animation
	public string[] PoseAttackNames;// list of attack animation
	public string[] ComboAttackLists;// list of combo set
	
	public string[] PoseHitNames;// pose animation when character got hit
	public int WeaponType; // type of attacking
	public string PoseIdle = "Idle";
	public string PoseRun = "Run";
	public bool IsHero;
	
	//private variable
	private bool diddamaged;
	private int attackStep = 0;
	private string[] comboList;
	private int attackStack;
	private float attackStackTimeTemp;
	private float frozetime;
	private bool hited;
	private bool attacking;
	
	CharacterMotor motor;
	
	void Start()
	{
		motor = gameObject.GetComponent<CharacterMotor>();
		// Play pose Idle first
		gameObject.GetComponent<Animation>().CrossFade(PoseIdle);
		attacking = false;
	}
    
	void Update()
	{
		// Animation combo system
		
		if(ComboAttackLists.Length<=0){// if have no combo list
			return;
		}

		comboList = ComboAttackLists[WeaponType].Split(","[0]);// Get list of animation index from combolists split by WeaponType
		
		if(comboList.Length > attackStep){
			int poseIndex = int.Parse(comboList[attackStep]);// Read index of current animation from combo array
			if(poseIndex < PoseAttackNames.Length){
				// checking index of PoseAttackNames list
				
				AnimationState attackState = this.gameObject.GetComponent<Animation>()[PoseAttackNames[poseIndex]]; // get animation PoseAttackNames[poseIndex]
				attackState.layer = 2;
    			attackState.blendMode = AnimationBlendMode.Blend;
				attackState.speed = SpeedAttack;

                // set attacking to True when time of attack animation is running to 10% of animation
                if (attackState.time >= attackState.length * 0.1f){
		  			attacking = true;
	  			}
                // if the time of attack animation is running to marking point (PoseAttackTime[poseIndex]) 
                // calling CharacterAttack.cs to push a damage out
                if (attackState.time >= PoseAttackTime[poseIndex]){
	      			if(!diddamaged){
						// push a damage out
		 				this.gameObject.GetComponent<CharacterAttack>().DoDamage();
		 	 		}
				}
                // if the time of attack animation is running to 80% of animation. It's should be Finish this pose.
                if (attackState.time >= attackState.length * 0.8f){
					attackState.normalizedTime = attackState.length;
					diddamaged = true;
					attacking = false;
					attackStep += 1;
                    attackStack -= 1;

                    // checking if a calling attacking is stacked
                    if (attackStack >= 1){
						fightAnimation();
					}

					// reset character damage system
					this.gameObject.GetComponent<CharacterAttack>().StartDamage();
	  			}
			}
		}

        // Freeze when got hit
        if (hited){
			if(frozetime > 0){
				frozetime--;
			}else{
				hited = false;
				this.gameObject.GetComponent<Animation>().Play(PoseIdle);
			}
		}
		
		if(Time.time > attackStackTimeTemp + 2 && attackStack < 1){
			resetCombo();
		}
	}
	
	public void GotHit(float time){
		if(!IsHero){
			if(PoseHitNames.Length > 0){
				// play random Hit animation
				this.gameObject.GetComponent<Animation>().Play(PoseHitNames[Random.Range(0,PoseHitNames.Length)], PlayMode.StopAll);
			}
			frozetime = time * Time.deltaTime;// froze time when got hit
			hited = true;
		}
	}
	
	private void resetCombo(){
		attackStep = 0;
		attackStack = 0;
	}
	
	private void fightAnimation(){
		attacking = false;
		if(attackStep >= comboList.Length){
		  	resetCombo();
		}
		
		int poseIndex = int.Parse(comboList[attackStep]);
		if(poseIndex < PoseAttackNames.Length){// checking poseIndex is must in the PoseAttackNames list.
            if (this.gameObject.GetComponent<CharacterAttack>()){
                // Play Attack Animation 
                this.gameObject.GetComponent<Animation>().Play(PoseAttackNames[poseIndex], PlayMode.StopAll);
			}
    		diddamaged = false;
		}
	}
	
	public void Attack()
	{
		if(frozetime <= 0){
			attackStackTimeTemp = Time.time;
		    fightAnimation();
			attackStack += 1;
		}
	}
    
	public void Move(Vector3 dir){
		if(!attacking){
			moveDirection = dir;
		}else{
			moveDirection = dir/2f;
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
