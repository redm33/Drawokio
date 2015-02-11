using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Game/Room/Resettables/Movable")]
public class Movable : Resettable {
	Vector3 startPos;
	Quaternion startRot;

	public bool alwaysKinematic = false;

	void Awake() {
		startPos = transform.position;
		startRot = transform.rotation;
	}

	public override void PerformReset() {
		if( !rigidbody.isKinematic ) {
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}

		transform.position = startPos;
		transform.rotation = startRot;
	}

	Vector3 pauseVel;
	Vector3 pauseRotVel;

	public override bool paused {
		get {
			return base.paused;
		}
		set {
			if( paused == value )
				return;

			if( !paused ) {
				pauseVel = rigidbody.velocity;
				pauseRotVel = rigidbody.angularVelocity;
			}

			base.paused = value;

			rigidbody.isKinematic = value || alwaysKinematic;

			if( !paused && !alwaysKinematic ) {
				rigidbody.velocity = pauseVel;
				rigidbody.angularVelocity = pauseRotVel;
			}
		}
	}
}
