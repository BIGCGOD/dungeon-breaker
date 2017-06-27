using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

class ActionManager : MonoBehaviour
{
    const string iconPath = "DB/Textures/UI/";
    const string prefabPath = "DB/Prefabs/";

    public Hashtable actionHash;

    void Start()
    {
        actionHash = new Hashtable();

        XmlDocument doc = new XmlDocument();
        doc.Load("Assets/Resources/DB/Data/ActionData.xml");    //加载Xml文件
        XmlElement actionData = doc.DocumentElement;    //获取根节点
        XmlNodeList actionNodes = actionData.GetElementsByTagName("Action");    //获取Person子节点集合
        foreach (XmlNode node in actionNodes)
        {
            BaseAction action = new BaseAction();
            string str;

            str = ((XmlElement)node).GetAttribute("name");
            if (str != "")
                action.name = str;

            str = ((XmlElement)node).GetAttribute("icon");
            if (str != "")
                action.icon = Resources.Load(iconPath + str, typeof(Texture2D)) as Texture2D;

            str = ((XmlElement)node).GetAttribute("prefab");
            if (str != "")
                action.prefab = Resources.Load(prefabPath + str, typeof(GameObject)) as GameObject;

            str = ((XmlElement)node).GetAttribute("animationName");
            if (str != "")
                action.animationName = str;

            str = ((XmlElement)node).GetAttribute("speed");
            if (str != "")
                action.speed = float.Parse(str);

            str = ((XmlElement)node).GetAttribute("prepareTime");
            if (str != "")
                action.prepareTime = float.Parse(str);
            str = ((XmlElement)node).GetAttribute("finishTime");
            if (str != "")
                action.finishTime = float.Parse(str);
            str = ((XmlElement)node).GetAttribute("isCurrent");
            if (str != "")
                action.isCurrent = bool.Parse(str);

            str = ((XmlElement)node).GetAttribute("reducedMoveSpeed");
            if (str != "")
            {
                string[] reducedMoveSpeed = str.Split(',');
                action.reducedMoveSpeed[0] = float.Parse(reducedMoveSpeed[0]);
                action.reducedMoveSpeed[1] = float.Parse(reducedMoveSpeed[1]);
                action.reducedMoveSpeed[2] = float.Parse(reducedMoveSpeed[2]);
            }

            int index = int.Parse(((XmlElement)node).GetAttribute("index"));
            actionHash.Add(index, action);
        }
    }
}
