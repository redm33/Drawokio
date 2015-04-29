using UnityEngine;
using System.Collections;

public class Collectable : Resettable {
	
	public string objectTitle;
	public FadingText text;
	public ParticleSystem collected;
    public GameObject shard;
    public ParticleSystem shardParticles;

	void OnTriggerEnter( Collider other ) 
	{
		if(other.tag.Equals("Player")) {
			if (!pickedUp) {
				pickedUp = true;
				text.QueueShardPopup (0, 5, objectTitle);
				shard.renderer.enabled = false;
                shardParticles.Stop();
				collected.Play();
			}
		}
	}
	
	public override void PerformReset ()
	{
		pickedUp = false;
		shard.renderer.enabled = true;
	}
	
	public override bool isProgress 
	{
		get 
		{
			return true;
		}
	}
}
