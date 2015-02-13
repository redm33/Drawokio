using UnityEngine;
using System.Collections;

public class Pellet : Resettable 
{

	void OnTriggerEnter( Collider other ) 
    {
		Gate.instance.gathered++;

		gameObject.SetActive(false);
	}

	public override void PerformReset ()
	{
		gameObject.SetActive(true);
	}

}
