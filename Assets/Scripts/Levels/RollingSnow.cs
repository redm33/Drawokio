using UnityEngine;
using System.Collections;

public class RollingSnow : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay(Collider other)
    {
        int layer = other.gameObject.layer;
        float dir = Player.instance.rigidbody.velocity.z;

        if (layer == Player.layer2D || layer == Player.layer3D)
        {
            Player.instance.GetComponent<PlayerMovementController>().speed = .5f;
            Player.instance.GetComponent<PlayerMovementController>().movementInput = new Vector3(Player.instance.GetComponent<PlayerMovementController>().movementInput.x, Player.instance.GetComponent<PlayerMovementController>().movementInput.y, -Player.instance.GetComponent<PlayerMovementController>().movementInput.z);

            //if (dir < 0)
                //Player.instance.rigidbody.velocity = new Vector3(Player.instance.rigidbody.velocity.x, Player.instance.rigidbody.velocity.y, Player.instance.rigidbody.velocity.z + 10);
            //else
                //Player.instance.rigidbody.velocity = new Vector3(Player.instance.rigidbody.velocity.x, Player.instance.rigidbody.velocity.y, Player.instance.rigidbody.velocity.z - 10);

        }
            
        else if (layer == Pencil.layer || layer == Charcoal.layer)
            other.GetComponent<Ink>().Erase();
    }


    void OnTriggerExit(Collider other)
    {
        Player.instance.GetComponent<PlayerMovementController>().speed = 1.15f;
    }
}
