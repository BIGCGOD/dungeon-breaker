/// <summary>
/// Adding this file into Player Character will Calling 'endgame' 
/// to GameEvent() in GameManager.cs when Player is dead
/// </summary>


using UnityEngine;
using System.Collections;

public class HeroDie : MonoBehaviour
{

	void OnDestroy()
	{
		var gameManager	= (GameManager)FindObjectOfType(typeof(GameManager));
		if(gameManager)
			gameManager.GameEvent("endgame");
	}
}
