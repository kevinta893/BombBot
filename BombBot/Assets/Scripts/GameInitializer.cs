using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameInitializer : MonoBehaviour {


	public GameObject gameMenu;						//group of UI objects join and create
	public GameObject joinGameMenu;					//group of UI objects for IP and Connect
	public Text ipText;
	public Text infoText;

	private string ip;
	private int port = 59981;

	// Use this for initialization
	void Start () 
	{
		Object.DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update () 
	{
		// quit current mode
		//if (Input.GetKeyDown(KeyCode.Escape)) 
			//Application.Quit();

	}


	/* Starts the player off as the expert. Loads the server
	 * 
	 */
	public void StartServer()
	{
		Application.LoadLevel("ExpertMode");
	}

	/* Joins a game
	 *
	 */
	public void JoinGame()
	{

		this.ip = ipText.text;

		if (ip.Length > 0) {
			if (Network.peerType == NetworkPeerType.Disconnected) {
				infoText.text = "Connecting...";

				NetworkConnectionError err = Network.Connect (ip, port);

				//await confirmination by event
			}
		}
	}


	//On connection to server
	void OnConnectedToServer() {
		Application.LoadLevel ("ClientMode");
	}

	//Failed connection
	void OnFailedToConnect(NetworkConnectionError error) {
		infoText.text = "Could not connect to server: " + error.ToString();
	}








	/* Shows the GUI for joining a game
	 * 
	 */
	public void ShowJoinGameGUI()
	{
		gameMenu.SetActive (false);
		joinGameMenu.SetActive (true);
	}

	public void ShowGameMenu()
	{
		gameMenu.SetActive (true);
		joinGameMenu.SetActive (false);
		infoText.text = "";
	}














	public void QuitGame() 
	{
		Application.Quit();
	}

	public string GetIP()
	{
		return ip;
	}
}
