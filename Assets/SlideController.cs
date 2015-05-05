using UnityEngine;
using System.Collections;

public class SlideController : MonoBehaviour {
    public Transform[] planeWaypoints;
    public Transform airplane;
    public Transform playerPosition;
    public Transform endPlayerPosition;
    public float flySpeed = 1;
    private Transform cameraTransformParent;

    public Vector3 cameraPos;
    public Vector3 cameraRot;
    public GameObject camera;

    int currentWaypoint = 0;



    public bool isFlying = false;
    public bool hasFlown = false;

    public Material white;
    public Material black;
    private float fadeAmount = 0;

    // Use this for initialization
    void Start() {
        Reset();
    }

    // Update is called once per frame
    void Update() {
        if(isFlying) {
            

            Quaternion currentRotation = airplane.rotation;

            Transform temp = airplane;
            temp.LookAt(planeWaypoints[currentWaypoint]);
            Quaternion newRotation = temp.rotation;

            airplane.rotation = Quaternion.Slerp(currentRotation, newRotation, 4 * Time.deltaTime);
            airplane.position = Vector3.MoveTowards(airplane.transform.position, planeWaypoints[currentWaypoint].position, .1f * flySpeed);

            camera.GetComponent<ThirdPersonCamera>().enabled = false;

            camera.transform.parent = this.transform;
            camera.transform.localEulerAngles = cameraRot;
            camera.transform.localPosition = cameraPos;

            if(airplane.position == planeWaypoints[currentWaypoint].position) {
                if(currentWaypoint < planeWaypoints.Length - 1) {
                    currentWaypoint++;
                } else {
                    EndFlying();
                }
            }

        } else if(camera.transform.parent == this.transform) {
            camera.transform.parent = cameraTransformParent;
        }
    }

    void StartFlying() {
        camera.GetComponent<ThirdPersonCamera>().enabled = false;
        Player.instance.transform.parent = this.transform;
        cameraTransformParent = camera.transform.parent;
        camera.transform.parent = this.transform;
        isFlying = true;
    }

    void EndFlying() {
        if(isFlying)
            Player.instance.state = Player.State.WALKING;
        isFlying = false;


        Player.instance.GetComponent<DisolveShader>().UnPauseDissolve();

        Player.instance.transform.parent = null;
        Player.instance.transform.localScale = new Vector3(.1f, .1f, .1f);
        Player.instance.transform.position = endPlayerPosition.position;

        camera.GetComponent<ThirdPersonCamera>().enabled = true;

        camera.transform.parent = cameraTransformParent;


        white.SetFloat("_FadePosition", fadeAmount);
        black.SetFloat("_FadePosition", fadeAmount);
        fadeAmount = 0;

        hasFlown = false;
        Reset();
        //airplane.gameObject.SetActive(false);
        //Rigidbody gameObjectsRigidBody = airplane.gameObject.AddComponent<Rigidbody>();
        //gameObjectsRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        //gameObjectsRigidBody.mass = 100;
        //Reset();
    }

    void Reset() {
        currentWaypoint = 0;
        isFlying = false;
        airplane.position = planeWaypoints[0].position;
        airplane.LookAt(planeWaypoints[currentWaypoint + 1]);
    }

    void OnTriggerStay(Collider other) {
        if(other.tag.Equals("Player")) {
            Debug.Log("In Plane Trigger");
            if(!hasFlown) {
                Debug.Log("In Plane Trigger");
                Player.instance.StartTransport(playerPosition);
                Player.instance.GetComponent<PlayerMovementController>().isGrounded = true;
                Player.instance.GetComponent<PlayerMovementController>().fallLimit = 2f;
                Player.instance.GetComponent<DisolveShader>().PauseDissolve();
                fadeAmount = fadeAmount > 0 ? fadeAmount : white.GetFloat("_FadePosition");
                white.SetFloat("_FadePosition", -1);
                black.SetFloat("_FadePosition", -1);
                if(!isFlying) {
                    StartFlying();
                }
            }
        }
        hasFlown = true;
    }
}

