using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour
{
	

	public const int LISTEN_PORT = 59981;
	private const int MAX_CLIENTS = 2;					//max clients including expert themselves
	private const bool USE_NAT = false;					//enable nat punchtrough for reachable outside internet





	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnGUI() 
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
				
				if(GUILayout.Button("Logout"))
				{
					Network.Disconnect(250);
				}
			}


		}
	}

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
				return false;
			}
			return true;
					
		}

		return false;
				
	}

	public bool StopServer ()
	{
		Network.Disconnect (250);
		return true;
	}
}
