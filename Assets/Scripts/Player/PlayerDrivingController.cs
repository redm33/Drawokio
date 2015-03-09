using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Player/Movement Controller")]
public class PlayerDrivingController : MonoBehaviour
{

    public Player player;
    public PlayerTransformationController transformationController;
    public PlayerAnimationController animationController;

    public float speed = 40.0f;
    public float jumpSpeed = 2f;
    public float standingVerticalVelocity = 10;

    Vector3 movementInput = Vector3.zero;
    bool jumpInput = false;

    [HideInInspector]
    public Vector3 spawnPosition;

    private bool _isActive = false;
    public bool isActive
    {
        get { return _isActive; }
        set
        {
            if (_isActive == value)
                return;

            _isActive = value;

            rigidbody.isKinematic = !isActive;
        }
    }

    void Update()
    {
        movementInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        jumpInput = Input.GetButton("Jump");
    }

    void FixedUpdate()
    {
        if (isActive && !player.paused)
            Movement(Time.fixedDeltaTime);
    }

    void Awake()
    {
        isActive = true;
    }

    public Vector3 forcedMovement = Vector3.zero;
    void Movement(float dt)
    {
        spawnPosition = this.transform.position;
        if (transformationController.in3D)
        {
            CameraController camera = CameraController.instance;
            rigidbody.velocity = camera.moveRight * movementInput.x + camera.moveForward * movementInput.y;
        }
      
        rigidbody.velocity *= speed;


        animationController.state = PlayerAnimationController.State.IDLE;

        if (rigidbody.velocity.sqrMagnitude > 0)
            transform.rotation = Quaternion.LookRotation(rigidbody.velocity);

        rigidbody.velocity -= transform.up * standingVerticalVelocity;
        //rigidbody.velocity += transform.TransformDirection(Physics.gravity) * dt;


        rigidbody.MovePosition(rigidbody.position + forcedMovement);
        forcedMovement = Vector3.zero;

        if (jumpInput)
            Jump();
    }

    public void Jump()
    {
        animationController.state = PlayerAnimationController.State.JUMPING;
        rigidbody.velocity += transform.up * jumpSpeed;
    }

    /**
     * Grounding
     */





    

}
