using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Game/Camera/Camera Controller")]
public class CameraController : MonoBehaviour {

	public static CameraController instance;

	public float baseTimeToTarget = 1.0f;
	private float _timeToTarget = 1.0f;
	public float timeToTarget {
		get {
			return _timeToTarget;
		}
		set {
			if( value <= 0 )
			{
				Debug.LogWarning( "Cannot have timeToTarget value of " + value );
				return;
			}

			_timeToTarget = value;
			inverseTimeToTarget = 1.0f / _timeToTarget;
		}
	}
	public float inverseTimeToTarget { get; private set; }

	private LinkedList<CameraFollowable> followQueue = new LinkedList<CameraFollowable>();

	public CameraFollowable startTarget = null;
	private CameraFollowable _target = null;
	public CameraFollowable target {
		get {
			return _target;
		}
		set {
			if( _target == value ) {
				return;
			}

			_target = value;
			startT = Time.time;
			startPos = transform.position;
			startRot = transform.rotation;
		}
	}

	public CameraFollowable highestPriorityTarget {
		get {
			CameraFollowable ret = null;
			foreach( CameraFollowable cur in followQueue ) {
				if( ret == null || cur.priority > ret.priority )
					ret = cur;
			}
			if( ret == null )
				ret = startTarget;
			return ret;
		}
	}

	float startT = 0;
	Vector3 startPos = Vector3.zero;
	Quaternion startRot = Quaternion.identity;

	void Awake() {
		timeToTarget = baseTimeToTarget;
		target = startTarget;

		instance = this;
	}

	void FixedUpdate() {
		if( target != null ) {
			Transform trans = target.followTransform;
			float t = ( target.instant ? 1 : ( Time.time - startT ) * inverseTimeToTarget );

			Quaternion rot = ( target.offset == Vector3.zero ? trans.rotation : Quaternion.LookRotation( target.followTransform.position - transform.position ) );
		
			transform.position = Vector3.Lerp( startPos, target.target, t );
			transform.rotation = Quaternion.Lerp( startRot, rot, t );
		}
	}

	public void AddToQueue( CameraFollowable followable, bool atEnd = false ) {
		if( atEnd ) {
			followQueue.AddLast( followable );
			if( followQueue.Count == 1 )
				target = followable;
			else if( followable.priority > target.priority )
				target = followable;
		} else {
			followQueue.AddFirst( followable );
			if( followQueue.Count == 1 || followable.priority >= target.priority )
				target = followable;
		}
	}

	public void RemoveFromQueue( CameraFollowable followable ) {
		bool wasFirst = ( followable == target );
		followQueue.Remove( followable );

		if( wasFirst ) {
			target = highestPriorityTarget;
		}
	}

	public void ClearQueue() {
		followQueue.Clear();
		target = startTarget;
	}

	public void Reset() {
		target = highestPriorityTarget;
	}

	public Vector3 moveRight {
		get {
			Vector3 ret = transform.right;
			ret.y = 0;
			return ret.normalized;
		}
	}

	public Vector3 moveForward {
		get {
			Vector3 ret = transform.forward;
			ret.y = 0;
			return ret.normalized;
		}
	}
}
