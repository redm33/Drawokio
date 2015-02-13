using UnityEngine;
using System.Collections;

public class Hat : Resettable 
{
	public bool startInactive = false;
	public int hatIndex = 0;

	public override void PerformReset ()
	{
		pickedUp = false;
		gameObject.SetActive(!startInactive);
	}

	void OnTriggerEnter( Collider other ) 
    {
		Room.instance.SetHat( hatIndex );
		gameObject.SetActive(false);
		other.GetComponent<Player>().SetHat(hatIndex);

		pickedUp = true;
	}

	public override bool isPickup 
    {
		get 
        {
			return true;
		}
	}

	public override void ForcePickup ()
	{
		gameObject.SetActive(false);
		base.ForcePickup();
	}
}
