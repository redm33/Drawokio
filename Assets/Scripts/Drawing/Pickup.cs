using UnityEngine;
using System.Collections;

public class Pickup : Resettable 
{
    public ParticleSystem pickup;
	public enum Type 
    {
		PENCIL,
		CHARCOAL,
        PEN
	}
	public Type type;

	void OnTriggerEnter( Collider other ) 
    {
				if (type == Type.PENCIL) {
						Drawer.instance.hasPencil = true;
                        pickup.Play();
						PopupController.QueuePopup(7, 0.0f, 10.0f);
			Debug.Log("Picked up");
				} else if (type == Type.CHARCOAL) {
						Drawer.instance.hasCharcoal = true;
						PopupController.QueuePopup(8, 0.0f, 10.0f);
				} else if (type == Type.PEN) {
						Drawer.instance.hasPen = true;
						PopupController.QueuePopup(6, 0.0f, 10.0f);
				}

		gameObject.SetActive(false);



		//info.SetActive(true);

		pickedUp = true;
	}

	public override void PerformReset ()
	{
		gameObject.SetActive(true);
		//info.SetActive(false);

		pickedUp = false;
	}

	public override bool isPickup 
    {
		get 
        {
			return true;
		}
	}

	public override void ForcePickup ()
	{
		if( type == Type.PENCIL )
			Drawer.instance.hasPencil = true;
		else if( type == Type.CHARCOAL )
			Drawer.instance.hasCharcoal = true;
        else if (type == Type.PEN)
            Drawer.instance.hasPen = true;
		gameObject.SetActive(false);

		pickedUp = true;
	}

	public GameObject info;
}
