using UnityEngine;
using System.Collections;

public class Plane : Patrol 
{

	public CameraFollowable cameraFollowable;
	public Transform playerPosition;
    public GameObject camera;

    //private variables
    private Vector3 cameraPos;
    private Vector3 cameraRot;

    //Wobble
    Vector3 previousAngle;
    Vector3 angleDelta = new Vector3(0, 0, 40);
    Vector3 changing_plane;
    Vector3 changing_player;
    float accelerationBuffer = 0.3f;
    float decelerationBuffer = 0.1f;
    float wobbleTime = .3f;
    bool switchWobbleDir = true;

    public bool fly = false;
    public bool running_on = false;

    void Start()
    {
        camera = GameObject.Find("Main Camera");
        cameraPos = new Vector3(-2.1f, -10.4f, -10.4f);
        cameraRot = new Vector3(310.131f, 10, 10);
        previousAngle = this.transform.eulerAngles;
        changing_plane = this.transform.eulerAngles;

    }
    void Update()
    {
        if (running)
        {
            wobbleCounter();

            transform.position = Vector3.MoveTowards(this.transform.position, patrolPoints[cur].position, speed * Time.deltaTime);
            camera.GetComponent<ThirdPersonCamera>().enabled = false;
            camera.transform.parent = Player.instance.transform;

            camera.transform.localPosition = cameraPos;
            camera.transform.localEulerAngles = cameraRot;

            //Player.instance.transform.parent = this.transform;

            //If boat pointing to sky - reduce X
            if (switchWobbleDir)
            {
                changing_plane = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - angleDelta.z * Time.deltaTime);
                //changing_player = new Vector3(Player.instance.transform.eulerAngles.x, Player.instance.transform.eulerAngles.y - angleDelta.z * Time.deltaTime, Player.instance.transform.eulerAngles.z);

                //transform.rotation.x -= angleDelta.x * Time.deltaTime;


                //If boat diving into ocean - increase X
            }
            else
            {
                changing_plane = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + angleDelta.z * Time.deltaTime);
                //changing_player = new Vector3(Player.instance.transform.eulerAngles.x, Player.instance.transform.eulerAngles.y + angleDelta.z * Time.deltaTime, Player.instance.transform.eulerAngles.z);

                //transform.eulerAngles.x += angleDelta.x * Time.deltaTime; //Must not forget to use deltaTime!
            }

            transform.eulerAngles = changing_plane;
            //Debug.Log(Player.instance.transform.eulerAngles);
            //Player.instance.transform.eulerAngles = new Vector3(Player.instance.transform.eulerAngles.x, Player.instance.transform.eulerAngles.y, Player.instance.transform.eulerAngles.z);
            //record rotation for next update
            previousAngle = transform.eulerAngles;
            running_on = true;
        }
        else
        {
            fly = false;
            if(running_on)
            {
                running_on = false;
            }
            

        }
    }

    void OnTriggerStay(Collider other)
    {
        if (fly)
        {
            Player.instance.StartTransport(playerPosition);
            Player.instance.GetComponent<PlayerMovementController>().isGrounded = true;
            Player.instance.GetComponent<PlayerMovementController>().fallLimit = 2f;

            running = true;
        }
    }
	public override void PerformReset ()
	{
		if( running ) 
			Player.instance.state = Player.State.WALKING;

		CameraController.instance.RemoveFromQueue( cameraFollowable );
        running = false;

        
		base.PerformReset ();
	}

    public void wobbleCounter()
    {
        wobbleTime -= Time.deltaTime;
        if(wobbleTime <= 0)
        {
            switchWobbleDir = !switchWobbleDir;
            wobbleTime = Random.Range(.3f, .5f);
        }

    }

    
}
