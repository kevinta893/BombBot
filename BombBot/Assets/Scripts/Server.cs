using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Server : MonoBehaviour
{
	public const int LISTEN_PORT = 59981;
	private const int MAX_CLIENTS = 2;					//max clients including expert themselves
	private const bool USE_NAT = false;					//enable nat punchtrough for reachable outside internet
	public static int MAX_BOMBS = 20;

	public Text debugText;
	public Image indicator;
	public BombManager bm;
	public Canvas canvas;
	public Image circle;

	private Quaternion cameraRotation;
	private Vector3 CENTER_RADAR; 
	private float INDICATOR_RADIUS;

	//private RuntimePlatform platform = Application.platform;

	// Use this for initialization
	void Start ()
	{
		INDICATOR_RADIUS = canvas.scaleFactor * circle.rectTransform.rect.height* 0.25f;
		Debug.Log (circle.rectTransform.rect.height);
		CENTER_RADAR = indicator.transform.position;
		UpdateRadar (Quaternion.identity);

		Random.seed = System.DateTime.Now.Millisecond;				//set random seed

		InitializeServer ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			debugText.text = "Server disconnected.";
		} 
		else if (Network.peerType == NetworkPeerType.Server) 
		{
			debugText.text = "Server connected." +
				"\nConnections: " + Network.connections.Length +
				"\nMy IP: " + Network.player.ipAddress +
				"\n Rotation: " + cameraRotation +
				"\nIndicator" + indicator.transform.position +
				"\nCurrent: " + bm.currentBomb;
					
			UpdateRadar (cameraRotation);
		}

		// quit current mode
		if (Input.GetKeyDown(KeyCode.Escape)) 
			Application.LoadLevel("MainMenu");
	}



	/*
	 * Updates direction indicator of BombBot
	 */
	void UpdateRadar(Quaternion camRot)
	{
		// calculate indicator position based on rotation
		Vector3 direction = GetForwardVector(camRot);
		
		direction.y = 0;			//ignore vertial displacement
		direction.Normalize();
		
		//swap z and y components
		float temp = direction.y;
		direction.y = direction.z;
		direction.z = temp;

		Vector3 pos= CENTER_RADAR + (direction * INDICATOR_RADIUS);

		indicator.rectTransform.position = new Vector2 (pos.x, pos.y);
	}

	//Taken from http://nic-gamedev.blogspot.ca/2011/11/quaternion-math-getting-local-axis.html?m=1
	Vector3 GetForwardVector(Quaternion q) 
	{
		return new Vector3( 2 * (q.x * q.z + q.w * q.y), 
		            2 * (q.y * q.x - q.w * q.x),
		            1 - 2 * (q.x * q.x + q.y * q.y));
	}

	//attempt to intialize server, returns true if server started, false otherwise
	public bool InitializeServer()
	{
		//make only on no connection
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			NetworkConnectionError err = Network.InitializeServer (MAX_CLIENTS, LISTEN_PORT, false);
					
			Debug.Log (Network.player.ipAddress);
			//check if error on initalize
			if (err != NetworkConnectionError.NoError)
			{
				Debug.Log (err.ToString ());
				debugText.text = "Failed to connect.";
				return false;
			}
			return true;
		}
		return false;
	}



	public bool HasPlayer() { return Network.connections.Length >= 1; }


	//====================================================
	//From client methods

	/*
	* 	Receive the BombBot's rotation direction live.
	*/
	[RPC]
	void UpdateCamera(Quaternion rotation)
	{
		cameraRotation = rotation;
	}
	
	
	/*
	*	Receive the current bomb that BombBot is looking at
	*/
	[RPC]
	void CurrentBomb (int id) 
	{ 
		bm.currentBomb = id;
	}
	
	/*
	*	Receive an attempted solution from BombBot
	*/
	[RPC]
	void CheckSolution (int id, int solution) 
	{ 
		bool success = bm.VerifySolution(id, solution);
		
		if (success) {
			Debug.Log ("Defused bomb " + id);
			networkView.RPC ("DestroyBomb", RPCMode.All, id, true);
		}
		else {	// bomb go boom!
			Debug.Log ("Detonating (from expiry) bomb " + id);
			networkView.RPC ("DestroyBomb", RPCMode.All, id, false);
		}
	}


	//====================================================
	//To Client methods


	/*
	 * 	Bomb manager has generated a bomb,
	 * 	spawn the bomb on the client side
	 */
	public void GenerateBomb(BombEntity bomb) 
	{
		Debug.Log("Bomb Spawn ID:" + bomb.id + 
		          " Shape:" + bomb.shape + 
		          " Colour:" + bomb.colour +
		          " Solution:" + bomb.solution +
		          " Degrees:" + bomb.degrees +
		          " Timer:" + bomb.timer);

		// draw bomb on overview
		if (bomb.position == -1) {
			Debug.LogError ("Error, too many bombs on scene, position returned: " + bomb.position);
		}
		networkView.RPC ("SpawnBomb", RPCMode.All, bomb.id, bomb.shape, bomb.colour, bomb.degrees);
		
	}
	[RPC]	// blank RPC method on client
	public void SpawnBomb (int id, int shape, int colour, float degrees) { }

	
	/*
	*	Bomb explodes from timer expiry
	*/
	public void DetonateBomb(int id)
	{
		Debug.Log ("Detonating (from expiry) bomb " + id);
		networkView.RPC ("DestroyBomb", RPCMode.All, id, false);
	}
	
	[RPC]	// blank RPC method on client
	public void DestroyBomb (int id, bool safety) { }
	
	
	
	
	
	
	
	
	
	
	
}