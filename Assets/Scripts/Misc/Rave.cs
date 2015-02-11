using System.Collections;
using UnityEngine;

public class Rave : MonoBehaviour
{
	public static bool raving = false;

	Color realStartColor;

	Color destColor, startColor;
	float progress = 0;

	public float speed = 1;
	public float raveIntensity = 3;
	float startIntensity;

	void Awake()
	{
		realStartColor = startColor = light.color;
		destColor = new Color( Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f) );

		startIntensity = light.intensity;
	}

	void Update()
	{
		if( raving ){
			light.intensity = raveIntensity;

			progress += Time.deltaTime * speed;

			if( progress > 1 ) {
				light.color = destColor;

				startColor = destColor;
				destColor = new Color( Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f) );

				progress = 0;
			} else {
				light.color = Color.Lerp( startColor, destColor, progress );
			}
		} else {
			light.color = realStartColor;
			light.intensity = startIntensity;
		}
	}
}
