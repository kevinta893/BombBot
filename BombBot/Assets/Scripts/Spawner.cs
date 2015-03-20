using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public Server server;

	private float BOMB_PERIOD = 5.0f;

	private int id_count;
	private float timer;

	void Start()
	{
		id_count = 1;
		timer = BOMB_PERIOD;
		Random.seed = System.DateTime.Now.Millisecond;
	}

	/*
	 * 	Generates a new bomb every period
	 */
	void Update()
	{
		timer -= Time.deltaTime;

		// on countdown, spawn bomb and reset timer if connected to network
		if ((Network.peerType != NetworkPeerType.Disconnected) && (timer < 0)) {
			AddBomb();
			timer = BOMB_PERIOD;
		}
		else if (timer < 0) {	// reset timer anyways even if not connected
			timer = BOMB_PERIOD;
		}
	}

	/*
	 * 	Add the bomb to server's list of bombs
	 */
	public void AddBomb()
	{
		server.GenerateBomb(id_count, 
		                    Random.Range(0,3), 
		                    (float) Random.Range(0, 360), 
		                    Random.Range (0, 3), 
		                    Random.Range (3, 13));

		id_count++;
	}

}
