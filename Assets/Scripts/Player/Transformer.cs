using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Player/Transformer")]
public class Transformer : MonoBehaviour 
{

	public static int layer = 10;

	public Transform target2D, target3D;
	public DrawingCanvas.LockType lockType;
	public PlayerTransformationController.TransformType transformType;
	public bool isHidden = true;

	public bool ignore3DX, ignore3DY, ignore3DZ;
}
