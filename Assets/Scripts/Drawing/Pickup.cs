using UnityEngine;
using System.Collections;

public class Pickup : Resettable {

	public enum Type {
		PENCIL,
		CHARCOAL
	}
	public Type type;

	void OnTriggerEnter( Collider other ) {
		if( type == Type.PENCIL )
			Drawer.instance.hasPencil = true;
		else if( type == Type.CHARCOAL )
			Drawer.instance.hasCharcoal = true;

		gameObject.SetActive(false);
		info.SetActive(true);

		pickedUp = true;
	}

	public override void PerformReset ()
	{
		gameObject.SetActive(true);
		info.SetActive(false);

		pickedUp = false;
	}

	public override bool isPickup {
		get {
			return true;
		}
	}

	public override void ForcePickup ()
	{
		if( type == Type.PENCIL )
			Drawer.instance.hasPencil = true;
		else if( type == Type.CHARCOAL )
			Drawer.instance.hasCharcoal = true;
		gameObject.SetActive(false);

		pickedUp = true;
	}

	public GameObject info;
}
