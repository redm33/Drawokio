using UnityEngine;
using System.Collections;

public class YarnBall : MonoBehaviour {
    public Transform[] ballWaypoints;
    public Transform yarnball;
    public float fallSpeed = 1;


    int currentWaypoint = 0;



    public bool isFalling = false;
    public bool hasFallen = false;

    // Use this for initialization
    void Start() {
        Reset();
    }

    // Update is called once per frame
    void Update() {
        if(isFalling) {
            if(yarnball.position == ballWaypoints[currentWaypoint].position) {
                if(currentWaypoint < ballWaypoints.Length - 1) {
                    currentWaypoint++;
                } else {
                    EndFalling();
                }
            }

            Quaternion currentRotation = yarnball.rotation;

            //Transform temp = yarnball;
            //temp.LookAt(ballWaypoints[currentWaypoint]);
            //Quaternion newRotation = temp.rotation;
            
            //yarnball.rotation = Quaternion.Slerp(currentRotation, newRotation, 4 * Time.deltaTime);
            yarnball.Rotate(12, 0, 0);
            yarnball.position = Vector3.MoveTowards(yarnball.transform.position, ballWaypoints[currentWaypoint].position, .1f * fallSpeed);
        }
    }

    void StartFalling() {
        isFalling = true;
    }

    void EndFalling() {
        isFalling = false;
         //Reset();
    }

    void Reset() {
        currentWaypoint = 0;
        isFalling = false;
        yarnball.position = ballWaypoints[0].position;
        yarnball.LookAt(ballWaypoints[currentWaypoint + 1]);
    }

    void OnTriggerStay(Collider other) {
        if(other.tag.Equals("Player")) {
            Debug.Log("In Yarn Trigger");
            if(!hasFallen) {
                StartFalling();
            }
        }
        hasFallen = true;
    }
}

