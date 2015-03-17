using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Server : MonoBehaviour
{
	public const int LISTEN_PORT = 59981;
	private const int MAX_CLIENTS = 2;					//max clients including expert themselves
	private const bool USE_NAT = false;					//enable nat punchtrough for reachable outside internet

	private Spawner spawn;
	private Quaternion cameraRotation;

	public Text infoText;
	public Text debugText;
	public Text startServerButtonText;
	public Image indicator;

	private Vector3 CENTER_RADAR; //= new Vector3(-181.4f, -14.6f, 0.0f);
	private const float RADIUS = 100.0f;



	// Use this for initialization
	void Start ()
	{
		spawn = new Spawner(this);
		CENTER_RADAR = indicator.transform.position;
		Debug.Log(CENTER_RADAR);
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
						
			// calculate indicator position based on rotation
			Vector3 direction = GetForwardVector(cameraRotation);


			direction.y = 0;			//ignore vertial displacement
			direction.Normalize();


			//swap z and y components
			float temp = direction.y;
			direction.y = direction.z;
			direction.z = temp;


			indicator.transform.position= CENTER_RADAR + (direction*RADIUS);
		}
		
		
	}


	//Taken from http://nic-gamedev.blogspot.ca/2011/11/quaternion-math-getting-local-axis.html?m=1
	Vector3 GetForwardVector(Quaternion q) 
	{
		return new Vector3( 2 * (q.x * q.z + q.w * q.y), 
		            2 * (q.y * q.x - q.w * q.x),
		            1 - 2 * (q.x * q.x + q.y * q.y));
	}
	/*void OnGUI() 
	{
		if (Network.peerType == NetworkPeerType.Disconnected) 
		{
			if (GUILayout.Button("Start Server", GUILayout.Height(100)))
			{
				InitializeServer ();
			}
		}
		else 
		{
			if(Network.peerType == NetworkPeerType.Server)
			{
				GUILayout.Label("Server");
				GUILayout.Label("My IP: " + Network.player.ipAddress);
				GUILayout.Label("Connections: " + Network.connections.Length);
				GUILayout.Label("Rotation: " + cameraRotation);
				
				if(GUILayout.Button("Logout"))
				{
					Network.Disconnect(250);
				}
			}


		}
	}*/


	// TODO: UNUSED? What do we do with this?
	bool ConnectToServer (string ip)
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			NetworkConnectionError err = Network.Connect (ip, LISTEN_PORT);
			//check if error on initalize
			if (err != NetworkConnectionError.NoError)
			{
				Debug.Log (err.ToString ());
				return false;
			}
			return true;
					

		}
		return false;
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
	public bool InitializeServer ()
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
		}
		return true;
	}

	public bool StopServer ()
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

	public void SpawnBomb(int id, int type, float degrees) {
		networkView.RPC ("GenerateBomb", RPCMode.All, id, type, degrees);
	}

	// temp method for debug only
	public void ButtonAddBomb ()
	{
		// right now spawn actually creates a random bomb. change later
		spawn.AddBomb(1, 1, 0, 1);
	}
	
	[RPC]
	public void GenerateBomb (int id, int type, float degrees)
	{
		// blank RPC is weird
	}

}
