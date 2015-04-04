using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
		InitLevels ();
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
		//TODO display life onto UI

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



	//==========================================================
	//Randomization Methods
	
	
	
	private int PRECISION = 100000000;			//how many floating point numbers to consider
	/*
	 * Returns true 'chance' percent of the time.
	 */
	private bool RandTrue(float chance)
	{
		int threshold = (int) chance * PRECISION;
		return (Random.Range (0, PRECISION + 1) >= threshold ? true : false);
		
	}
	
	
	//http://rosettacode.org/wiki/Knuth_shuffle#C.23
	static void ShuffleArray<T>(T[] array)
	{
		
		for (int i = 0; i < array.Length; i++)
		{
			//find random index to swap
			int j = Random.Range(i, array.Length);
			
			//swap
			T temp = array[i];
			array[i] = array[j];
			array[j] = temp;
		}
	}


	//=========================================================
	// Private classes
	private class GameLevel{
		
		private int levelNum;
		private List<BombSpawn> bombList;
		
		public GameLevel(int levelNum)
		{
			this.levelNum = levelNum;
			bombList = new List<BombSpawn>();
		}
		
		public void AddBombSpawn(BombSpawn bombSpawn){
			bombList.Add (bombSpawn);
		}
		
		public int GetLevelNum()
		{
			return levelNum;
		}
		
		public BombSpawn[] GetBombsArray(){
			BombSpawn[] array = new BombSpawn[bombList.Count];
			bombList.CopyTo (array, 0);
			return array;
		}
	}
	
	private class BombSpawn{
		
		private int minTimer;
		private int maxTimer;
		
		
		public BombSpawn(int minTimer, int maxTimer){
			
			this.minTimer = minTimer;
			this.maxTimer = maxTimer;
			
		}
		
		/*
		 * Gets a random timer value (integer) between min and max specified.
		 */
		public float GetRandomTimer(){
			return Random.Range(minTimer, maxTimer);
		}
	}


	//==========================================================
	//Bomb Spawning methods

	private List<GameLevel> levels;




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

	private void InitLevels(){
		levels = new List<GameLevel> ();
		
		
		//level1
		GameLevel level1 = new GameLevel (1);
		levels.Add (level1);
		level1.AddBombSpawn (new BombSpawn (EASY_TIMER_MIN,EASY_TIMER_MAX));
		level1.AddBombSpawn (new BombSpawn (EASY_TIMER_MIN,EASY_TIMER_MAX));
		level1.AddBombSpawn (new BombSpawn (EASY_TIMER_MIN,EASY_TIMER_MAX));
		level1.AddBombSpawn (new BombSpawn (EASY_TIMER_MIN,EASY_TIMER_MAX));
		level1.AddBombSpawn (new BombSpawn (EASY_TIMER_MIN,EASY_TIMER_MAX));
		
		
	}













}
