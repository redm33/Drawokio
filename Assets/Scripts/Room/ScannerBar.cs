using UnityEngine;
using System.Collections;

public class ScannerBar : Resettable {

	public float speed = 1;
	float elapsed = -.5f;

	float startScale;

	void Awake() {
		startScale = transform.localScale.x;
	}

	void Update() {
		elapsed += speed * Time.deltaTime;

		Vector3 scale = transform.localScale;
		if( elapsed < 0 )
			scale.x = 0;
		else {

			float x = ( Mathf.Cos( Mathf.Min( 6.28f, elapsed * 3.14f ) ) * 0.5f + 0.5f ) * Mathf.Lerp( 0, startScale, elapsed );

			scale.x = x;
		}
		transform.localScale = scale;
	}

	public override void PerformReset ()
	{
		elapsed = -.5f;
	}
}
