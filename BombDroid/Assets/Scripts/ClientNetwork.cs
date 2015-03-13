using UnityEngine;
using System.Collections;

public class ClientNetwork : MonoBehaviour
{
	
		public string IP = "192.168.1.67";
		public int Port = 59981;
		public GameObject camera;
		private GUIStyle myStyle;
	
		void Start ()
		{

		/*
				for (int i =0; i < (360/10); i++) {
					AddBomb (i, i % 2, i * 15+ 12);
				}

				for (int i =0; i < (360/10); i++) {
					RemoveBomb (i);
				}
				*/
		}
	
		void OnGUI ()
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
			
				} else {
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
	
		[RPC]
		void UpdateCamera (Quaternion rotation)
		{
				camera.transform.rotation = rotation;
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
		ArrayList bombList = new ArrayList (20);

		struct BombObject
		{
				public int id;
				public int type;
				public float radians;
				public GameObject obj;
		}






		/* Adds a bomb to the play field
	 * @param id Should be unique
	 * @param type 0 for square, 1 for circle, 2 for tetrahedron
	 * @param degreesLoc a value between 0.0 - 360.0f indicating degree location on unit circle
	 */
		[RPC]
		void AddBomb (int id, int type, float degreesLoc)
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
		
				BombObject newBomb = new BombObject ();
				newBomb.id = id;
				newBomb.type = type;
				newBomb.radians = radians;
				newBomb.obj = (GameObject) Instantiate (selection, position, Quaternion.identity);
				bombList.Add (newBomb);
		}

		[RPC]
		void RemoveBomb (int id)
		{

				for (int i =0; i < bombList.Count; i++) {
						BombObject cursor = (BombObject) bombList[i];

						if (cursor.id == id) {
								Destroy (cursor.obj);
						}
				}
		}

}
