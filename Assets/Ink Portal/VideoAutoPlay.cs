using UnityEngine;
using System.Collections;

public class VideoAutoPlay : MonoBehaviour {

    public GameObject gameObj;
    private MovieTexture text;

    void Start() {
        text = (MovieTexture)gameObj.renderer.material.mainTexture;
        text.loop = true;
        text.Play();
    }

}
