using UnityEngine;
using System.Collections;

public class ClientNetwork : MonoBehaviour
{
	
	public string IP = "192.168.1.67";
	public int Port = 59981;
	
	private GUIStyle myStyle;

	void Start ()
	{
		// spawn bombs from client to debug
		/*for (int i =0; i < (360/10); i++) {
			GenerateBomb (i, i % 2, i * 15+ 12);
		}*/
		
	}

	void OnGUI ()
	{
		myStyle = new GUIStyle (GUI.skin.textArea);
		myStyle.fontSize = 40;

		// TODO: move this server connect to another scene or panel?
		if (Network.peerType == NetworkPeerType.Disconnected) {
			if (IP.Equals ("") == true) {
				IP = GUI.TextArea (new Rect (1000, 75, 300, 100), "192.168.1.67", myStyle);
			} else {
				IP = GUI.TextArea (new Rect (1000, 75, 300, 100), IP, myStyle);
			}

			if (GUI.Button (new Rect (1000, 200, 300, 100), "Connect to Expert", myStyle)) {
				Network.Connect (IP, Port);
			}
		} 
		else {
			if (Network.peerType == NetworkPeerType.Client) {
				if (GUI.Button (new Rect (1000, 200, 300, 100), "Logout")) {
					Network.Disconnect (250);	
				}
			}
		}
	}





	//================================================================================
	//To server functions
	public void SendCameraData (Quaternion rotation)
	{
		networkView.RPC ("UpdateCamera", RPCMode.Server, rotation);
	}

	/*	Only need this function if we want to move the client camera from server
	 * 	I don't think we want that.
	[RPC]
	void UpdateCameraFromServer (Quaternion rotation)
	{
		camera.transform.rotation = rotation;
	}
	*/


	//===============================================================================
	//From server functions

	private const float RADIUS = 2.0f;
	private const float X_OFFSET = 0.0f;
	private const float Z_OFFSET = 0.0f;
	private const float BOMB_HEIGHT = 0.50f;
	public GameObject cubeBombPrefab;
	public GameObject sphereBombPrefab;
	public GameObject tetraBombPrefab;
	ArrayList bombList = new ArrayList (20);
		

	/* Generate a bomb to the play field
	 * 
	 * @param id Should be unique
	 * @param type 0 for square, 1 for circle, 2 for tetrahedron
	 * @param degreesLoc a value between 0.0 - 360.0f indicating degree location on unit circle
	 */
	[RPC]
	void GenerateBomb (int id, int type, float degreesLoc)
	{
		GameObject selection = null;
		switch (type) {
		case 0:
			selection = cubeBombPrefab;
			break;
		case 1:
			selection = sphereBombPrefab;
			break;
		case 2:
			selection = cubeBombPrefab;			//TODO MAKE TRIAGULAR bomb
			break;
		}

		float radians = degreesLoc * (Mathf.PI / 180);
		Vector3 position = new Vector3 (Mathf.Cos (radians), BOMB_HEIGHT, Mathf.Sin (radians)) * RADIUS;		//a unit vector times radius

		GameObject obj = (GameObject) Instantiate (selection, position, Quaternion.identity);
		BombObject bomb = new BombObject(id, type, degreesLoc, obj);
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
