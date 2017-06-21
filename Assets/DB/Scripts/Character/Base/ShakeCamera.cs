/// <summary>
/// Shake camera.
/// Shaking Camera System using adding to Camera with <OrbitGameObject>
/// </summary>

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(OrbitGameObject))]
public class ShakeCamera : MonoBehaviour
{
	public static ShakeCamera Shake(float magnitude,float duration)
	{
		var shake	= Camera.main.gameObject.AddComponent<ShakeCamera>();
		shake.Magnitude	= magnitude;
		shake.Duration	= duration;
		return shake;
	}

	public float Magnitude	= 1;
	public float Duration	= 1;
	// Update is called once per frame
	void Update()
	{
		Duration	-= Time.deltaTime;
		if(Duration < 0)
			Destroy(this);
		
		var orbit	= gameObject.GetComponent<OrbitGameObject>();
		//Shaking Camera
		orbit.ArmOffset.y	= Mathf.Sin(1000 * Time.time) * Duration * Magnitude;
	}
}
