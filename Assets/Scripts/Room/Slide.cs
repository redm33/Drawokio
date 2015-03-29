using UnityEngine;
using System.Collections;

public class Slide : Patrol
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


    void Start()
    {
        camera = GameObject.Find("Main Camera");
        cameraPos = new Vector3(14.7f, 4.6f, -1.4f);
        cameraRot = new Vector3(0,270f, 0);

    }
    void Update()
    {
        if (running)
        {
            //camera.GetComponent<ThirdPersonCamera>().enabled = false;

            //camera.transform.parent = this.transform;
            GameObject.Find("Main Camera").GetComponent<ThirdPersonCamera>().camState = ThirdPersonCamera.CamStates.Front;
            Player.instance.movementController.isGrounded = false;
            Player.instance.animationController.state = PlayerAnimationController.State.AIRBORNE;
        }   
    }

    void OnTriggerStay(Collider other)
    {
        if (other.name == "Player")
        {
            Player.instance.transform.parent = this.transform;
            Player.instance.StartTransport(Player.instance.transform);
            Player.instance.transform.localPosition = new Vector3(.87f, .86f, 0);
            Player.instance.transform.localEulerAngles = new Vector3(325,90,4.5859f);
            Player.instance.GetComponent<PlayerMovementController>().isGrounded = true;
            Player.instance.GetComponent<PlayerMovementController>().fallLimit = 2f;
            Player.instance.GetComponent<DisolveShader>().PauseDissolve();
            fadeAmount = fadeAmount > 0 ? fadeAmount : white.GetFloat("_FadePosition");
            white.SetFloat("_FadePosition", -1);
            black.SetFloat("_FadePosition", -1);
            running = true;
        }
    }
    public override void PerformReset()
    {
        if (running)
            Player.instance.state = Player.State.WALKING;

        CameraController.instance.RemoveFromQueue(cameraFollowable);
        running = false;
        Player.instance.GetComponent<DisolveShader>().UnPauseDissolve();

        Player.instance.transform.parent = null;
        Player.instance.transform.localScale = new Vector3(.1f, .1f, .1f);

        white.SetFloat("_FadePosition", fadeAmount);
        black.SetFloat("_FadePosition", fadeAmount);
        fadeAmount = 0;

        base.PerformReset();
    }



}
