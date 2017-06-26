/// <summary>
/// Character status. is the Character Structure contain a basic Variables 
/// such as HP , SP , Defend , Name or etc... and you can adding more.
///  - this class will calculate all character Status. ex. Hp regeneration per sec
///  - Checking any event. ex. ApplyDamage , Dead , etc...
///  - After the Character is dead will replaced with Dead body or Ragdoll object [DeadbodyModel]
/// </summary>

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

// Update()                 更新生命恢复等数值变动
// ApplayDamage()           造成伤害，死亡判定，防御值计算，记录伤害方向
// AddParticle()            添加击中效果
// Dead()                   创建尸体，销毁对象
// CopyTransformsRecurse()  复制位置，包括身体部件，添加身体部件飞出的效果

public class CharacterStatus : MonoBehaviour
{
	public GameObject DeadbodyModel;
	public GameObject ParticleObject;

    //Current Status
    public string Name = "";
	public int HP = 10;
	public int SP = 10;
    public int SPmax = 10;
    public int HPmax = 10;
    public int Damage = 1;
    public int Defend = 1;
    public int HPregen = 1;
    public int SPregen = 1;

    //Equipment Status
    public int EquipmentSPmax = 10;
    public int EquipmentHPmax = 10;
    public int EquipmentDamage = 1;
    public int EquipmentDefend = 1;
    public int EquipmentHPregen = 1;
    public int EquipmentSPregen = 1;

    //Original Status
    public int OriginalSPmax = 10;
    public int OriginalHPmax = 10;
    public int OriginalDamage = 1;
    public int OriginalDefend = 1;
    public int OriginalHPregen = 1;
    public int OriginalSPregen = 1;

    public AudioClip[] SoundHit;
	
	private Vector3 velocityDamage;
	float lastRegen;

    //BUFF Delegate and Event
    public delegate int BuffDelegate(int num);

    public event BuffDelegate SPmaxBuffEvent;
    public event BuffDelegate HPmaxBuffEvent;
    public event BuffDelegate DamageBuffEvent;
    public event BuffDelegate DefendBuffEvent;
    public event BuffDelegate HPregenBuffEvent;
    public event BuffDelegate SPregenBuffEvent;
    
    int CountBuffNum(BuffDelegate buffChange, int num)
    {
        if (buffChange != null)
        {
            System.Delegate[] delArray = buffChange.GetInvocationList();
            foreach (System.Delegate del in delArray)
            {
                BuffDelegate method = (BuffDelegate)del;
                num = method(num);
            }
        }
        return num;
    }

    void Start()
    {
        gameObject.AddComponent<BaseBuff>();
    }

    void Update()
    {
        updateBuffData();

        if (Time.time - lastRegen >= 1)
		{
			lastRegen = Time.time;
			HP += HPregen;
			SP += SPregen;
		}

		if(HP > HPmax)
			HP = HPmax;

		if(SP > SPmax)
			SP = SPmax;
	}

    public void updateBuffData()
    {
        SPmax = CountBuffNum(SPmaxBuffEvent, EquipmentSPmax);
        HPmax = CountBuffNum(HPmaxBuffEvent, EquipmentHPmax);
        Damage = CountBuffNum(DamageBuffEvent, EquipmentDamage);
        Defend = CountBuffNum(DefendBuffEvent, EquipmentDefend);
        HPregen = CountBuffNum(HPregenBuffEvent, EquipmentHPregen);
        SPregen = CountBuffNum(SPregenBuffEvent, EquipmentSPregen);
    }

    public int ApplayDamage(int damage,Vector3 dirdamge)
	{
        // Applay Damage function
        if (HP < 0) {
			return 0;
		}
		if(SoundHit.Length > 0){
			int randomindex = Random.Range(0, SoundHit.Length);
			if(SoundHit[randomindex] != null){
				AudioSource.PlayClipAtPoint(SoundHit[randomindex], this.transform.position);
			}
		}
		if(this.gameObject.GetComponent<CharacterSystem>()){
			this.gameObject.GetComponent<CharacterSystem>().GotHit(1);
		}
		var damval = damage - Defend;
		if(damval < 1){
			damval = 1;
		}
		HP -= damval;
		velocityDamage = dirdamge;
		if(HP <= 0){
			Dead();
		}
		return damval;
	}
	
	public void AddParticle(Vector3 pos){
		if(ParticleObject){
			var bloodeffect = (GameObject)Instantiate(ParticleObject, pos, transform.rotation);
			GameObject.Destroy(bloodeffect, 1);
		}
	}
	
	void Dead()
	{
		if(DeadbodyModel)
		{
			var deadbody = (GameObject)Instantiate(DeadbodyModel, this.gameObject.transform.position, this.gameObject.transform.rotation);
			CopyTransformsRecurse(this.gameObject.transform, deadbody.transform);
			Destroy(deadbody, 10.0f);
		}
		Destroy(gameObject);
	}
	
	public void CopyTransformsRecurse (Transform src,Transform dst)
	{
		// Copy all transforms to Dead object replacement (Ragdoll)
		dst.position = src.position;
		dst.rotation = src.rotation;
		dst.localScale = src.localScale;
		foreach(var child in dst.Cast<Transform>())
		{
			var curSrc = src.Find(child.name);
			if(child.GetComponent<Rigidbody>())
				child.GetComponent<Rigidbody>().AddForce(velocityDamage / 3f);

			if(curSrc)
				CopyTransformsRecurse(curSrc, child);
		}
	}
}

