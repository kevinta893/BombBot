using UnityEngine;
using System.Collections;

public class ClientNetwork : MonoBehaviour
{
	public string IP = "192.168.1.67";
	public int port = 59981;
	
	private GUIStyle myStyle;

	private GameInitializer initialParams;
	private int currentBomb;

	public AudioClip explodeSound;
	public AudioClip correctSound;

	public GameOverPanel gameOverPanel;
	public NextRoundUI nextRoundUI;
	void Start ()
	{
		// spawn bombs from client to debug
		/*for (int i =0; i < 65; i++) {
			SpawnBomb (i, i%3,2, i * 15);
		}*/
		//initialParams = (GameInitializer) GameObject.Find ("GameInitializer").GetComponent("GameInitializer");
		//this.IP = initialParams.GetIP ();
		
		currentBomb = -1;
	}

	void Update()
	{
		// quit current mode
		if (Input.GetKeyDown(KeyCode.Escape)) 
			Application.LoadLevel("MainMenu");
	}



	/*
	 * Lost connection, end game.
	 */
	void OnDisconnectedFromServer(NetworkDisconnection info) 
	{
		//Network.Disconnect ();

		gameOverPanel.ShowPanelConnectionLost ();
	}


	void GameOver()
	{
		//TODO Gameover, stop movement, clear all bombs

	}


	//================================================================================
	//To server functions

	private int camThrottle;
	private const int SKIP_SEND = 3;				//set to 1 for no throttle

	/*
	*	Send the camera state to the server
	*/
	public void SendCameraData (Quaternion rotation)
	{
		if (camThrottle == 0) {
			networkView.RPC ("UpdateCamera", RPCMode.Server, rotation);
		}
		camThrottle = (camThrottle + 1) % SKIP_SEND;
	}
	[RPC]
	void UpdateCamera (Quaternion rotation) { }
	

	/*
	*	Send the id of the bomb that the client is currently looking at to the server
	*/
	public void SendCurrentBomb(GameObject bomb) 
	{
		if (bomb != null) 
		{
			// find id of current bomb
			for (int i = 0; i < bombList.Count; i++) {
				BombObject tmp = (BombObject) bombList[i];
				if (tmp.obj == bomb) {
					currentBomb = tmp.id;
					break;
				}
				else {
					currentBomb = -1;
				}
			}
		}
		Debug.Log("Sending current bomb id " + currentBomb);
		networkView.RPC ("CurrentBomb", RPCMode.Server, currentBomb);
	}
	[RPC]
	void CurrentBomb (int id) { }
	
	/*
	*	Send the server the attempted solution for the current bomb
	*/
	public void AttemptDefuse(int solution)
	{
		networkView.RPC ("CheckSolution", RPCMode.Server, currentBomb, solution);
	}
	[RPC]
	void CheckSolution (int id, int solution) { }

	//===============================================================================
	//From server functions

	private const float RADIUS = 2.0f;
	private const float X_OFFSET = 0.0f;
	private const float Z_OFFSET = 0.0f;
	private const float BOMB_HEIGHT = 0.50f;
	public GameObject cubeBombPrefab;
	public GameObject sphereBombPrefab;
	public GameObject tetraBombPrefab;
	ArrayList bombList = new ArrayList (Server.MAX_BOMBS);
		

	/* Spawn a bomb to the play field
	 * 
	 * @param id Should be unique
	 * @param shape 0 for square, 1 for circle, 2 for tetrahedron
	 * @param colour 0 for red, 1 for blue, 2 for green
	 * @param degreesLoc a value between 0.0 - 360.0f indicating degree location on unit circle
	 */
	[RPC]
	void SpawnBomb (int id, int shape, int colour, float degreesLoc)
	{
		GameObject selection = null;
		Color bombColour = Color.clear;

		switch (shape) {
		case 0:
			selection = cubeBombPrefab;
			break;
		case 1:
			selection = sphereBombPrefab;
			break;
		case 2:
			selection = tetraBombPrefab;			
			break;
		}

		switch (colour) {
		case 0:
			bombColour = Color.red;
			break;
		case 1:
			bombColour = Color.blue;
			break;
		case 2:
			bombColour = Color.green;
			break;
		}

		float radians = degreesLoc * (Mathf.PI / 180);
		Vector3 position = new Vector3 (Mathf.Cos (radians), BOMB_HEIGHT, Mathf.Sin (radians)) * RADIUS;		//a unit vector times radius

		// generate bomb with proper shape and colour
		GameObject obj = (GameObject) Instantiate (selection, position, Quaternion.identity);
		obj.renderer.material.color = bombColour;

		BombObject bomb = new BombObject(id, degreesLoc, obj);
		bombList.Add(bomb);
	}


	/*
	* Remove a bomb from the game field, either safely or exlpodedely.
	*
	* @param id, 	int id of the bomb to remove
	* @param safe, 	bool indicating result of defusion
 	* 				true: success
 	* 				false: explosion
 	*/
	[RPC]
	void DestroyBomb (int id, bool safe)
	{
		BombObject cursor = null;

		// find the bomb iteratively
		for (int i = 0; i < bombList.Count; i++) {
			cursor = (BombObject) bombList[i];

			if (cursor.id == id) {
				if (safe) {
					Debug.Log ("Destroying Bomb #" + i + "safely");
					AudioSource.PlayClipAtPoint(correctSound, cursor.obj.transform.position);
					Destroy(cursor.obj);
					bombList.RemoveAt(i);
				} else {
					Debug.Log ("Destroying Bomb #" + i + "explodedely");
					// EXPLODE animation
					AudioSource.PlayClipAtPoint(explodeSound, cursor.obj.transform.position);
					Destroy(cursor.obj);

					bombList.RemoveAt(i);
				}
				break;
			}
		}
	}


	[RPC]
	public void SendGameOver(int finalScore)
	{

		gameOverPanel.ShowPanel (finalScore);
		GameOver ();
	}


	
	[RPC]
	public void SendEndLevel(int levelCompleted)
	{
		nextRoundUI.ShowPanel (levelCompleted);
	}
	
	[RPC]
	public void SendStartNewLevel(int level)
	{
		nextRoundUI.HidePanel ();
	}

}
