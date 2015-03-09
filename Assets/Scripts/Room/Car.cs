using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

    public bool drivable = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Player.instance != null)
        {
            if (drivable)
            {
                //Player.instance.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + .5f, this.transform.position.z);
                //Player.instance.transform.rotation = this.transform.rotation;
                this.transform.parent.transform.parent.transform.rotation = Player.instance.transform.rotation;
                this.transform.parent.transform.parent.transform.position = new Vector3(Player.instance.transform.position.x, this.transform.parent.transform.parent.transform.position.y, Player.instance.transform.position.z);
            }
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "Player")
        {
            Debug.Log("Enter");
            drivable = true;
            //col.gameObject.rigidbody.useGravity = true;
            Player.instance.GetComponent<PlayerMovementController>().fallLimit = 2f;
            Player.instance.GetComponent<PlayerMovementController>().enabled = false;
            Player.instance.GetComponent<PlayerDrivingController>().enabled = true;
            Player.movement = 'D';

        }     
    }

    void OnTriggerExit(Collider col)
    {
        if (col.name == "Player")
        {
            Debug.Log("Exit");
            drivable = false;
            Player.instance.GetComponent<PlayerMovementController>().enabled = true;
            Player.instance.GetComponent<PlayerDrivingController>().enabled = false;
            Player.movement = 'M';

        }
    }


}
