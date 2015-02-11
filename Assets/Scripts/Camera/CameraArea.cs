using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Camera/Camera Area")]
public class CameraArea : MonoBehaviour {

	public CameraController cameraController { get { return CameraController.instance; } }

	public CameraFollowable followable;

	private Collider cur;
	bool inArea = false;

	void OnTriggerEnter( Collider other ) {
		cur = other;
		inArea = true;

		cameraController.AddToQueue( followable );
	}

	void OnTriggerExit( Collider other ) {
		inArea = false;
		cur = null;

		cameraController.RemoveFromQueue( followable );
	}

	void Update()
	{
		if( inArea && ( cur == null || !cur.gameObject.activeInHierarchy ) )
			OnTriggerExit(null);
	}
}
