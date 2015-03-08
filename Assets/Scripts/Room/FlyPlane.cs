using UnityEngine;
using System.Collections;

public class FlyPlane : MonoBehaviour 
{

    void OnTriggerEnter()
    {
       this.transform.parent.transform.parent.GetComponent<Plane>().fly = true;
    }
}
