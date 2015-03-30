using UnityEngine;
using System.Collections;

public class EndingController : MonoBehaviour 
{

	public static EndingController instance;

	public bool runAtStartup = false;
    public static bool topHatSet = false;
	public TextMesh text;

	[System.Serializable]
	public class Step 
    {
		public float duration = 1;
		[HideInInspector]
		public float elapsed = 0;

		public CameraFollowable followable;
		public Animation animation;

		public GameObject[] gameObjectsToEnable;

		public bool skipStop = false;

		public void Reset()
		{
			elapsed = 0;

			if( animation != null ) 
            {
				animation.Rewind();
				animation.Stop();
			}

			foreach( GameObject obj in gameObjectsToEnable )
            {
                obj.SetActive(false);
                if (obj.name == "BasicEnding" && !topHatSet)
                    ((MovieTexture)obj.GetComponent<Renderer>().material.mainTexture).Stop();
                if (obj.name == "TopHatEnding" && topHatSet)
                    ((MovieTexture)obj.GetComponent<Renderer>().material.mainTexture).Stop();
            }
		}

		public void Start()
		{
			if( followable != null ) 
            {
				CameraController.instance.Reset();
				CameraController.instance.AddToQueue( followable );
			}

			if( animation != null ) 
				animation.Play();

            foreach (GameObject obj in gameObjectsToEnable)
            {
                if (obj.name == "BasicEnding" && !topHatSet)
                {
                    ((MovieTexture)obj.GetComponent<Renderer>().material.mainTexture).Play();
                    obj.SetActive(true);
                }
                else if (obj.name == "TopHatEnding" && topHatSet)
                {
                    ((MovieTexture)obj.GetComponent<Renderer>().material.mainTexture).Play();
                    obj.SetActive(true);
                }
                else if(obj.name != "BasicEnding" && obj.name != "TopHatEnding")
                    obj.SetActive(true);

                
            }
		}

		public void Stop()
		{
			if( skipStop )
				return;

			if( animation != null ) 
            {
				animation.Rewind();
				animation.Stop();
			}

            foreach (GameObject obj in gameObjectsToEnable)
            {
                obj.SetActive(false);
                if (obj.name == "BasicEnding" && !topHatSet)
                    ((MovieTexture)obj.GetComponent<Renderer>().material.mainTexture).Stop();
                if (obj.name == "TopHatEnding" && topHatSet)
                    ((MovieTexture)obj.GetComponent<Renderer>().material.mainTexture).Stop();
            }
		}

		public bool Update( float dt )
		{
			elapsed += dt;
			if( elapsed > duration )
				return false;

			return true;
		}
	}

	public Step[] steps;

	void Start()
	{
		instance = this;
		if( runAtStartup )
			Run();
	}

	int index = -1;
	public bool running 
    {
		get 
        {
			return (index >= 0);
		}
	}

	public void ResetAll()
	{
		index = -1;
		foreach( Step step in steps )
			step.Reset();

		Room.instance.state = Room.State.MENU_MAIN;
	}

	public void Run()
	{
		if( running ) 
        {
			Debug.LogWarning( "Tried to start the ending when it was already running!" );
			return;
		}

        GameObject.Find("Main Camera").GetComponent<ThirdPersonCamera>().enabled = false;
		index = 0;
		steps[0].Start();
		Debug.Log ("Completion: " + Room.instance.completionPercentage + "%");
		text.text = "Completion: " + Room.instance.completionPercentage + "%";
	}

	void OnTriggerEnter( Collider other )
	{
		Run ();
        topHatSet = Player.instance.hats[2].active;
		Destroy ( other.gameObject );
	}

	void Update()
	{
		if( running ) 
        {
			if( !steps[index].Update( Time.deltaTime ) ) 
            {

				if( ++index == steps.Length ) 
					ResetAll ();
				else {
					steps[index-1].Stop();
					steps[index].Start();

				}

			}
		}
	}
}
