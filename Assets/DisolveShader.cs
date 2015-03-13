using UnityEngine;
using System.Collections;

public class DisolveShader : MonoBehaviour {

	public Material dissolveMaterial;
	public Material dissolveMaterialBlack;

	public ParticleSystem respawnParticles;

	private PlayerTransformationController.State lastState;
	private PlayerTransformationController.State currentState;
	public float dissolveDelay = 5;
	public float dissolveTime = 5;

	private float delay;
	private float time;

	void Start () {
		lastState = Player.instance.transformationController.state;
		currentState = lastState;
		delay = 0;
		time = 0;
		dissolveMaterial.SetFloat ("_FadePosition", -1);
		dissolveMaterialBlack.SetFloat ("_FadePosition", -1);


	}
	
	// Update is called once per frame
	void Update () {
		currentState = Player.instance.transformationController.state;
		dissolveMaterial.SetFloat ("_ObjectPosition", this.transform.position.y);
		dissolveMaterialBlack.SetFloat ("_ObjectPosition", this.transform.position.y);

		if (lastState.Equals (PlayerTransformationController.State.IN_3D)) {
			if(currentState.Equals(PlayerTransformationController.State.IN_3D)) {
				if(delay < dissolveDelay) {
					delay += Time.deltaTime;
				} else if(time < dissolveTime) {
					time += Time.deltaTime;

					dissolveMaterial.SetFloat ("_FadePosition", (time/dissolveTime)/2);
					dissolveMaterialBlack.SetFloat ("_FadePosition", (time/dissolveTime)/2);

				} else if(time >= dissolveTime) {
					Player.instance.Kill();
				}
			} else if(currentState.Equals(PlayerTransformationController.State.TRANSFORMING_3D_TO_2D)) {
				dissolveMaterial.SetFloat ("_FadePosition", -1);
				dissolveMaterialBlack.SetFloat ("_FadePosition", -1);

				delay = 0;
				time = 0;
			}
		} else if (lastState.Equals (PlayerTransformationController.State.IN_2D)) {
			if(currentState.Equals(PlayerTransformationController.State.TRANSFORMING_3D_TO_2D)) {
				delay = 0;
				time = 0;
			}
		}
		lastState = currentState;
        
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "RespawnTrigger")
        {
            time = 0;
            delay = 0;
            dissolveMaterial.SetFloat("_FadePosition", -1);
            dissolveMaterialBlack.SetFloat("_FadePosition", -1);
			respawnParticles.Play();
        }
    }
}
