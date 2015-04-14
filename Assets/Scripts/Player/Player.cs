using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Game/Player/Player")]
//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player: MonoBehaviour
{
	//Items such as the minigun or the fire-pen

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
    public static char movement = 'M';

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

		//Initialize the items list
		carriedItems = new List<Transform>();
		equippedItem = -1;
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

    /*
    public void Kill()
    {
        Instantiate(deathPoof, transform.position, transform.rotation);

        if(rigidbody == null)
        {
            Player.instance.gameObject.AddComponent<Rigidbody>();
            Player.instance.GetComponent<CapsuleCollider>().enabled = true;
            Player.instance.rigidbody.useGravity = false;
            Player.instance.rigidbody.isKinematic = false;
            Player.instance.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            Player.instance.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            Player.instance.rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        GameObject.Find("Main Camera").GetComponent<ThirdPersonCamera>().enabled = false;
        GameObject.Find("Main Camera").transform.parent = Player.instance.transform.parent;
        StartCoroutine(DelayRespawn());
        //Destroy(gameObject);
    }
    */
    public void Kill()
    {
        GameObject.Find("Model Parent").SetActive(false);
        Destroy((ParticleSystem)this.transform.Find("DissolveParticles").gameObject.particleSystem);
        transformationController.projectorShadow.SetActive(false);

        Instantiate(deathPoof, transform.position, transform.rotation);
        StartCoroutine(DelayRespawn());

        GameObject.Find("Main Camera").GetComponent<ThirdPersonCamera>().enabled = false;
        GameObject.Find("Main Camera").transform.parent = Player.instance.transform.parent;
    }

    IEnumerator DelayRespawn()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        Room.instance.SpawnAtLatestSpawn();
        //Player player = Instantiate(Room.instance.playerPrefab, spawnPosition, rot) as Player;
        //player.name = "Player";
        //Player.instance.transformationController.Become3D();
       // ParticleSystem temp = (ParticleSystem)player.transform.Find("DissolveParticles").gameObject.particleSystem;
        //temp.Play();
       // player.transform.Find("Blob Shadow Projector").gameObject.SetActive(true);

    }
    public void StartTransport(Transform transport)
    {
        this.transport = transport;
        state = State.TRANSPORTED;
    }

	void Start() {

	}

	void Update()
	{
        if (!paused && Input.GetButtonDown("Back"))
        {
            Room.instance.state = Room.State.MENU_MAIN;
            Player.instance.GetComponent<PlayerDrivingController>().Jump();

        }

        playerPosition = this.transform.localPosition;
        if (rigidbody != null)
        {
            /**if (Input.GetMouseButton(0) && this.transformationController.in3D && !this.rigidbody.isKinematic)
            {
                this.rigidbody.velocity = Vector3.zero;
                this.animationController.state = PlayerAnimationController.State.IDLE;
                this.GetComponent<PlayerMovementController>().enabled = false;
                this.GetComponent<PlayerDrivingController>().enabled = false;
                GameObject.Find("CarChild").GetComponent<PlayerCar_Script>().enabled = false;
            }**/
			if(Input.GetMouseButton(0) && this.transformationController.in3D && !this.rigidbody.isKinematic && this.equippedItem != -1) {

			}
            else if (Player.movement == 'M')
                this.GetComponent<PlayerMovementController>().enabled = true;
            else if (Player.movement == 'D')
            {
                GameObject.Find("CarChild").GetComponent<PlayerCar_Script>().enabled = true;
                this.GetComponent<PlayerDrivingController>().enabled = true;
            }
        }

	}



}