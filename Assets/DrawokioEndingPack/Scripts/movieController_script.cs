using UnityEngine;
using System.Collections;

public class movieController_script : MonoBehaviour {
	public MovieTexture diffuse;
	public MovieTexture mask;
	// Use this for initialization
	public void PlayVideos () 
	{
		//MovieTexture movie = renderer.material.mainTexture as MovieTexture;
		if (diffuse != null) {
						diffuse.Play ();
				}
		if (mask != null) {
						mask.Play ();
				}
	}


	// Update is called once per frame
	void Update () 
	{
		
	}
}
