using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

    public static bool drivable = false;
    public GameObject car;
    public Transform driverPosition;
    public Transform ejectPosition;

	// Use this for initialization
	void Start () {
        car = GameObject.Find("CarChild");
	}
	
	// Update is called once per frame
	void Update () {
        if (Player.instance != null)
        {
            if (drivable)
            {
                Player.instance.transform.eulerAngles = new Vector3(car.transform.eulerAngles.x, car.transform.eulerAngles.y + 180, car.transform.eulerAngles.z); 
                Player.instance.transform.position = driverPosition.position;
            }
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "Player")
        {
			EndingController.FindRC();
            drivable = true;
            //Player.instance.rigidbody.isKinematic = true;
            Player.instance.transform.parent = GameObject.Find("CarChild").transform;

            Player.instance.GetComponent<PlayerMovementController>().fallLimit = 2f;
            if (Player.instance.GetComponent<PlayerMovementController>().enabled)
            {
                Player.instance.GetComponent<PlayerMovementController>().enabled = false;
                Player.instance.GetComponent<PlayerMovementController>().isGrounded = true;
            }
            Player.instance.GetComponent<PlayerDrivingController>().enabled = true;

            Camera.main.transform.parent = GameObject.Find("CarChild").transform;


            GameObject.Find("CarChild").GetComponent<PlayerCar_Script>().enabled = true;
            GameObject.Find("CarChild").rigidbody.isKinematic = false;

            Player.instance.GetComponent<DisolveShader>().enabled = false;


            Player.movement = 'D';

        }     
    }

    void OnTriggerExit(Collider col)
    {
        if (col.name == "Player" && Input.GetButton("Jump"))
        {
            drivable = false;
            camera.transform.parent = null;
            /*
            Player.instance.rigidbody.isKinematic = false;

            Player.instance.GetComponent<PlayerMovementController>().enabled = true;
            Player.instance.GetComponent<PlayerMovementController>().isGrounded = false;

            GameObject.Find("CarChild").GetComponent<PlayerCar_Script>().enabled = false;

            Player.instance.GetComponent<PlayerDrivingController>().enabled = false;
            GameObject.Find("CarChild").rigidbody.isKinematic = true;

            Player.movement = 'M';
             * */

        }
    }


}
