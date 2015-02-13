using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Drawing/Canvas")]
public class DrawingCanvas : MonoBehaviour 
{
	public static int layer = 11;

	public enum LockType 
    {
		LOCK_X,
		LOCK_Y,
		LOCK_Z
	}
	public LockType lockType = LockType.LOCK_Z;

	public static RigidbodyConstraints GetConstraintsForLockType( LockType lockType ) 
    {
		switch( lockType ) 
        {
		case LockType.LOCK_X:
			return RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
		case LockType.LOCK_Y:
			return RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		default:
			return RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
		}
	}

	public RigidbodyConstraints drawingConstraints 
    {
		get 
        {
			return GetConstraintsForLockType(lockType);
		}
	}
}
