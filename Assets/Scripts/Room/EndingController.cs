using UnityEngine;
using System.Collections;

public class EndingController : MonoBehaviour 
{

	public static EndingController instance;

	public static bool endingrunning = false;

	private static bool foundBalloon = false;
	private static bool foundRC = false;
	private static bool foundToy = false;
	private static bool foundTree = false;

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
		public GameObject[] videosToPlay;
		public GameObject[] videosToMaybePlay;
		public bool playEndingVideos = false;
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

			if (playEndingVideos) {
				foreach(GameObject vid in videosToPlay) {
					vid.SetActive(true);
					vid.GetComponent<movieController_script>().PlayVideos();
				}



				if(foundTree) {
					videosToMaybePlay[0].GetComponent<movieController_script>().PlayVideos();
					videosToMaybePlay[0].SetActive(true);
				}
				if(foundBalloon) {
					if(foundRC) {
						if(foundToy) {
							videosToMaybePlay[7].GetComponent<movieController_script>().PlayVideos();
							videosToMaybePlay[7].SetActive(true);

						} else {
							videosToMaybePlay[6].GetComponent<movieController_script>().PlayVideos();
							videosToMaybePlay[6].SetActive(true);

						}
					} else if(foundToy) {
						videosToMaybePlay[8].GetComponent<movieController_script>().PlayVideos();
						videosToMaybePlay[8].SetActive(true);

					} else {
						videosToMaybePlay[4].GetComponent<movieController_script>().PlayVideos();
						videosToMaybePlay[4].SetActive(true);

					}
				} else if(foundRC) {
					if(foundToy) {
						videosToMaybePlay[5].GetComponent<movieController_script>().PlayVideos();
						videosToMaybePlay[5].SetActive(true);

					} else {
						videosToMaybePlay[2].GetComponent<movieController_script>().PlayVideos();
						videosToMaybePlay[2].SetActive(true);

					}
				} else if(foundToy) {
					videosToMaybePlay[3].GetComponent<movieController_script>().PlayVideos();
					videosToMaybePlay[3].SetActive(true);

				} else {
					videosToMaybePlay[1].GetComponent<movieController_script>().PlayVideos();
					videosToMaybePlay[1].SetActive(true);

				}
			}

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
	} //End of class Step

	public Step[] steps;

	void Start()
	{
		instance = this;
		if( runAtStartup )
			Run();
	}

	public static void FindTree() {
		foundTree = true;
	}

	public static void FindRC() {
		foundRC = true;
	}

	public static void FindToy() {
		foundToy = true;
	}

	public static void FindBalloon() {
		foundBalloon = true;
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

    //Start an ending
	public void Run() {
		if( running ) { //If an ending is already running
			Debug.LogWarning( "Tried to start the ending when it was already running!" );
			return;
        } else { 
            GameObject.Find("Main Camera").GetComponent<ThirdPersonCamera>().enabled = false;
		    index = 0;
		    steps[0].Start();
		    Debug.Log ("Completion: " + Room.instance.GetCompletionPercentage() + "%");
		    text.text = "Completion: " + Room.instance.GetCompletionPercentage() + "%";
        }
	}

	void OnTriggerEnter( Collider other )
	{
		Run ();
		endingrunning = true;
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
