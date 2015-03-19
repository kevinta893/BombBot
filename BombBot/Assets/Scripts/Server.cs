using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Server : MonoBehaviour
{
	public const int LISTEN_PORT = 59981;
	private const int MAX_CLIENTS = 2;					//max clients including expert themselves
	private const bool USE_NAT = false;					//enable nat punchtrough for reachable outside internet
	public static int MAX_BOMBS = 20;

	private Quaternion cameraRotation;

	public Text infoText;
	public Text debugText;
	public Text startServerButtonText;
	public Image indicator;

	private Vector3 CENTER_RADAR; 
	private const float RADIUS = 100.0f;

	// Use this for initialization
	void Start ()
	{
		CENTER_RADAR = indicator.transform.position;
		UpdateRadar (Quaternion.identity);

		InitializeServer ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			infoText.text = "Server disconnected.";
		} 
		else if (Network.peerType == NetworkPeerType.Server) 
		{
			infoText.text = "Server connected." +
				"\nConnections: " + Network.connections.Length +
				"\nMy IP: " + Network.player.ipAddress;
				
			debugText.text = "Rotation: " + cameraRotation +
							"\nIndicator" + indicator.transform.position;
					
			UpdateRadar (cameraRotation);

		}
		
		
	}
	void UpdateRadar(Quaternion camRot){
		// calculate indicator position based on rotation
		Vector3 direction = GetForwardVector(camRot);
		
		
		direction.y = 0;			//ignore vertial displacement
		direction.Normalize();
		
		
		//swap z and y components
		float temp = direction.y;
		direction.y = direction.z;
		direction.z = temp;
		
		
		indicator.transform.position= CENTER_RADAR + (direction*RADIUS);
	}

	//Taken from http://nic-gamedev.blogspot.ca/2011/11/quaternion-math-getting-local-axis.html?m=1
	Vector3 GetForwardVector(Quaternion q) 
	{
		return new Vector3( 2 * (q.x * q.z + q.w * q.y), 
		            2 * (q.y * q.x - q.w * q.x),
		            1 - 2 * (q.x * q.x + q.y * q.y));
	}

	public void ToggleServer() 
	{
		
		if (Network.peerType == NetworkPeerType.Disconnected) 
		{	// start server if not on
			if (InitializeServer())
				startServerButtonText.text = "Disconnect";


		} 
		else if (Network.peerType == NetworkPeerType.Server) 
		{	// stop server if on
			if (StopServer())
				startServerButtonText.text = "Start Server";
		}

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

	public bool StopServer()
	{
		if (Network.peerType == NetworkPeerType.Server) {
			Network.Disconnect (250);
			return true;
		}
		return false;
	}



	//====================================================
	//From client methods

	[RPC]
	void UpdateCamera(Quaternion rotation)
	{
		cameraRotation = rotation;
	}


	//====================================================
	//To Client methods


	ArrayList bombList = new ArrayList (Server.MAX_BOMBS);

	/*
	 * 	Generates a bomb entity and update own list
	 * 	Then spawn the bomb on the client side
	 */
	public void GenerateBomb(int id, int type, float degrees, int solution, float timer) 
	{

		BombEntity bomb = new BombEntity(id, type, degrees, solution, timer);
		bombList.Add(bomb);

		Debug.Log("Bomb Spawn ID:" + bomb.id + 
		          " Type:" + bomb.type + 
		          " Degrees:" + bomb.degrees +
		          " Solution:" + bomb.solution + 
		          " Timer:" + bomb.timer);
		
		networkView.RPC ("SpawnBomb", RPCMode.All, id, type, degrees);
	}


	// temp method for debug only
	public void ButtonAddBomb ()
	{
		// right now spawn actually creates a random bomb. change later
		//spawn.AddBomb(1, 1, 0, 1);
	}
	
	[RPC]	// blank RPC method on client
	public void SpawnBomb (int id, int type, float degrees) { }


}
