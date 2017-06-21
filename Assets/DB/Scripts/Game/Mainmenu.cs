/// <summary>
/// Mainmenu. Just Main menu GUI
/// </summary>

using UnityEngine;
using System.Collections;

public class Mainmenu : MonoBehaviour {

	public Texture2D LogoGame;
	public GUISkin skin;
	void Start ()
	{
	}
	
	void OnGUI()
	{
		Screen.lockCursor = false;
		if(skin)
			GUI.skin = skin;

		GUI.DrawTexture(new Rect(Screen.width/2 - (LogoGame.width*0.5f)/2,Screen.height/2 - 200,LogoGame.width *0.5f,LogoGame.height*0.5f),LogoGame);	
		if(GUI.Button(new Rect(Screen.width/2 - 80,Screen.height/2 +20,160,30),"Start Demo"))
			Application.LoadLevel("Demo");

		if(GUI.Button(new Rect(Screen.width/2 - 80,Screen.height/2 +60,160,30),"Purchase"))
		{
		}

		GUI.skin.label.fontSize = 12;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(0,Screen.height - 50,Screen.width,30),"Dungeon Breaker Starter Kit beta.  By Rachan Neamprasert | www.hardworkerstudio.com");
	}

	void Update ()
	{
	}
}
