using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour 
{

	void OnTriggerEnter( Collider other ) 
    {
		int layer = other.gameObject.layer;

		if( layer == Player.layer2D || layer == Player.layer3D ) 
			other.GetComponent<Player>().Kill();
        else if( layer == Pencil.layer || layer == Charcoal.layer ) 
			other.GetComponent<Ink>().Erase();
	}
}
