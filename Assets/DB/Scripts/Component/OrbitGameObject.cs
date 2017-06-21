using UnityEngine;
using System.Collections;

public class OrbitGameObject : Orbit
{
	public GameObject Target;
	public Vector3 ArmOffset	= Vector3.zero;
	void Start()
	{
		Data.Zenith	= -0.3f;
		Data.Length	= -6;
	}

	protected override void Update()
	{
		Time.timeScale	+= (1 - Time.timeScale) / 10f;

		var lookAt	= ArmOffset;
		if(Target != null)
			lookAt	+= Target.transform.position;

		base.Update();
		gameObject.transform.position	+= lookAt;
		gameObject.transform.LookAt(lookAt);
	}
}
