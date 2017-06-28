using UnityEngine;
using System.Collections;

public class BaseAction
{
    public string name;
    public string animationName;
    public Texture2D icon;
    public GameObject prefab;

    public float speed;
    public float prepareTime;
    public float finishTime;
    public bool isCurrent;
    public float[] reducedMoveSpeed;
    public int manaCost;

    public Action doSomething;
    public Action doWhenStop;

    public BaseAction()
    {
        reducedMoveSpeed = new float[3];
    }
}
