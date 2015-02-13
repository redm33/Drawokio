using UnityEngine;
using System.Collections;

public class PickupInfo : MonoBehaviour 
{

	void Update() 
    {
		if( Input.GetButtonDown( "Back" ) ) 
        {
			Room.instance.paused = false;
			gameObject.SetActive(false);
		} 
        else
			Room.instance.paused = true;
	}
}
