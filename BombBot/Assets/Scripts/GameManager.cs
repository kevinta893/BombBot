using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour, CountdownUI.CountDownFinishCallback {

	
	//Easy bomb constants
	private const int EASY_TIMER_MAX = 45;
	private const int EASY_TIMER_MIN = 30;
	
	//Medium bomb constants
	private const int MED_TIMER_MAX = 30;
	private const int MED_TIMER_MIN = 20;
	
	//hard bomb constants
	private const int HARD_TIMER_MAX = 20;
	private const int HARD_TIMER_MIN = 15;


	public CountdownUI countdown;
	public WaitingPlayerText waitingText;
	public GameOverPanel gameOverPanel;
	public NextRoundUI nextRoundPanel;

	public Text scoreText;
	public Text lifeText;

	public Server server;

	private int bombsDefused = 0;
	private const int MAX_LIVES = 700;
	private int lives = MAX_LIVES;
	
	private bool spawnPause = true;
	private bool gameover = false;


	// Use this for initialization
	void Start () 
	{

		scoreText.text = "Score: " + bombsDefused;
		lifeText.text = "Lives: " + lives;

		nextRoundPanel.gameObject.SetActive (true);
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
	void OnPlayerConnected(NetworkPlayer player) 
	{

		waitingText.StopText ();
		LoadNextLevel ();		//begin game by loading the next level


	}


	/*
	 * Gain a point, bomb defused.
	 */
	public void WinPoint()
	{
		bombsDefused++;
		scoreText.text = "Score: " + bombsDefused.ToString();
	}


	/*
	 * Lose a life
	 */
	public void LoseLife()
	{
		lives--;
		lifeText.text = "Lives: " + ((lives <= 0 ) ? 0 : lives);

		if (lives == 0) {
			//game over
			GameOver ();
		}
	}

	private void GameOver()
	{
		spawnPause = true;
		gameover = true;

		gameOverPanel.ShowPanel (bombsDefused);

		server.GameOver (bombsDefused);
	}



	/*
	 * Do game over if we lose connection
	 */
	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		//lost connection to player, set gameover
		gameOverPanel.ShowPanelConnectionLost (bombsDefused);

		GameOver ();

		Network.Disconnect ();		//shutdown server
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

	static void ShuffleList<T>(List<T> list)
	{
		
		for (int i = 0; i < list.Count; i++)
		{
			//find random index to swap
			int j = Random.Range(i, list.Count);
			
			//swap
			T temp = list[i];
			list[i] = list[j];
			list[j] = temp;
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
		
		public void AddBombSpawn(BombSpawn bombSpawn)
		{
			bombList.Add (bombSpawn);
		}
		
		public int GetLevelNum()
		{
			return levelNum;
		}
		
		public List<BombSpawn> GetBombsList()
		{
			return bombList;
		}

		/*
		 * Shuffles bomb list
		 */ 
		public void ShuffleBombSpawns(){
			ShuffleList (this.bombList);
		}

	}
	
	private class BombSpawn{
		
		private int minTimer;
		private int maxTimer;
		private float deltaDelay;		//delay for the CURRENT bomb, not the next bomb, delay is time between bombs, not from start of level
		
		public BombSpawn(int minTimer, int maxTimer, float deltaDelay)
		{
			
			this.minTimer = minTimer;
			this.maxTimer = maxTimer;
			this.deltaDelay = deltaDelay;
		}
		
		/*
		 * Gets a random timer value (integer) between min and max specified.
		 */
		public float GetRandomTimer()
		{
			return Random.Range(minTimer, maxTimer);
		}

		public float GetDelay()
		{
			return deltaDelay;
		}


	}


	//==========================================================
	//Bomb Spawning methods

	public BombManager bm;

	private List<GameLevel> levels;

	private List<BombSpawn> bombQueue;

	private float spawnTimer = 0.0f;

	private bool waitFinishLevel = false;
	private int currentLevel = 0;
	private int bombGoal = 0;


	private void UpdateSpawn()
	{

		//wait until next level
		if (waitFinishLevel == true) 
		{
			
			if (bombsDefused == (bombGoal - (MAX_LIVES - lives)))
			{
				//finished level 
				FinishLevel();
			}
			
			if (bombsDefused > bombGoal){
				Debug.Log ("Warning! Went over bomb goal");
			}
		}


		//run as long as game not over
		if (spawnPause == true) 
		{
			return;
		}
		
		spawnTimer -= Time.deltaTime;


		//spawn
		if (spawnTimer <= 0.0f) 
		{

			//spawn next bomb now
			bm.ConstructBomb(bombQueue[0].GetRandomTimer());
			bombQueue.RemoveAt(0);

			if (bombQueue.Count == 0)
			{
				//no bombs left, wait until player has destroyed all bombs.
				spawnPause = true;
				waitFinishLevel = true;		//wait until player finishes all bombs

			}else{
				//another bomb in queue, spawn with delay
				spawnTimer = bombQueue[0].GetDelay();
			}
		}




	}

	private void FinishLevel()
	{

		currentLevel++;
		waitFinishLevel = false;
		server.InformLevelComplete (currentLevel);
		LoadNextLevel();
	}


	public void StartLevel()
	{
		server.InformLevelStart (currentLevel);
		countdown.PlayTimer (this);
	}

	//start level
	public void CountDownFinishedCallback(){
		spawnPause = false;
	}


	/*
	 * Load the next level goals and spawn list
	 * Shows the start round menu to start
	 */
	private void LoadNextLevel()
	{

		if (levels.Count > 0) {
			//level complete, throw it out
			bombQueue = levels [0].GetBombsList ();

			spawnTimer = bombQueue [0].GetDelay ();
			bombGoal += bombQueue.Count;

			levels.RemoveAt (0);
			nextRoundPanel.ShowPanel (currentLevel);
		
			lastLevelBombCount = bombQueue.Count;			//for record keeping 
		} else {
			//no more levels, generate endless
			GenerateNextEndlessLevel();
			LoadNextLevel();		//call self as we have made a level
		}
		//all one has to do now is set spawnPause = false; to start
	}


	private int lastLevelBombCount;
	private const int MAX_BOMBS_PER_ENDLESS_LEVEL = 20;

	private void GenerateNextEndlessLevel()
	{
		GameLevel newLevel = new GameLevel (currentLevel);
		levels.Add (newLevel);

		lastLevelBombCount += 2;
		lastLevelBombCount = lastLevelBombCount >= MAX_BOMBS_PER_ENDLESS_LEVEL ? MAX_BOMBS_PER_ENDLESS_LEVEL : lastLevelBombCount;

		for (int i =0; i < lastLevelBombCount+2; i++) 
		{
			if (i == 0){
				//first bomb should spawn close to immediate
				newLevel.AddBombSpawn(new BombSpawn(HARD_TIMER_MIN, HARD_TIMER_MAX, Random.Range(1,3)));
			}else{
				newLevel.AddBombSpawn(new BombSpawn(HARD_TIMER_MIN, HARD_TIMER_MAX, Random.Range(3,6)));
			}
		}


	}

	/*
	 * Initializes the levels. Declare predefined levels here.
	 * First level is ready to load
	 */
	private void InitLevels()
	{
		levels = new List<GameLevel> ();
		

		//level1
		GameLevel level1 = new GameLevel (1);
		levels.Add (level1);
		/*
		level1.AddBombSpawn (new BombSpawn (EASY_TIMER_MIN, EASY_TIMER_MAX, 2.0f));
		level1.AddBombSpawn (new BombSpawn (EASY_TIMER_MIN, EASY_TIMER_MAX, Random.Range (4,10)));
		level1.AddBombSpawn (new BombSpawn (EASY_TIMER_MIN, EASY_TIMER_MAX, Random.Range (4,10)));
		level1.AddBombSpawn (new BombSpawn (EASY_TIMER_MIN, EASY_TIMER_MAX, Random.Range (4,10)));
		level1.AddBombSpawn (new BombSpawn (EASY_TIMER_MIN, EASY_TIMER_MAX, Random.Range (4,10)));
		*/

		//debug Level
		level1.AddBombSpawn (new BombSpawn(1, 1, 1.0f));
		level1.AddBombSpawn (new BombSpawn(1, 1, 1.0f));

		GameLevel level2 = new GameLevel (2);
		levels.Add (level2);
		level2.AddBombSpawn (new BombSpawn(1, 1, 1.0f));
		level2.AddBombSpawn (new BombSpawn(1, 1, 2.0f));

		GameLevel level3 = new GameLevel (3);
		levels.Add (level3);
		level3.AddBombSpawn (new BombSpawn(1, 1, 1.0f));
		level3.AddBombSpawn (new BombSpawn(1, 1, 2.0f));

		GameLevel level4 = new GameLevel (4);
		levels.Add (level4);
		level4.AddBombSpawn (new BombSpawn(1, 1, 1.0f));
		level4.AddBombSpawn (new BombSpawn(1, 1, 2.0f));

	}






}
