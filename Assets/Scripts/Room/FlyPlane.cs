using UnityEngine;
using System.Collections;

public class FlyPlane : MonoBehaviour 
{

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.name);
        this.transform.parent.transform.parent.GetComponent<Plane>().fly = true;
    }
}
