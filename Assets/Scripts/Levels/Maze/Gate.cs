using UnityEngine;
using System.Collections;

public class Gate : Resettable {

	public static Gate instance;

	public int gathered = 0;
	public int total = 5;

	void Awake() {
		instance = this;
	}

	public override void PerformReset ()
	{
		gathered = 0;
	}

	void Update() {
		bool active = ( gathered < total );

		renderer.enabled = active;
		collider.enabled = active;
	}
}
