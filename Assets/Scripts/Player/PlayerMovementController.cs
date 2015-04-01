using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Player/Movement Controller")]
public class PlayerMovementController : MonoBehaviour 
{

	public Player player;
	public PlayerTransformationController transformationController;
	public PlayerAnimationController animationController;

	public AudioSource walkSoundStart;
	public AudioSource walkSoundLooping;
	private bool playedStart = false;

	public float speed = 20.0f;
    public float sprintSpeed = 1.5f;
	public float jumpSpeed = 13.0f;
	public float standingVerticalVelocity = 10;
    public float fallLimit = 2f;
    public float slideForce = 0;

	bool jumpInput = false;
    bool sprintInput = false;
    bool airborneOff = false;
    bool canMoveUp = true;

    float maxWalkSloap = 45;

	[HideInInspector]
    public Vector3 movementInput = Vector3.zero;
	public Vector3 startPosition; // For use with constraining to a plane.
    public Vector3 spawnPosition;
    public string twoDAxis = "Z";

	private bool _isActive = false;
	public bool isActive {
		get { return _isActive; }
		set 
        {
			if( _isActive == value )
				return;

			_isActive = value;

			rigidbody.isKinematic = !isActive;
		}
	}

	void Awake() 
    {
		isGrounded = false;
		startPosition = transform.position;
	}

	void Update() 
    {
        if (transformationController.in2D && ((Player.instance.transform.eulerAngles.y >= -1 && Player.instance.transform.eulerAngles.y <= 1) || (Player.instance.transform.eulerAngles.y >= 179 && Player.instance.transform.eulerAngles.y <= 181)))
        {
            movementInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0);
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

        }
        else if (transformationController.in2D)
        {
            movementInput = new Vector3(0, Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
        }
        else
        {
            movementInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        }
		jumpInput = Input.GetButton("Jump");
        sprintInput = Input.GetKey(KeyCode.LeftShift);
	}

	void FixedUpdate() 
    {
		if( isActive && !player.paused ) 
			Movement( Time.fixedDeltaTime );
	}

	public void ApplyLockType( DrawingCanvas.LockType lockType ) 
    {
		rigidbody.constraints = DrawingCanvas.GetConstraintsForLockType( lockType ) | RigidbodyConstraints.FreezeRotation;
	}

	public Vector3 forcedMovement = Vector3.zero;
	void Movement(float dt)
	{		
		CheckGrounded();
	
		if( isGrounded ) 
        {
            
			if( transformationController.in3D ) 
            {
				CameraController camera = CameraController.instance;
				rigidbody.velocity = camera.moveRight * movementInput.x + camera.moveForward * movementInput.y;
			}
            else if (movementInput.x == 0)
                rigidbody.velocity = transform.right * movementInput.z;
            else
                rigidbody.velocity = transform.right * movementInput.x;


            if (sprintInput)
                rigidbody.velocity *= sprintSpeed;
            else
                rigidbody.velocity *= speed;
			    

			if (rigidbody.velocity.sqrMagnitude == 0){
				animationController.state = PlayerAnimationController.State.IDLE;
				walkSoundStart.Stop();
				walkSoundLooping.Stop();
				playedStart = false;
			}
			else 
            {
                if (sprintInput){
                    animationController.state = PlayerAnimationController.State.RUNNING;
					if(!walkSoundStart.isPlaying && !walkSoundLooping.isPlaying && !playedStart) {
						walkSoundStart.Play();
						walkSoundStart.pitch = 0.5f;
						playedStart = true;
					} else if(!walkSoundStart.isPlaying && !walkSoundLooping.isPlaying) {
						walkSoundLooping.Play();
						walkSoundLooping.pitch = 0.5f;
					}
				}
                else {
                    animationController.state = PlayerAnimationController.State.WALKING;
					if(!walkSoundStart.isPlaying && !walkSoundLooping.isPlaying && !playedStart) {
						walkSoundStart.Play();
						walkSoundStart.pitch = 0.45f;
						playedStart = true;
					} else if(!walkSoundStart.isPlaying && !walkSoundLooping.isPlaying) {
						walkSoundLooping.Play();
						walkSoundLooping.pitch = 0.45f;
					}
				}
				if (transformationController.in3D)
				{
					if( rigidbody.velocity.sqrMagnitude > 0 )
						transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
				}
				else
				{
					if (movementInput.x > 0 || movementInput.z > 0)
						animationController.direction = PlayerAnimationController.Direction.RIGHT;
                    else if (movementInput.x < 0 || movementInput.z < 0)
						animationController.direction = PlayerAnimationController.Direction.LEFT;
				}
			}

			if( jumpInput ) 
				Jump ();
			else
				rigidbody.velocity -=  transform.up * standingVerticalVelocity;

            if (fallLimit > 0)
            {
                spawnPosition = this.transform.position;
                ResetFallTimer();
            }


            if (!canMoveUp)
            {
                rigidbody.AddForce(Vector3.down * slideForce * Time.deltaTime);
                Player.instance.animationController.state = PlayerAnimationController.State.T;
            }
                

		} 
        else
        {
            CameraController camera = CameraController.instance;
			rigidbody.velocity += transform.TransformDirection( Physics.gravity ) * dt * rigidbody.mass + camera.moveRight * movementInput.x * dt + camera.moveForward * movementInput.y * dt;
            StartFallTimer();

            if (!canMoveUp)
            {
                rigidbody.AddForce(Vector3.down * slideForce * Time.deltaTime);
                Player.instance.animationController.state = PlayerAnimationController.State.T;
            }
            else if (isGrounded)
                player.state = Player.State.WALKING;
            else if (!airborneOff) {
                animationController.state = PlayerAnimationController.State.AIRBORNE;
				walkSoundStart.Stop();
				walkSoundLooping.Stop();
				playedStart = false;
			}

		}
		
		rigidbody.MovePosition(rigidbody.position + forcedMovement);
		forcedMovement = Vector3.zero;
	}

	public void Jump()
	{
		animationController.state = PlayerAnimationController.State.JUMPING;
		walkSoundStart.Stop();
		walkSoundLooping.Stop();
		playedStart = false;
		rigidbody.velocity += transform.up * jumpSpeed;
	}
	
	/**
     * Grounding
     */
	
	public Transform groundCastOrigin;
	public float groundCastRadius = 1;
	public float groundCastDistance = 1;
	
	public int[] groundLayers;
	public int groundLayerMask
	{
		get
		{
			if (groundLayers == null)
				return 0;
			
			int ret = 0;
			foreach (int layer in groundLayers)
				ret += (1 << layer);

			if( transformationController.in2D )
				ret += (1 << Bounds.layer2D);
			if( transformationController.in3D )
				ret += (1 << Bounds.layer3D);

			return ret;
		}
	}
	
	public bool isGrounded;
	
	void CheckGrounded()
	{
		RaycastHit hit;
		isGrounded = (Physics.SphereCast(groundCastOrigin.position, groundCastRadius, Vector3.down, out hit, groundCastDistance, groundLayerMask));
	}

    void StartFallTimer()
    {
        if (transformationController.in3D)
        {
            fallLimit -= Time.deltaTime;
            //Debug.Log(fallLimit);
        }
        /*if (fallLimit <= 0)
        {
            Quaternion rot = this.transform.rotation;
            Player.instance.Kill();
            ResetFallTimer();
            Player player = Instantiate(Room.instance.playerPrefab, spawnPosition, rot) as Player;
            player.name = "Player";
            Player.instance.transformationController.Become3D();
		
        }*/
    }

    void OnCollisionEnter(Collision col)
    {
        if(fallLimit <= 0)
        {
            Quaternion rot = this.transform.rotation;
            Player.instance.Kill();
            ResetFallTimer();
            /*
            Player player = Instantiate(Room.instance.playerPrefab, spawnPosition, rot) as Player;
            player.name = "Player";
            Player.instance.transformationController.Become3D();
			ParticleSystem temp = (ParticleSystem)player.transform.Find ("DissolveParticles").gameObject.particleSystem;
			temp.Play ();
			player.transform.Find ("Blob Shadow Projector").gameObject.SetActive (true);
             * */
        }
    }

    void OnCollisionStay (Collision collision)
    {
        /**
        //Recieve the info on the current collision
        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.Log(Vector3.Angle(contact.normal, Vector3.up));
            //If the "constactpoints" angle is lower than maxWalkSloap
            if(Vector3.Angle(contact.normal, Vector3.up) < maxWalkSloap)
             //Don't do anything
             canMoveUp = true;
        }
        foreach (ContactPoint contact in collision.contacts)
        {
            if(Vector3.Angle(contact.normal, Vector3.up) > maxWalkSloap)
             //Else slide the player down
             canMoveUp = false;
        }**/
 }
 

    void ResetFallTimer()
    {
        fallLimit = 2f;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "BalloonBase")
            airborneOff = true;
    }
    void OnTriggerExit(Collider col)
    {
        if (col.name == "BalloonBase")
            airborneOff = false;
    }


  
}
