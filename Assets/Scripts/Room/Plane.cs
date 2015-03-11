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

    public bool fly = false;

    void Start()
    {
        camera = GameObject.Find("Main Camera");
        cameraPos = new Vector3(-2.7f, -54.34f, -41.14f);
        cameraRot = new Vector3(320.19f, 10, 10); 

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

		base.PerformReset ();
	}

    
}
