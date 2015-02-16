﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Game/Camera/Camera Controller")]
public class CameraController : MonoBehaviour 
{

	public static CameraController instance;

	public float baseTimeToTarget = 1.0f;
	private float _timeToTarget = 1.0f;
	public float timeToTarget {
		get 
        {
			return _timeToTarget;
		}
		set 
        {
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
	public CameraFollowable target 
    {
		get 
        {
			return _target;
		}
		set 
        {
			if( _target == value ) 
				return;

			_target = value;
			startT = Time.time;
			startPos = transform.position;
			startRot = transform.rotation;
		}
	}

	public CameraFollowable highestPriorityTarget 
    {
		get 
        {
			CameraFollowable ret = null;
			foreach( CameraFollowable cur in followQueue )
            {
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

	void Awake() 
    {
		timeToTarget = baseTimeToTarget;
		target = startTarget;

		instance = this;
	}

	void FixedUpdate() 
    {
		if( target != null ) 
        {
			Transform trans = target.followTransform;
			float t = ( target.instant ? 1 : ( Time.time - startT ) * inverseTimeToTarget );

			Quaternion rot = ( target.offset == Vector3.zero ? trans.rotation : Quaternion.LookRotation( target.followTransform.position - transform.position ) );
            if (Player.instance != null)
            {
                //This is temporary until I get 3D cameras working
                Player player = GameObject.Find("Player").GetComponent<Player>();
                if (player.transformationController.in2D)
                {
                    transform.parent = Player.instance.transform;
                    transform.localPosition = new Vector3(0, 0, -30f);

                    var relativePos = Player.instance.transform.position - transform.position;
                    var rotation = Quaternion.LookRotation(relativePos);
                    transform.rotation = rotation;
                }
                else 
                {
                    //Remove the camera from the player object
                    transform.parent = Player.instance.transform.parent;

                    //Get the vector between the player and the camera
                    var relativePos = Player.instance.transform.position - transform.position;
                    //Rotate the camera accordingly
                    var rotation = Quaternion.LookRotation(relativePos);
                    transform.rotation = rotation;

                    //If the relative position of the camera and player, is greater than 5, move the camera away from the player
                    if (relativePos.magnitude > 5)
                    {
                        Vector3 playerPosition = new Vector3(Player.instance.transform.position.x, transform.position.y, Player.instance.transform.position.z);
                        transform.position = Vector3.MoveTowards(transform.position, playerPosition, 1.5f*Time.deltaTime);
                    }

                    //If the relative position of the camera and player, is less than 4.5, move the camera closer to the player
                    else if(relativePos.magnitude <= 4.5)
                    {
                        Vector3 playerPosition = new Vector3(Player.instance.transform.position.x, transform.position.y, Player.instance.transform.position.z);
                        transform.position = Vector3.MoveTowards(transform.position, playerPosition, -1.5f*Time.deltaTime);
                    }

                    /*Old Method
                    transform.parent = Player.instance.transform.parent;
                    transform.position = Vector3.Lerp(startPos, target.target, t);
                    transform.rotation = Quaternion.Lerp(startRot, rot, t);
                    */
                }
            }
            else
            {

                transform.position = Vector3.Lerp(startPos, target.target, t);
                transform.rotation = Quaternion.Lerp(startRot, rot, t);
            }
		}
	}

	public void AddToQueue( CameraFollowable followable, bool atEnd = false ) 
    {
		if( atEnd ) 
        {
			followQueue.AddLast( followable );
			if( followQueue.Count == 1 )
				target = followable;
			else if( followable.priority > target.priority )
				target = followable;
		} 
        else 
        {
			followQueue.AddFirst( followable );
			if( followQueue.Count == 1 || followable.priority >= target.priority )
				target = followable;
		}
	}

	public void RemoveFromQueue( CameraFollowable followable ) 
    {
		bool wasFirst = ( followable == target );
		followQueue.Remove( followable );

		if( wasFirst ) 
			target = highestPriorityTarget;
	}

	public void ClearQueue() 
    {
		followQueue.Clear();
		target = startTarget;
	}

	public void Reset() 
    {
		target = highestPriorityTarget;
	}

	public Vector3 moveRight 
    {
		get 
        {
			Vector3 ret = transform.right;
			ret.y = 0;
			return ret.normalized;
		}
	}

	public Vector3 moveForward 
    {
		get 
        {
			Vector3 ret = transform.forward;
			ret.y = 0;
			return ret.normalized;
		}
	}
}
