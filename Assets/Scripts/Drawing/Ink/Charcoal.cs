using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Game/Ink/Charcoal")]
public class Charcoal : Ink {

	public static int layer = 13;

	public override Type type {
		get {
			return Type.CHARCOAL;
		}
	}

	//public float riseForce = 1.0f;
	public float riseSpeed = 1.0f;
	public float riseAccel = 1.0f;
	public float riseForce = 1.0f;
	bool rising = false;

	public override bool ConnectTo (Connector connector)
	{
		if( connector.type == Type.DEFAULT )
			return true;
		return false;
	}

	public override bool paused {
		get {
			return base.paused;
		}
		set {
			_paused = value;

			if( value ) {
				pauseVelocity = rigidbody.velocity;
				pauseRotVelocity = rigidbody.angularVelocity;
			}

			rigidbody.isKinematic = paused;

			if( !value ) {
				rigidbody.velocity = pauseVelocity;
				rigidbody.angularVelocity = pauseRotVelocity;
			}

			rising = !paused;

			if( value )
				particleSystem.Pause();
			else {
				particleSystem.Play();
				renderer.enabled = false;
			}
		}
	}
	
	public override void AddChild ( Ink child )
	{
		child.transform.parent = Drawer.instance.drawingParent;
		children.Add( child );
	}

	public override void OnCreated ()
	{
		base.OnCreated ();

		PrepRigidbody();
	}
	
	public override void OnDrawEnd() {
		//renderer.enabled = false;
		foreach( Ink child in children )
			child.OnDrawEnd();
	}

	protected override void OnFixedUpdate() {
		if( rising ) {
			//rigidbody.AddForce( Vector3.up * riseForce );

			Vector3 vel = rigidbody.velocity;
			vel.y = Mathf.Min( riseSpeed, vel.y + riseAccel * Time.fixedDeltaTime );
			rigidbody.velocity = vel;
		}

	}

	public int upperBoundsLayer = 9;
	void OnCollisionEnter( Collision hit ) {
		if( rising ) {
			int layer = hit.gameObject.layer;

			if( layer == upperBoundsLayer )
				Destroy ( gameObject );
			else if( layer == gameObject.layer ) {
				/*if( hit.relativeVelocity.y > 0 ) {
					Vector3 myVel = rigidbody.velocity, theirVel = hit.rigidbody.velocity;
					myVel.y = theirVel.y = Mathf.Max( myVel.y, theirVel.y );
					rigidbody.velocity = myVel;
					hit.rigidbody.velocity = theirVel;
				}*/
			}
		}
	}
}
