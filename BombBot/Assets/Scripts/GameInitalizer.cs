using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameInitalizer : MonoBehaviour {


	public GameObject gameMenu;						//group of UI objects join and create
	public GameObject joinGameMenu;					//group of UI objects for IP and Connect
	public Text ipText;

	private string ip;

	// Use this for initialization
	void Start () {
		Object.DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update () {

	}


	/* Starts the player off as the expert. Loads the server
	 * 
	 */
	public void StartServer(){
		Application.LoadLevel("ExpertMode");
	}

	/* Joins a game
	 *
	 */
	public void JoinGame(){
		this.ip = ipText.text;
		Application.LoadLevel("ClientMode");
	}


	/* Shows the GUI for joining a game
	 * 
	 */
	public void ShowJoinGameGUI(){
		gameMenu.SetActive (false);
		joinGameMenu.SetActive (true);
	}

	public void ShowGameMenu(){
		gameMenu.SetActive (true);
		joinGameMenu.SetActive (false);
	}

	public string GetIP(){
		return ip;
	}
}
