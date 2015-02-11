using UnityEngine;
using System.Collections;

public class Pushable : Movable {

	public Vector3 velocity;

	void FixedUpdate() {
		if( pushing && !paused )
			rigidbody.MovePosition( transform.position + velocity * Time.fixedDeltaTime );
	}

	public bool pushing = false;
	void OnTriggerStay( Collider other ) {
		pushing = true;
	}

	void OnTriggerExit( Collider other ) {
		pushing = false;
	}
}
