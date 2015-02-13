using UnityEngine;
using System.Collections;

public class RaveTrigger : MonoBehaviour 
{
	void OnTriggerEnter( Collider other ) 
    {
		Rave.raving = !Rave.raving;
	}
}
