using UnityEngine;
using System.Collections;

public class TreeTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            EndingController.FindTree();
        }
    }
}
