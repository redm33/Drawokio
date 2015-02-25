using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

    bool drivable = false;
    bool updateOnce = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(drivable && updateOnce)
        {
            Player.instance.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + .5f, this.transform.position.z);
            transform.parent = Player.instance.transform;
            updateOnce = false;
        }
        else if(updateOnce)
        {
            transform.parent = GameObject.Find("PlayerInteractables").transform;
        }

	}

    void OnTriggerStay(Collider col)
    {
        if (col.name == "Player" && Input.GetKeyDown(KeyCode.Tab))
        {
            drivable = !drivable;
            updateOnce = true;
            Debug.Log("Tab Hit");
        }
        
    }

}
