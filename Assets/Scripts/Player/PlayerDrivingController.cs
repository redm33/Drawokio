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
        jumpInput = Input.GetButtonDown("Jump");

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

    //public Vector3 forcedMovement = Vector3.zero;
    void Movement(float dt)
    {
        spawnPosition = this.transform.position;
        animationController.state = PlayerAnimationController.State.IDLE;

        if (jumpInput)
            Jump();
    }

    public void Jump()
    {
        Player.instance.transform.parent = Room.instance.transform.parent;
        Player.instance.rigidbody.isKinematic = false;
        Player.instance.GetComponent<PlayerMovementController>().isGrounded = false;
        animationController.state = PlayerAnimationController.State.JUMPING;

        rigidbody.velocity += transform.up * jumpSpeed;

        jumpInput = false;

        Player.instance.transform.parent = GameObject.Find("CarChild").transform;
        Player.instance.rigidbody.isKinematic = false;

        Player.instance.GetComponent<PlayerMovementController>().enabled = true;
        Player.instance.GetComponent<PlayerMovementController>().isGrounded = false;

        GameObject.Find("CarChild").GetComponent<PlayerCar_Script>().enabled = false;

        Player.instance.GetComponent<PlayerDrivingController>().enabled = false;
        GameObject.Find("CarChild").rigidbody.isKinematic = true;

        Player.movement = 'M';
    }

    /**
     * Grounding
     */





    

}
