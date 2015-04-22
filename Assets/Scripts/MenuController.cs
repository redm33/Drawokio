using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
    public static MenuController instance;

    public GameObject mainMenu;
    public GameObject pauseMenu;

    private MenuUIStates _state;

    public MenuUIStates state {
        get { return _state; }
        set { 
            switch(value) {
                case MenuUIStates.NONE:
                    Debug.Log("set menu none");
                    mainMenu.SetActive(false);
                    pauseMenu.SetActive(false);
                    break;
                case MenuUIStates.MAIN:
                    Debug.Log("set menu main");
                    mainMenu.SetActive(true);
                    pauseMenu.SetActive(false);
                    break;
                case MenuUIStates.PAUSE:
                    Debug.Log("drew pause menu");
                    mainMenu.SetActive(false);
                    pauseMenu.SetActive(true);
                    break;
            }
            _state = value;
        }
    }

    public enum MenuUIStates {
        NONE,
        MAIN,
        OPTIONS,
        CREDITS,
        PAUSE,
        QUIT
    };

	// Use this for initialization
	void Awake () {
        instance = this;
	}
    public void ClosePauseMenu() {
        Player.instance.paused = false;
        Room.instance.state = Room.State.PLAYING;
    }
    public void ReturnToMain() {
        Room.instance.state = Room.State.MENU_MAIN;
        Player.instance.GetComponent<PlayerDrivingController>().Jump();
    }

}
