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
    Vector3 angleDelta = new Vector3(0, 0, 10);
    Vector3 changing;
    float accelerationBuffer = 0.3f;
    float decelerationBuffer = 0.1f;

    public bool fly = false;

    void Start()
    {
        camera = GameObject.Find("Main Camera");
        cameraPos = new Vector3(-2.1f, -10.4f, -10.4f);
        cameraRot = new Vector3(310.131f, 10, 10);
        previousAngle = this.transform.eulerAngles;
        changing = this.transform.eulerAngles;

    }
    void Update()
    {
        if (running)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, patrolPoints[cur].position, speed * Time.deltaTime);
            camera.GetComponent<ThirdPersonCamera>().enabled = false;
            camera.transform.parent = Player.instance.transform;
            camera.transform.localPosition = cameraPos;
            camera.transform.localEulerAngles = cameraRot;

            //If boat pointing to sky - reduce X
            if (transform.eulerAngles.z < 45)
            {
                changing = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - angleDelta.z * Time.deltaTime);
                //transform.rotation.x -= angleDelta.x * Time.deltaTime;


            //If boat diving into ocean - increase X
            }
            else if (transform.eulerAngles.z > 315)
            {
                changing = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + angleDelta.z * Time.deltaTime);
                //transform.eulerAngles.x += angleDelta.x * Time.deltaTime; //Must not forget to use deltaTime!
            }

            transform.eulerAngles = changing;
            //record rotation for next update
            previousAngle = transform.eulerAngles;
        }
        else
            fly = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (fly)
        {
            Player.instance.StartTransport(playerPosition);
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

    
}
