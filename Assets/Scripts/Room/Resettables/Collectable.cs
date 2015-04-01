using UnityEngine;
using System.Collections;

public class Collectable : Resettable {
	
	public string objectTitle;
	public FadingText text;
	public ParticleSystem collected;

	void OnTriggerEnter( Collider other ) 
	{
		if(other.tag.Equals("Player")) {
			if (!pickedUp) {
				pickedUp = true;
				text.QueueShardPopup (0, 5, objectTitle);
				gameObject.renderer.enabled = false;
				collected.Play();
			}
		}
	}
	
	public override void PerformReset ()
	{
		pickedUp = false;
		gameObject.renderer.enabled = true;
	}
	
	public override bool isProgress 
	{
		get 
		{
			return true;
		}
	}
}
