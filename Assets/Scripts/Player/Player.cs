using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Game/Player/Player")]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player: MonoBehaviour
{
	public static Player instance;
    public static int layer3D = 9, layer2D = 8;
    public static bool IsPlayerLayer(int layer)
    {
        return (layer == layer3D || layer == layer2D);
    }

	public PlayerAnimationController animationController;
	public PlayerMovementController movementController;
	public PlayerTransformationController transformationController;
	public PlayerClimbingController climbingController;

	public ParticleSystem deathPoof;
    public static Vector3 playerPosition;

    public enum State
    {
        WALKING,
        CLIMBING,
        TRANSFORMING,
		TRANSPORTED,
		NONE
    }
	private State _state = State.NONE;
	public State state 
    {
		get { return _state; }
		set 
        {
			if( _state == value )
				return;

			switch( _state ) 
            {
			case State.WALKING:
				WalkingLeave();
				break;
			case State.CLIMBING:
				ClimbingLeave();
				break;
			case State.TRANSFORMING:
				TransformingLeave();
				break;
			case State.TRANSPORTED:
				TransportedLeave();
				break;
			}

			_state = value;

			switch( _state ) 
            {
			case State.WALKING:
				WalkingEnter();
				break;
			case State.CLIMBING:
				ClimbingEnter();
				break;
			case State.TRANSFORMING:
				TransformingEnter();
				break;
			case State.TRANSPORTED:
				TransportedEnter();
				break;
			}

		}
	}
	void UpdateState() 
    {
		switch( _state )
        {
		case State.WALKING:
			WalkingUpdate();
			break;
		case State.CLIMBING:
			ClimbingUpdate();
			break;
		case State.TRANSFORMING:
			TransformingUpdate();
			break;
		case State.TRANSPORTED:
			TransportedUpdate();
			break;
		}
	}

	void WalkingEnter() 
    {
		movementController.isActive = true;
	}

	void WalkingUpdate() {}

	void WalkingLeave() 
    {
		movementController.isActive = false;
	}

	void ClimbingEnter() 
    {
		rigidbody.isKinematic = false;
	}

	void ClimbingUpdate() {}

	void ClimbingLeave() 
    {
		rigidbody.isKinematic = true;
	}

	void TransformingEnter() 
    {
		animationController.state = PlayerAnimationController.State.JUMPING;
	}
	
	void TransformingUpdate() {}

	void TransformingLeave() 
    {
		movementController.startPosition = transform.position;
	}

	Transform transport;
	void TransportedEnter() 
    {
		transform.position = transport.position;
		transform.rotation = transport.rotation;
		animationController.state = PlayerAnimationController.State.T;
	}

	void LateUpdate() 
    {
		if( state == State.TRANSPORTED ) 
			transform.position = transport.position;
	}

	void TransportedUpdate() {}

	void TransportedLeave() 
    {
		transform.rotation = Quaternion.identity;
	}

    private bool _paused = false;
    public bool paused
    {
        get { return _paused; }
        set
        {
			if( _paused == value )
				return;

            _paused = value;

            if (paused)
				BeginPause();
            else
				EndPause();
        }
    }

	void BeginPause() 
    {
		pauseVelocity = rigidbody.velocity;

		rigidbody.isKinematic = true;
		
		animationController.Pause();
	}
	
	void EndPause() 
    {
		rigidbody.isKinematic = false;
		rigidbody.velocity = pauseVelocity;
		
		animationController.Start();
	}
	
	Vector3 pauseVelocity = Vector3.zero;

    /**
     * Generic
     */

    void Awake()
    {
		instance = this;
		state = State.WALKING;
    }

	
	public GameObject[] hats;
	int currentHat = -1;
	
	public void SetHat( int index ) 
    {
		if( currentHat >= 0 )
			hats[currentHat].SetActive(false);
		
		currentHat = index;
		
		if( index >= 0 && index < hats.Length ) 
			hats[index].SetActive(true);
	}

	void Update()
	{
		if( !paused && Input.GetButtonDown("Back") ) 
			Room.instance.state = Room.State.MENU_MAIN;

        playerPosition = this.transform.localPosition;

        if (Input.GetMouseButton(0) && this.transformationController.in3D)
        {
            this.rigidbody.velocity = Vector3.zero;
            this.animationController.state = PlayerAnimationController.State.IDLE;
            this.GetComponent<PlayerMovementController>().enabled = false;
        }
        else
            this.GetComponent<PlayerMovementController>().enabled = true;

	}

	void FixedUpdate(){}

	public void Kill()
	{
		Instantiate( deathPoof, transform.position, transform.rotation );
		Destroy ( gameObject );
	}

	public void StartTransport( Transform transport ) 
    {
		this.transport = transport;
		state = State.TRANSPORTED;
	}
}