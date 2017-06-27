using UnityEngine;
using System.Collections;

public class BaseBuff : MonoBehaviour
{
    public float time = 0;
    public float durationTime = 10;
    public GameObject comeFrom;

    protected CharacterStatus status;

    void Start()
    {
        status = GetComponent<CharacterStatus>();
        addBuff();
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time > durationTime)
            removeBuff();
    }

    int addAttack(int num)
    {
        return num * 100;
    }

    void addBuff()
    {
        /* 如何在这里使用匿名函数，使其可以被移除
         * 使用变量来命名匿名函数
         * CharacterStatus.BuffDelegate a = (int x) => { return x * 10; }; */
        status.DamageBuffEvent += addAttack;
    }

    void removeBuff()
    {
        status.DamageBuffEvent -= addAttack;
        Destroy(this);
    }
}
