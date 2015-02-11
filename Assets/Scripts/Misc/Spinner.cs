using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour {

	public float speed = 90.0f;

	void Update()
	{
		transform.Rotate( Vector3.up, speed * Time.deltaTime );
	}
}
