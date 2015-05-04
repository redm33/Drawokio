using UnityEngine;
using System.Collections;

public class VideoAutoPlay : MonoBehaviour {

   // public GameObject gameObj;
    public MovieTexture text;
	public MovieTexture mask;

    void Start() {
       // text = (MovieTexture)gameObj.renderer.material.mainTexture;
        if(text != null)
        {
            text.loop = true;
            text.Play();
        }
		if(mask != null)
		{
			mask.loop = true;
			mask.Play();
		}
        
    }

}
