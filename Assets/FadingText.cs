using UnityEngine;
using System.Collections;

public class FadingText : MonoBehaviour {
	public UnityEngine.UI.Text text;
	private bool isDisplaying = false;
	private bool hasFadedIn = false;
	private bool isFading = false;
	private float delayTime = 0;
	private float displayTime = 0;
	private float fadeAmount = 0;
	private float fadeSpeed = 4f;

	// Update is called once per frame
	void Update () {
		if(isDisplaying){
			if(delayTime > 0) {
				delayTime -= Time.deltaTime;
			} else if(!hasFadedIn) {
				if( fadeAmount < 1 ) {
					fadeAmount += Time.deltaTime * fadeSpeed;
				}else {
					fadeAmount = 1;
					hasFadedIn = true;
				}
				text.color = new Color(text.color.r, text.color.g, text.color.b, fadeAmount);
			} else if(displayTime > 0) {
				text.color = new Color(text.color.r, text.color.g, text.color.b, 1.0f);
				displayTime -= Time.deltaTime;
			} else if(!isFading) {
				if( fadeAmount > 0 ) 
					fadeAmount = Mathf.Max( 0, fadeAmount - fadeSpeed * Time.deltaTime );
				else {
					delayTime = 0;
					displayTime = 0;
					fadeAmount = 0;
					hasFadedIn = false;
					isFading = false;
					isDisplaying = false;
				}
				text.color = new Color(text.color.r, text.color.g, text.color.b, fadeAmount);
			}
		}
	}

	public void QueuePopup(float delay, float length, string info) {
		text.text = info + " Discovered";
		delayTime = delay;
		displayTime = length;
		isDisplaying = true;
		text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
	}
}
