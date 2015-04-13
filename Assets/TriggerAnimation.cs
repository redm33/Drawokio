using UnityEngine;
using System.Collections;

public class TriggerAnimation : MonoBehaviour {
    private bool triggered = false;
    private Animation animation;
    public Transform spawnObject;
    public Transform spawnPoint;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("stalagtite"))
        {
        }
        else
        {
            GameObject.Instantiate(spawnObject, spawnPoint.position, spawnPoint.rotation);
            this.gameObject.SetActive(false);
        }
	}

    void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("Player") && !triggered)
        {
            triggered = true;
            animation = this.GetComponent<Animator>().animation;
            this.GetComponent<Animator>().enabled = true;
        }

    }
}
