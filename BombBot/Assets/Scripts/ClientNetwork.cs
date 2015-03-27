using UnityEngine;
using System.Collections;

public class ClientNetwork : MonoBehaviour
{
	public string IP = "192.168.1.67";
	public int port = 59981;
	
	private GUIStyle myStyle;

	private GameInitializer initialParams;


	void Start ()
	{
		// spawn bombs from client to debug
		/*for (int i =0; i < (360/10); i++) {
			SpawnBomb (i, i%3, i * 15);
		}*/
		initialParams = (GameInitializer) GameObject.Find ("GameInitializer").GetComponent("GameInitializer");
		this.IP = initialParams.GetIP ();
	}

	void Update()
	{
		// quit current mode
		if (Input.GetKeyDown(KeyCode.Escape)) 
			Application.LoadLevel("MainMenu");
	}

	/*void OnGUI ()
	{
		myStyle = new GUIStyle (GUI.skin.textArea);
		myStyle.fontSize = 40;

		if (Network.peerType == NetworkPeerType.Disconnected) {

			if (IP.Equals ("") == true) {
				IP = GUI.TextArea (new Rect (1000, 75, 300, 100), "192.168.1.67", myStyle);
			} else {
				IP = GUI.TextArea (new Rect (1000, 75, 300, 100), IP, myStyle);
			}

			if (GUI.Button (new Rect (1000, 200, 300, 100), "Connect to Expert", myStyle)) {
				Network.Connect (IP, Port);
			}
			//Network.Connect (IP, port);
		} 
		else {
			if (Network.peerType == NetworkPeerType.Client) {
				if (GUI.Button (new Rect (1000, 200, 300, 100), "Logout")) {
					Network.Disconnect (250);	
				}
			}
		}
	}*/





	//================================================================================
	//To server functions
	public void SendCameraData (Quaternion rotation)
	{
		networkView.RPC ("UpdateCamera", RPCMode.Server, rotation);
	}
	[RPC]
	void UpdateCamera (Quaternion rotation)
	{
		//blank RPC
	}
	

	public void SendBombSolution(int id, int solution){
		//networkView.RPC ("UpdateCamera", RPCMode.Server, rotation);
	}

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
		int i;

		// find the bomb iteratively
		for (i = 0; i < bombList.Count; i++) {
			cursor = (BombObject) bombList[i];

			if (cursor.id == id) {
				break;
			}
		}

		if (safe) {
			Destroy(cursor.obj);
			bombList.RemoveAt(i);
		} else {
			// EXPLODE animation
			Destroy(cursor.obj);
			bombList.RemoveAt(i);
		}
	}

}
