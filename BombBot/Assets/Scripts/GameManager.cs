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
	public Server server;

	private int bombsDefused;
	
	
	private const float SPAWN_INTERVAL = 5.0f;
	private float spawnTimer = SPAWN_INTERVAL;
	
	private bool paused = false;
	
	// Use this for initialization
	void Start () 
	{
		spawnTimer = SPAWN_INTERVAL;


	}
	
	// Update is called once per frame
	void Update () 
	{
		if (paused) 
		{
			return;
		}
		
		spawnTimer -= Time.deltaTime;

		if (spawnTimer <= 0.0f) 
		{

		}

	}
	
	
	/*
	 * Player has connected.
	 */
	void OnPlayerConnected(NetworkPlayer player) {
		countdown.PlayTimer ();
	}


	/*
	 * Player Gains point (bomb defused)
	 */
	public void WinPoint()
	{

	}


	/*
	 * Player loses Life
	 */
	public void LoseLife()
	{

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
