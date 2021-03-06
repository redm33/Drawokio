﻿using UnityEngine;
using System.Collections;

public class Plane : Patrol 
{

	public CameraFollowable cameraFollowable;
	public Transform playerPosition;
    public GameObject camera;

	public Material white;
	public Material black;
	private float fadeAmount = 0;

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
        cameraPos = new Vector3(78f, 55.5f, 2.4f);
        cameraRot = new Vector3(46,270,0);
        previousAngle = this.transform.eulerAngles;
        changing_plane = this.transform.eulerAngles;

    }
    void Update()
    {
        if (running)
        {

            Quaternion turnTo = Quaternion.LookRotation(patrolPoints[cur].transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, turnTo, Time.deltaTime*2);
            wobbleCounter();

            camera.GetComponent<ThirdPersonCamera>().enabled = false;
            Player.instance.transform.parent = this.transform;
            camera.transform.parent = this.transform;

            camera.transform.localPosition = cameraPos;
            camera.transform.localEulerAngles = cameraRot;

            //If boat pointing to sky - reduce X
            if (switchWobbleDir)
            {
                changing_plane = new Vector3(transform.eulerAngles.x - angleDelta.z * Time.deltaTime, transform.eulerAngles.y, transform.eulerAngles.z);
                //changing_player = new Vector3(Player.instance.transform.eulerAngles.x, Player.instance.transform.eulerAngles.y - angleDelta.z * Time.deltaTime, Player.instance.transform.eulerAngles.z);

                //transform.rotation.x -= angleDelta.x * Time.deltaTime;


                //If boat diving into ocean - increase X
            }
            else
            {
                changing_plane = new Vector3(transform.eulerAngles.x + angleDelta.z * Time.deltaTime, transform.eulerAngles.y, transform.eulerAngles.z);
                //changing_player = new Vector3(Player.instance.transform.eulerAngles.x, Player.instance.transform.eulerAngles.y + angleDelta.z * Time.deltaTime, Player.instance.transform.eulerAngles.z);

                //transform.eulerAngles.x += angleDelta.x * Time.deltaTime; //Must not forget to use deltaTime!
            }

            transform.eulerAngles = changing_plane;
            previousAngle = transform.eulerAngles;
            running_on = true;
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.name == "Player")
        {
            if (fly)
            {
                Debug.Log("In Plane Trigger");
                Player.instance.StartTransport(playerPosition);
                Player.instance.GetComponent<PlayerMovementController>().isGrounded = true;
                Player.instance.GetComponent<PlayerMovementController>().fallLimit = 2f;
				Player.instance.GetComponent<DisolveShader>().PauseDissolve();
				fadeAmount = fadeAmount > 0 ? fadeAmount : white.GetFloat ("_FadePosition");
				white.SetFloat ("_FadePosition", -1);
				black.SetFloat ("_FadePosition", -1);
                running = true;
            }
        }
    }
	public override void PerformReset ()
	{
		if( running ) 
			Player.instance.state = Player.State.WALKING;

		CameraController.instance.RemoveFromQueue( cameraFollowable );
        running = false;
        fly = false;
		Player.instance.GetComponent<DisolveShader>().UnPauseDissolve();

        Player.instance.transform.parent = null;
        Player.instance.transform.localScale = new Vector3(.1f, .1f, .1f);

		white.SetFloat ("_FadePosition", fadeAmount);
		black.SetFloat ("_FadePosition", fadeAmount);
		fadeAmount = 0;

		base.PerformReset ();
	}

    public void wobbleCounter()
    {
        wobbleTime -= Time.deltaTime*.5f;
        if(wobbleTime <= 0)
        {
            switchWobbleDir = !switchWobbleDir;
            wobbleTime = Random.Range(.3f, .5f);
        }

    }

    
}
