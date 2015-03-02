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

    void Start()
    {
        camera = GameObject.Find("Main Camera");
        cameraPos = new Vector3(0, -5.6f, -5.86f);
        cameraRot = new Vector3(300, 0, 8); 

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
    }

    void OnTriggerStay(Collider other)
    {
        Player.instance.StartTransport(playerPosition);
        running = true;
    }
	public override void PerformReset ()
	{
		if( running ) 
			Player.instance.state = Player.State.WALKING;

		CameraController.instance.RemoveFromQueue( cameraFollowable );

		base.PerformReset ();
	}

    
}
