using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Camera/Camera Followable")]
public class CameraFollowable : MonoBehaviour 
{
	public int priority = 1;

	public bool instant = false;

	[SerializeField]
	private Transform _followTransform = null;
	public Transform followTransform 
    {
		get { return (_followTransform == null ? transform : _followTransform); }
	}

	public Vector3 offset = Vector3.zero;
	public Vector3 target 
    {
		get 
        {
			if( _followTransform == null )
				return transform.position + offset;
			else
				return _followTransform.position + offset;
		}
	}
}
