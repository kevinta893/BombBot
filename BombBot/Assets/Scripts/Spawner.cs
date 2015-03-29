using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public Server server;

	private float BOMB_PERIOD = 0.0f;

	private float timer;

	private bool paused = true;

	void Start()
	{
	
		timer = BOMB_PERIOD;
		Random.seed = System.DateTime.Now.Millisecond;
		StartSpawner ();
	}

	void PauseSpawner(){
		paused = true;
	}

	void StartSpawner(){
		paused = false;
	}

	/*
	 * 	Generates a new bomb every period
	 */
	void Update()
	{

		if (paused == true) {
			return;
		}


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
	 * 	Add a bomb to server's list of bombs
	 */
	public void AddBomb()
	{	
		// params: int shape, int colour, int solution, float timer
		server.GenerateBomb(Random.Range(0,3),
		                    Random.Range(0,3),
		                    Random.Range (0, 3), 
		                    Random.Range (3, 13));


	}

}
