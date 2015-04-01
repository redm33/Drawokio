using UnityEngine;
using System.Collections;

public class DiscoverableArea : Resettable {

	public string areaTitle;
	public FadingText text;

	void OnTriggerEnter( Collider other ) 
	{
		if(other.tag.Equals("Player")) {
			if (!pickedUp) {
				text.QueuePopup (0, 5, areaTitle);
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
