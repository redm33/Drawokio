using UnityEngine;
using System.Collections;

public class DiscoverableArea : Resettable {

	public string areaTitle;

	void OnTriggerEnter( Collider other ) 
	{
		if(other.tag.Equals("Player")) {
			if (!pickedUp) {
				FadingText.instance.QueuePopup (0, 5, areaTitle);
			}
			pickedUp = true;
		}
	}

	public override void PerformReset ()
	{
		pickedUp = false;
	}

	public override bool isProgress 
	{
		get 
		{
			return true;
		}
	}
}
