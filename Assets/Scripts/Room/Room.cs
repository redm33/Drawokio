using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {
    public static Room instance;
    public Drawer drawer;
    public Player playerPrefab;
    //public SpawnPoint[] spawnPoints;
    public MovieTexture openingCutscene;
    public bool playOpening;
    public bool onlyOnce = true;
    //public GameObject[] disableDuringMenuObjects; //Objects to disable during the main menu
    //public GameObject[] menuGameObjects; //Menu text objects
    //public GameObject menuUI;

    public CameraController cameraController;
    public CameraFollowable[] cameraFollowables;
    public float chooseCameraSpinSpeed = 10;
    public Resettable[] resettables; // Items that reset on level reset
    //public Resettable[] discoverableAreas; // The discoverable zones in the room.
    public Resettable[] collectables; // The collectable items that go towards game progress. eg eight ball pieces.
    private List<Transform> carriedItems; // Items carried by and useable by the player.
    private int equippedItem; // The currently equipped useable item.
    public Pickup pencilPickup, charcoalPickup; // The drawing material pickups.
    public GameObject deathText; // The death message.
    float quitStart = 0;// The quit start.
    //bool firstDrag = true;
    Vector3 dragStart;
    //private int completionPercentage;

    public Transform GetEquippedItem() {
        if(equippedItem == -1 || equippedItem >= carriedItems.Count) {
            return null;
        } else {
            return carriedItems[equippedItem];
        }
    }

    public int GetCompletionPercentage() {
        int found = 0, total = 0;

        foreach(Resettable obj in resettables) {
            if(obj.isProgress) {
                total++;
                if(obj.pickedUp)
                    found++;
            }
        }

        return 100 * found / total;
    }

    public enum State { //The game states
        NONE,
        MENU_MAIN, //This is the state for the main menu and then if it is selected again it will start the game
        MENU_STARTING, //Implementing this state as you want to start the game
        MENU_CONTINUE, //This is the state it will enter when you continue
        MENU_OPTIONS, // The quite menu?
        MENU_PAUSED, //Pause menu
        MENU_QUIT, //Quit menu state
        PLAYING, //Playing the game state
        QUITTING
    }



    /// <summary>
    /// 
    /// </summary>
    public void SpawnAtLatestSpawn() {
        if(Player.instance != null) {
            Debug.LogWarning("Don't spawn more than one player!");
            return;
        }

        Continue();
        /*spawnPoint point = spawnPoints[0];
        for( int i = 1; i < spawnPoints.Length; i++ )
            if( spawnPoints[i].isLatestSpawn )
                point = spawnPoints[i];
        StartAt(point);*/
    }

    /// <summary>
    /// Sets the state of the game.
    /// </summary>
    /// <param name="state">New state of the game</param>
    public void SetState(int state) {
        this.state = (State)state;
    }
    /// <summary>
    /// The pointer to the game state object.
    /// </summary>
    private State _state = State.NONE;
    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    /// <value>The state.</value>
    public State state {
        get { return _state; }
        set {

            if(value != State.MENU_PAUSED) {
                //Performs any pre state setting tasks
                switch(_state) {
                    case State.PLAYING:
                        if(value != State.PLAYING) { DestroyAll(); }
                        break;
                    default:
                        break;
                }
            }

            if(value == State.MENU_PAUSED && _state != State.PLAYING) {
                Debug.Log("Tried to pause outside the menu");
            } else {
                //Sets the state
                _state = value;
                
                //Performs any post state setting tasks
                switch(_state) {
                    case State.MENU_STARTING:
                        onlyOnce = true;
                        playOpening = true;
                        MenuController.instance.state = MenuController.MenuUIStates.NONE;
                        break;
                    case State.MENU_CONTINUE:
                        MenuController.instance.state = MenuController.MenuUIStates.NONE;
                        Continue();
                        break;
                    case State.PLAYING:
                        MenuController.instance.state = MenuController.MenuUIStates.NONE;
                        drawer.gameObject.SetActive(true);
                        break;
                    case State.QUITTING:
                        quitStart = Time.time;
                        break;
                    case State.MENU_MAIN:
                        MenuController.instance.state = MenuController.MenuUIStates.MAIN;
                        break;
                    case State.MENU_PAUSED:
                        MenuController.instance.state = MenuController.MenuUIStates.PAUSE;
                        break;
                    default:
                        break;
                }
            }
            //cameraController.ClearQueue();
            //if((int)_state < cameraFollowables.Length)
            //    cameraController.AddToQueue(cameraFollowables[(int)_state]);
        }
    }
    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake() {
        instance = this;

        carriedItems = new List<Transform>();
        equippedItem = -1;

        SpawnPointController.instance.SetSpawnIndex();

        for(int i = 0; i < resettables.Length; i++) {
            resettables[i].index = i;
        }

        DiscoverableAreasController.instance.SetAreaIndex();

        for(int i = 0; i < collectables.Length; i++) {
            collectables[i].index = i;
        }
    }
    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start() {
        state = State.MENU_MAIN;
    }
    /// <summary>
    /// Destroys all.
    /// </summary>
    public void DestroyAll() {

        if(Player.instance != null) {
            GameObject.Find("Main Camera").transform.parent = Player.instance.transform.parent;
            GameObject.Find("Main Camera").GetComponent<ThirdPersonCamera>().enabled = false;
            Destroy(Player.instance.gameObject);
        }
        drawer.DestroyAll();
        drawer.gameObject.SetActive(false);
        paused = false;
        Drawer.instance.uiOpen = false;
    }

    void Update() {
        switch(state) {
            case State.QUITTING:
                if(Time.time - quitStart > 1.0f)
                    Application.Quit();
                break;
            case State.MENU_CONTINUE:
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
            //case State.MENU_MAIN:
            case State.MENU_QUIT:
                if(Input.GetMouseButtonDown(0)) {
                    RaycastHit hit;
                    if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, (1 << MenuButton.layer))) {

                        MenuButton button = hit.collider.GetComponent<MenuButton>();
                        if(button == null) {
                            Debug.LogWarning("Hit something that wasn't a MenuButton!");
                            return;
                        }

                        state = button.state;
                    }
                }
                break;
            case State.MENU_OPTIONS:
                if(Input.GetMouseButtonDown(0) || Input.GetButtonDown("Back")) {
                    state = State.MENU_MAIN;
                    Player.instance.GetComponent<PlayerDrivingController>().Jump();
                }
                break;
            case State.PLAYING:
                if(Player.instance == null && !EndingController.instance.running) {
                    //deathText.SetActive(true);

                    //if( Input.GetButtonDown( "Back" ) ) 
                    //{
                    DestroyAll();
                    SpawnAtLatestSpawn();
                    //}
                } else
                    deathText.SetActive(false);
                break;
        }
    }



    void Continue() {
        StartAt(SpawnPointController.instance.GetSpawnPoint(PlayerPrefs.GetInt("SpawnPoint", 0)));

        //Loading the resettables
        string pickupsStr = PlayerPrefs.GetString("Pickups", "");
        for(int i = 0; i < pickupsStr.Length; i++) {
            int index = (int)pickupsStr[i];
            resettables[index - 33].ForcePickup();
        }

        //Loading the discoverable areas
        string areasStr = PlayerPrefs.GetString("Areas", "");
        for(int i = 0; i < areasStr.Length; i++) {
            int index = (int)areasStr[i];
            DiscoverableAreasController.instance.GetDiscoverableArea(index - 33).ForcePickup();
        }

        //Loading the collectables
        string collectStr = PlayerPrefs.GetString("Collectables", "");
        for(int i = 0; i < collectStr.Length; i++) {
            int index = (int)collectStr[i];
            collectables[index - 33].ForcePickup();
        }

        //Loading the hat
        int hat = PlayerPrefs.GetInt("Hat", -1);
        if(hat >= 0) {
            SetHat(hat);
            Player.instance.SetHat(hat);
        }
    }

    public bool Save(int spawnPoint) {
        bool changed = false;

        if(PlayerPrefs.GetInt("SpawnPoint", 0) != spawnPoint)
            changed = true;
        PlayerPrefs.SetInt("SpawnPoint", spawnPoint);

        if(!changed && PlayerPrefs.GetInt("Hat", 0) != currentHat)
            changed = true;
        PlayerPrefs.SetInt("Hat", currentHat);

        //Saving the resettables
        string pickupsStr = "";
        for(int i = 0; i < resettables.Length; i++) {
            if(resettables[i].pickedUp)
                pickupsStr += (char)(i + 33);
        }

        if(!changed && PlayerPrefs.GetString("Pickups", "") != pickupsStr)
            changed = true;

        PlayerPrefs.SetString("Pickups", pickupsStr);

        //Saving the discoverable areas
        string areasStr = "";
        for(int i = 0; i < DiscoverableAreasController.instance.GetNumAreas(); i++) {
            if(DiscoverableAreasController.instance.GetDiscoverableArea(i).pickedUp)
                areasStr += (char)(i + 33);
        }

        if(!changed && PlayerPrefs.GetString("Areas", "") != areasStr)
            changed = true;

        PlayerPrefs.SetString("Areas", areasStr);

        //Saving the collectables
        string collectStr = "";
        for(int i = 0; i < collectables.Length; i++) {
            if(collectables[i].pickedUp)
                collectStr += (char)(i + 33);
        }

        if(!changed && PlayerPrefs.GetString("Collectables", "") != collectStr)
            changed = true;

        PlayerPrefs.SetString("Collectables", collectStr);



        return changed;
    }

    void StartAt(SpawnPoint point) {
        Debug.Log("Spawning");
        SpawnPointController.instance.ResetSpawnPoints();
        SpawnPointController.instance.SetLatestPoint(PlayerPrefs.GetInt("SpawnPoint", 0));

        Player player = Instantiate(playerPrefab, point.transform.position, point.transform.rotation) as Player;
        player.name = "Player";

        if(point.spawnIn3D) {
            player.transformationController.Become3D();
            player.transform.Find("DissolveParticles").gameObject.SetActive(true);
            player.transform.Find("Blob Shadow Projector").gameObject.SetActive(true);
        } else {
            player.movementController.ApplyLockType(point.lockType);
        }
        SetHat(-1);

        drawer.gameObject.SetActive(true);

        Drawer.instance.hasPencil = Drawer.instance.hasCharcoal = false;

        foreach(Resettable resettable in resettables) {
            if(resettable != null)
                resettable.PerformReset();
        }

        state = State.PLAYING;
    }

    public GameObject[] endHats;
    int currentHat = -1;

    public void SetHat(int index) {
        if(currentHat >= 0)
            endHats[currentHat].SetActive(false);

        currentHat = index;

        if(index >= 0 && index < endHats.Length)
            endHats[index].SetActive(true);
    }

    private bool _paused = false;
    public bool paused {
        get {
            return _paused;
        }
        set {
            if(_paused == value)
                return;

            _paused = value;

            foreach(Resettable resettable in resettables) {
                if(resettable != null)
                    resettable.paused = value;
            }

            Player.instance.paused = value;

            foreach(Transform child in Drawer.instance.drawingParent) {
                child.GetComponent<Ink>().paused = value;
            }
        }
    }

    void OnGUI() {
        if(playOpening) {
            if(openingCutscene != null) {
                openingCutscene.Play();
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), openingCutscene, ScaleMode.StretchToFill, false, 0.0f);

                if(onlyOnce) {
                    StartCoroutine(StopVideo());
                    onlyOnce = false;
                }
            } else {
                SkipStopVideo();
            }

            if(Input.GetKeyDown(KeyCode.Space)) {
                SkipStopVideo();
            }
        }
    }

    IEnumerator StopVideo() {
        yield return new WaitForSeconds(50f);
        SkipStopVideo();
    }

    void SkipStopVideo() {
        if(Player.instance == null) {
            playOpening = false;
            onlyOnce = true;
            if(openingCutscene != null) {
                openingCutscene.Stop();
            }
            PlayerPrefs.SetInt("SpawnPoint", 0);
            PlayerPrefs.SetString("Pickups", "");
            PlayerPrefs.SetInt("Hat", -1);
            StartAt(SpawnPointController.instance.GetSpawnPoint(0));
            //PopupController.QueuePopup(0, 0.5f, 5.0f);
            //PopupController.QueuePopup(1, 0.5f, 5.0f);
        }
    }
}
