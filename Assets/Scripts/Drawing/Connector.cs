using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Game/Drawing/Connector")]
public class Connector : InheritanceBase
{
	public enum Type 
    {
		DEFAULT,
		PENCIL,
		CHARCOAL,
        PEN
	}
	public virtual Type type 
    {
		get { return Type.DEFAULT; }
	}

}
