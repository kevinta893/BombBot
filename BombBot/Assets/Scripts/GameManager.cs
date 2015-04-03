using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	
	//Easy bomb constants
	private const int EASY_TIMER_MAX = 45;
	private const int EASY_TIMER_MIN = 30;
	
	//Medium bomb constants
	private const int MED_TIMER_MAX = 30;
	private const int MED_TIMER_MIN = 20;
	
	//hard bomb constants
	private const int HARD_TIMER_MAX = 20;
	private const int HARD_TIMER_MIN = 15;
	

	public Countdown countdown;
	public WaitingPlayerText waitingText;
	public GameObject gameOverText;


	public Server server;

	private int bombsDefused = 0;
	private int lives = 3;
	
	private const float SPAWN_INTERVAL = 5.0f;
	private float spawnTimer = SPAWN_INTERVAL;
	
	private bool spawnPause = true;
	private bool gameover = false;


	// Use this for initialization
	void Start () 
	{
		spawnTimer = SPAWN_INTERVAL;

		waitingText.StartText ();
	}
	
	// Update is called once per frame
	void Update () 
	{

		//run as long as game not over
		if (gameover == true) 
		{
			return;
		}

		UpdateSpawn ();			//run spawner

	}

	private void UpdateSpawn(){

		//run as long as game not over
		if (spawnPause == true) 
		{
			return;
		}
		
		spawnTimer -= Time.deltaTime;
		
		if (spawnTimer <= 0.0f) 
		{
			//spawn thing
		}
	}


	//Player connected. Start Game.
	void OnPlayerConnected(NetworkPlayer player) {

		waitingText.StopText ();

		countdown.PlayTimer ();


	}


	/*
	 * Gain a point, bomb defused.
	 */
	public void WinPoint()
	{
		bombsDefused++;
	}


	/*
	 * Lose a life
	 */
	public void LoseLife()
	{
		lives--;


		if (lives == 0) {
			//game over
			GameOver ();
		}
	}

	private void GameOver()
	{
		spawnPause = true;
		gameover = true;

		gameOverText.SetActive (true);

		server.GameOver ();
	}



	private int PRECISION = 100000000;			//how many floating point numbers to consider
	/*
	 * Returns true 'chance' percent of the time.
	 */
	private bool RandTrue(float chance)
	{
		int threshold = (int) chance * PRECISION;
		return (Random.Range(0, PRECISION +1) >= threshold ? true : false);
	}
}
