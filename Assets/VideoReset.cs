using UnityEngine;
using System.Collections;

public class VideoReset : MonoBehaviour {
	public MovieTexture [] videos;
	// Use this for initialization
	void Start () {
		StartCoroutine (ResetVids ());
	}
	IEnumerator ResetVids() {
		foreach (MovieTexture vid in videos) {
			vid.Stop(); 
			vid.Play(); 
		}
		yield return new WaitForSeconds(.1f);
		foreach (MovieTexture vid in videos) {
						vid.Pause ();
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
