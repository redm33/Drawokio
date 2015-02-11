using UnityEngine;
using System.Collections;

public class PatrolTrigger : MonoBehaviour {

	public Patrol patrol;

	void OnTriggerEnter( Collider other ) {
		patrol.running = true;
	}
}
