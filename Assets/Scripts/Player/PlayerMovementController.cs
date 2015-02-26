using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Player/Movement Controller")]
public class PlayerMovementController : MonoBehaviour 
{

	public Player player;
	public PlayerTransformationController transformationController;
	public PlayerAnimationController animationController;

	public float speed = 20.0f;
    public float sprintSpeed = 1.5f;
	public float jumpSpeed = 13.0f;
	public float standingVerticalVelocity = 10;

	Vector3 movementInput = Vector3.zero;
	bool jumpInput = false;
    bool sprintInput = false;
    bool airborneOff = false;

	[HideInInspector]
	public Vector3 startPosition; // For use with constraining to a plane.
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
            Debug.Log("Grounded");
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
			    

			if (rigidbody.velocity.sqrMagnitude == 0)
				animationController.state = PlayerAnimationController.State.IDLE;
			else 
            {
                if (sprintInput)
                    animationController.state = PlayerAnimationController.State.RUNNING;
                else
                    animationController.state = PlayerAnimationController.State.WALKING;

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

		} 
        else 
        {
            Debug.Log("Not Grounded");
			rigidbody.velocity += transform.TransformDirection( Physics.gravity ) * dt;
			
			if (isGrounded)
				player.state = Player.State.WALKING;
		    else if(!airborneOff)
				animationController.state = PlayerAnimationController.State.AIRBORNE;
		}
		
		rigidbody.MovePosition(rigidbody.position + forcedMovement);
		forcedMovement = Vector3.zero;
	}

	public void Jump()
	{
		animationController.state = PlayerAnimationController.State.JUMPING;	
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
