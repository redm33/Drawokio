using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PopupController : MonoBehaviour {

	public UnityEngine.UI.Image image;
	public Sprite[] menuList;
	public static List<int> popupList = new List<int>();
	public static List<float> popupDelay = new List<float>();
	public static List<float> popupLength = new List<float>();

	private bool isDisplaying = false;
	private bool hasFadedIn = false;
	private bool isFading = false;
	private float delayTime = 0;
	private float displayTime = 0;
	private float fadeAmount = 0;
	private float fadeSpeed = 4f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!isDisplaying) {
			if(popupList.Count > 0) {
				delayTime = popupDelay[0];
				displayTime = popupLength[0];
				image.sprite = menuList[popupList[0]];
				popupDelay.RemoveAt(0);
				popupLength.RemoveAt (0);
				popupList.RemoveAt(0);
				isDisplaying = true;
				image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
			}
		} else {
			if(delayTime > 0) {
				delayTime -= Time.deltaTime;
			} else if(!hasFadedIn) {
				if( fadeAmount < 1 ) {
					fadeAmount += Time.deltaTime * fadeSpeed;
			}else {
					fadeAmount = 1;
					hasFadedIn = true;
				}
				image.color = new Color(image.color.r, image.color.g, image.color.b, fadeAmount);
			} else if(displayTime > 0) {
				image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
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
				image.color = new Color(image.color.r, image.color.g, image.color.b, fadeAmount);
			}
	    }
	}

	public static void QueuePopup(int popupNumber, float delay, float length) {
		popupList.Add (popupNumber);
		popupDelay.Add (delay);
		popupLength.Add (length);
	}
}
