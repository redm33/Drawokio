using UnityEngine;
using System.Collections;

public class CheckpointText : MonoBehaviour {

	public static CheckpointText instance;

	public GUIText text;

	void Awake() {
		instance = this;
	}

	float a = 0;
	public float fadeoutSpeed = 1;

	public void Show() {
		a = 1;
	}

	void Update() {

		if( a > 0 ) {
			a = Mathf.Max( 0, a - fadeoutSpeed * Time.deltaTime );
		} else {
			a = 0;
		}

		Color col = Color.Lerp( Color.white, Color.black, 1-a );
		col.a = a;
		text.color = col;
	}
}
