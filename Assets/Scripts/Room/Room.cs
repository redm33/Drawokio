using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour 
{

	public static Room instance;

	public Drawer drawer;

	public Player playerPrefab;
	public SpawnPoint[] spawnPoints;
    public MovieTexture openingCutscene;
    public bool playOpening;
    public bool onlyOnce = true;
	public void ResetSpawnPoints()
	{
		foreach( SpawnPoint obj in spawnPoints )
			obj.isLatestSpawn = false;
	}
	public void SpawnAtLatestSpawn()
	{
		if( Player.instance != null ) 
        {
			Debug.LogWarning( "Don't spawn more than one player!" );
			return;
		}

		Continue();
		/*pawnPoint point = spawnPoints[0];
		for( int i = 1; i < spawnPoints.Length; i++ )
			if( spawnPoints[i].isLatestSpawn )
				point = spawnPoints[i];
		StartAt(point);*/
	}

	public GameObject[] disableDuringMenuObjects;

	public GameObject[] menuGameObjects;

	public CameraController cameraController;
	public CameraFollowable[] cameraFollowables;

	public float chooseCameraSpinSpeed = 10;

	public Resettable[] resettables;
	public Resettable[] discoverableAreas;
	public Resettable[] collectables;

	public Pickup pencilPickup, charcoalPickup;

	public GameObject deathText;

	public enum State 
    {
		NONE,
		MENU_MAIN,
		MENU_CHOOSE, // Holdover from when you could choose levels...don't want to change it all over...this is the state it will enter when you continue
		MENU_CONTROLS,
		MENU_QUIT,
		PLAYING,
		QUITTING
	}

    public void SetState( int state )
    {
        this.state = (State)state;
    }

	private State _state = State.NONE;
	public State state 
    {
		get { return _state; }
		set 
        {
			if( _state == value ) 
            {
				if( _state == State.MENU_MAIN ) 
                {
                    playOpening = true;

				}

				return;
			}

			switch( _state ) 
            {
			    case State.MENU_CHOOSE:
				    /*foreach( SpawnPoint point in spawnPoints )
					    point.particleSystem.Stop();*/
				    break;
			    case State.PLAYING:
				    DestroyAll();
				    break;
			    default:
				    break;
			}

			if( (int)_state < menuGameObjects.Length && menuGameObjects[(int)_state] != null ) 
				menuGameObjects[(int)_state].SetActive(false);

			_state = value;

			switch( _state ) 
            {
			    case State.MENU_CHOOSE:
				    /*foreach( SpawnPoint point in spawnPoints )
					    point.particleSystem.Play();*/
				    Continue ();
				    break;
			    case State.PLAYING:
				    drawer.gameObject.SetActive(true);
				    break;
			    case State.QUITTING:
				    quitStart = Time.time;
				    break;
			    case State.MENU_MAIN:
				    foreach( GameObject obj in disableDuringMenuObjects )
					    obj.SetActive(false);
				    break;
			    default:
				    break;
			}
			
			if( (int)_state < menuGameObjects.Length && menuGameObjects[(int)_state] != null ) 
				menuGameObjects[(int)_state].SetActive(true);
					
			cameraController.ClearQueue();
			if( (int)_state < cameraFollowables.Length ) 
				cameraController.AddToQueue( cameraFollowables[(int)_state] );
		}
	}

	void Awake()
	{
		instance = this;

		for( int i = 0; i < spawnPoints.Length; i++ ) 
        {
			spawnPoints[i].index = i;
		}

		for( int i = 0; i < resettables.Length; i++ ) 
        {
			resettables[i].index = i;
		}

		for( int i = 0; i < discoverableAreas.Length; i++ ) 
		{
			discoverableAreas[i].index = i;
		}

		for( int i = 0; i < collectables.Length; i++ ) 
		{
			collectables[i].index = i;
		}
	}

	void Start()
	{
		state = State.MENU_MAIN;
	}

	public void DestroyAll()
	{

        if (Player.instance != null)
        {
            GameObject.Find("Main Camera").transform.parent = Player.instance.transform.parent;
            GameObject.Find("Main Camera").GetComponent<ThirdPersonCamera>().enabled = false;
            Destroy(Player.instance.gameObject);
        }
        drawer.DestroyAll();
		drawer.gameObject.SetActive(false);
		paused = false;
		Drawer.instance.uiOpen = false;
	}

	float quitStart = 0;
	//bool firstDrag = true;
	Vector3 dragStart;
	void Update() 
    {
		switch( state ) 
        {
		    case State.QUITTING:
			    if( Time.time - quitStart > 1.0f ) 
				    Application.Quit();
			    break;
		    case State.MENU_CHOOSE:
			    // Deprecated...MENU_CHOOSE is an old thing that I am now using for continue.
			    /*if( Input.GetMouseButtonDown( 0 ) ) {
				    RaycastHit hit;
				    if( Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit, 1000, (1<<SpawnPoint.layer) ) ) {
					
					    SpawnPoint point = hit.collider.GetComponent<SpawnPoint>();
					    if( point == null )
					    {
						    Debug.LogWarning( "Hit something that wasn't a SpawnPoint!" );
						    return;
					    }

					    StartAt( point );

				    } else {
					    dragStart = Input.mousePosition;
				    }
			    } else if( Input.GetMouseButton(0) ) {
				    if( !firstDrag ) {
					    cameraFollowables[(int)State.MENU_CHOOSE].transform.Rotate( Vector3.up, ( Input.mousePosition.x - dragStart.x ) * chooseCameraSpinSpeed * Time.deltaTime, Space.World );
				    }
				    dragStart = Input.mousePosition;
				    firstDrag = false;
			    } else {
				    firstDrag = true;
			    }*/

			    break;
		    case State.MENU_MAIN:
		    case State.MENU_QUIT:
			    if( Input.GetMouseButtonDown( 0 ) ) 
                {
				    RaycastHit hit;
				    if( Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit, 1000, (1<<MenuButton.layer) ) ) 
                    {

					    MenuButton button = hit.collider.GetComponent<MenuButton>();
					    if( button == null ) 
                        {
						    Debug.LogWarning( "Hit something that wasn't a MenuButton!" );
						    return;
					    }

					    state = button.state;
				    }
			    }
			    break;
		    case State.MENU_CONTROLS:
                if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Back"))
                {
                    state = State.MENU_MAIN;
                    Player.instance.GetComponent<PlayerDrivingController>().Jump();
                }
			    break;
		    case State.PLAYING:
			    if( Player.instance == null && !EndingController.instance.running ) 
                {
				    //deathText.SetActive(true);

				    //if( Input.GetButtonDown( "Back" ) ) 
                    //{
					    DestroyAll();
					    SpawnAtLatestSpawn();
				    //}
			    } 
                else 
				    deathText.SetActive(false);
			    break;
		}
	}

	//
	public int completionPercentage 
    {
		get 
        {
			int found = 0, total = 0;

			foreach( Resettable obj in resettables ) 
            {
				if( obj.isProgress ) 
                {
					total++;
					if( obj.pickedUp )
						found++;
				}
			}

			return 100 * found / total;
		}
	}

	void Continue() 
    {
		StartAt( spawnPoints[PlayerPrefs.GetInt( "SpawnPoint", 0 )] );

		//Loading the resettables
		string pickupsStr = PlayerPrefs.GetString( "Pickups", "" );
		for( int i = 0; i < pickupsStr.Length; i++ ) 
        {
			int index = (int)pickupsStr[i];
			resettables[index-33].ForcePickup();
		}

		//Loading the discoverable areas
		string areasStr = PlayerPrefs.GetString( "Areas", "" );
		for( int i = 0; i < areasStr.Length; i++ ) 
		{
			int index = (int)areasStr[i];
			discoverableAreas[index-33].ForcePickup();
		}

		//Loading the collectables
		string collectStr = PlayerPrefs.GetString( "Collectables", "" );
		for( int i = 0; i < collectStr.Length; i++ ) 
		{
			int index = (int)collectStr[i];
			collectables[index-33].ForcePickup();
		}

		//Loading the hat
		int hat = PlayerPrefs.GetInt("Hat",-1);
		if( hat >= 0 ) 
        {
			SetHat(hat);
			Player.instance.SetHat(hat);
		}
	}

	public bool Save( int spawnPoint ) 
    {
		bool changed = false;

		if( PlayerPrefs.GetInt( "SpawnPoint", 0 ) != spawnPoint )
			changed = true;
		PlayerPrefs.SetInt( "SpawnPoint", spawnPoint );
		
		if( !changed && PlayerPrefs.GetInt( "Hat", 0 ) != currentHat )
			changed = true;
		PlayerPrefs.SetInt( "Hat", currentHat );

		//Saving the resettables
		string pickupsStr = "";
		for( int i = 0; i < resettables.Length; i++ ) 
        {
			if( resettables[i].pickedUp )
				pickupsStr += (char)(i+33);
		}
		
		if( !changed && PlayerPrefs.GetString( "Pickups", "" ) != pickupsStr )
			changed = true;

		PlayerPrefs.SetString( "Pickups", pickupsStr );

		//Saving the discoverable areas
		string areasStr = "";
		for( int i = 0; i < discoverableAreas.Length; i++ ) 
		{
			if( discoverableAreas[i].pickedUp )
				areasStr += (char)(i+33);
		}
		
		if( !changed && PlayerPrefs.GetString( "Areas", "" ) != areasStr )
			changed = true;
		
		PlayerPrefs.SetString( "Areas", areasStr );

		//Saving the collectables
		string collectStr = "";
		for( int i = 0; i < collectables.Length; i++ ) 
		{
			if( collectables[i].pickedUp )
				collectStr += (char)(i+33);
		}
		
		if( !changed && PlayerPrefs.GetString( "Collectables", "" ) != collectStr )
			changed = true;
		
		PlayerPrefs.SetString( "Collectables", collectStr );



		return changed;
	}

	void StartAt( SpawnPoint point ) 
    {

		ResetSpawnPoints();
		point.isLatestSpawn = true;
		
		Player player = Instantiate( playerPrefab, point.transform.position, point.transform.rotation ) as Player;
        player.name = "Player";

		if (point.spawnIn3D) {
						player.transformationController.Become3D ();
						player.transform.Find ("DissolveParticles").gameObject.SetActive (true);
						player.transform.Find ("Blob Shadow Projector").gameObject.SetActive (true);
				} else {
						player.movementController.ApplyLockType (point.lockType);
				}
		SetHat(-1);

		drawer.gameObject.SetActive(true);

		Drawer.instance.hasPencil = Drawer.instance.hasCharcoal = false;

		foreach( Resettable resettable in resettables ) 
        {
			if( resettable != null )
				resettable.PerformReset();
		}
		
		state = State.PLAYING;
	}
	
	public GameObject[] endHats;
	int currentHat = -1;

	public void SetHat( int index ) 
    {
		if( currentHat >= 0 )
			endHats[currentHat].SetActive(false);

		currentHat = index;

		if( index >= 0 && index < endHats.Length ) 
			endHats[index].SetActive(true);
	}

	private bool _paused = false;
	public bool paused 
    {
		get 
        {
			return _paused;
		}
		set 
        {
			if( _paused == value )
				return;

			_paused = value;

			foreach( Resettable resettable in resettables ) 
            {
				if( resettable != null )
					resettable.paused = value;
			}
			
			Player.instance.paused = value;
			
			foreach( Transform child in Drawer.instance.drawingParent ) 
            {
				child.GetComponent<Ink>().paused = value;
			}
		}
	}

    void OnGUI()
    {
        if(playOpening)
        {
            openingCutscene.Play();
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), openingCutscene, ScaleMode.StretchToFill, false, 0.0f);

            if (onlyOnce)
            {
                StartCoroutine(StopVideo());
                onlyOnce = false;
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                SkipStopVideo();
            }
        }
    }

    IEnumerator StopVideo()
    {
        yield return new WaitForSeconds(50f);
        SkipStopVideo();
    }

    void SkipStopVideo()
    {
        if (Player.instance == null)
        {
            playOpening = false;
            onlyOnce = true;
            openingCutscene.Stop();

            PlayerPrefs.SetInt("SpawnPoint", 0);
            PlayerPrefs.SetString("Pickups", "");
            PlayerPrefs.SetInt("Hat", -1);
            StartAt(spawnPoints[0]);
            //PopupController.QueuePopup(0, 0.5f, 5.0f);
            //PopupController.QueuePopup(1, 0.5f, 5.0f);
        }
    }
}
