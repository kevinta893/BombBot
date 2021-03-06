﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
*	This class manages the game state and logic of the bombs.
*/

public class BombManager : MonoBehaviour {

	public Server server;
	public GameObject overviewBomb;
	public Image circle;
	public Canvas canvas;
	public AudioClip explodeSound;
	public AudioClip correctSound;

	private int idCount;
	private float spawnTimer;
	private ArrayList bombList = new ArrayList (Server.MAX_BOMBS);
	private float OVERVIEW_RADIUS;
	
	//Random parameters
	private float SPAWN_PERIOD = 5.0f;
	private const int MIN_BOMB_TIMER = 10;
	private const int MAX_BOMB_TIMER = 30;			//inclusive
	
	
	// defusing
	public int currentBomb { get; set; }

	void Start()
	{
		idCount = 1;
		spawnTimer = 0;
		OVERVIEW_RADIUS = canvas.scaleFactor * circle.rectTransform.rect.height* 0.35f;
		currentBomb = -1;
		
		Random.seed = System.DateTime.Now.Millisecond;
	}

	void Update()
	{
		// decrement timers
		//UpdateSpawnTimer(Time.deltaTime);
		UpdateBombTimers(Time.deltaTime);
		
	}

/**************************************************************************
**************************   BOMB MANAGEMENT  *****************************
**************************************************************************/

	/*
	 * 	Tick down spawn timer by dec amount. 
	 *	Construct a bomb if it hits zero and a client is connected. Reset timer after.
	 */
	private void UpdateSpawnTimer(float dec)
	{
		spawnTimer -= Time.deltaTime;
		
		if (spawnTimer < 0)
		{	// only spawn bomb if a client is connected
			if (Network.connections.Length > 0)
				ConstructBomb();
			else
				Debug.Log ("No client to spawn bomb to!");
				
			spawnTimer = SPAWN_PERIOD;	// reset timer
		}
	}

	/*
	 * 	Tick all bomb's timers by dec amount then check the timer for expiry.
	 *	Prompt server to send RPC call to client if a bomb expires.
	 */
	private void UpdateBombTimers(float dec)
	{
		ArrayList toRemove = new ArrayList();	// temp list of bombs to remove from bombList afterwards
	
		for (int i = 0; i < bombList.Count ; i++){
			BombEntity cursor = (BombEntity) bombList[i];
			cursor.timer -= dec;
			
			// update countdown on overview
			cursor.overview.transform.SendMessage("UpdateTimer", Mathf.CeilToInt(cursor.timer));
			
			// bomb reaches zero, remove from list and inform server
			if (cursor.timer <= 0){
				server.DetonateBomb(cursor.id);
				toRemove.Add(cursor);
				
				// remove from overview and show BOOM!
				cursor.overview.SendMessage("Explode");
				Destroy(cursor.overview, 1f);		// destroy bomb in 1 sec
				AudioSource.PlayClipAtPoint(explodeSound, gameObject.transform.position);

			}
		} 	
		
		// remove the bombs after iterating through them to prevent counter inconsistency
		for (int i = 0; i < toRemove.Count; i++)
		{
			bombList.Remove(toRemove[i]);
		}
	}
	
	/* Find bomb by id
	 * Returns null if not found
	 */
	public BombEntity GetBombEntity(int id)
	{

		//find bomb by id
		for ( int i = 0; i < bombList.Count ; i++){
			BombEntity cursor = (BombEntity) bombList[i];
			if (cursor.id == id){
				return cursor;
			}
		}

		return null;
	}


	/*
	 * Construct a random bomb
	 */
	private void ConstructBomb()
	{
		// params: int shape, int colour, int solution, float timer
		AddBomb(Random.Range (0, 3),
		        Random.Range (0, 3),
		        Random.Range (1, 6),
		        Random.Range (MIN_BOMB_TIMER, MAX_BOMB_TIMER + 1));
	}



	/*
	 * Constructs a random bomb with the given timer.
	 * Shape, color, and solution generated randomly
	 */
	public void ConstructBomb(float timer)
	{
		AddBomb(Random.Range (0, 3),
		        Random.Range (0, 3),
		        Random.Range (1, 6),
		        timer);
	}


	
	/*Debug Use only
	 *	Generates a bomb given the parameters
	 */
	public void ConstructBomb(int shape, int colour, int solution, float timer)
	{
		AddBomb(shape, colour, solution, timer);
	}


	/*
	*	Remove a bomb id from the bomb list
	*/
	public void RemoveBomb(int id)
	{
		//find bomb by id
		for ( int i =0; i < bombList.Count ; i++) {
			BombEntity cursor = (BombEntity) bombList[i];
			if (cursor.id == id) {
				bombList.RemoveAt(i);
				break;
			}
		}
	}

	public int GetActiveBombCount()
	{
		return bombList.Count;
	}


	/**************************************************************************
	***********************   BOMB CONSTRUCTION   *****************************
	**************************************************************************/

	private const float DEGREE_OFFSET = 45.0f;			//Degree spacing to ensure that all bombs dont overlap
	private float INIT_DEGREE_OFFSET = 0.0f;

	private const int MAX_POSITIONS = (int) (360.0f / DEGREE_OFFSET);
	private const int FAIL_MAX = 100;					// how many times to fail the random position generation before giving up

	/*
	*	Add a bomb with the given parameter to the Bomb list
	*/
	private void AddBomb(int shape, int colour, int solution, float timer)
	{
		//get a free location
		int index = GetRandPosition ();

		BombEntity bomb = new BombEntity(idCount, shape, colour, index, solution, PositionToDegrees(index), timer, null);
		bombList.Add(bomb);

		idCount++;
		
		// draw bomb on overview
		GameObject overview = DrawBomb(bomb.degrees, bomb.timer);
		bomb.overview = overview;
		
		// tell server to relay bomb to client
		server.GenerateBomb(bomb);
	}

	// convert position into a degree
	private float PositionToDegrees(int index)
	{
		return (DEGREE_OFFSET * index) + INIT_DEGREE_OFFSET;
	}


	/*
	 * Gets a random position in the player circle
	 * Returns -1 if there's no position left
	 */
	private int GetRandPosition ()
	{
		int newPos = -1;
		int failCount = 0;

		//find next avaiable open spot
		do
		{
			newPos = Random.Range(0, MAX_POSITIONS);
			failCount++;
		}
		while ((IsOverlap(newPos) == true) && (failCount < FAIL_MAX));

		return (failCount < FAIL_MAX) ? newPos : -1;
	}


	// Checks if the given position has been taken
	private bool IsOverlap(int position)
	{
		for (int i = 0; i < bombList.Count; i++) {
			BombEntity cursor = (BombEntity) bombList[i];

			if (cursor.position == position){
				return true;
			}
		}
		return false;
	}
	

	/*
	 * Randomizes the initial degree offset when generating bombs
	 */
	public void RandomizeInitDegrees()
	{
		INIT_DEGREE_OFFSET = Random.Range (0, 360);
	}

	/**************************************************************************
	**************************   BOMB DRAWING   *******************************
	**************************************************************************/

	/*
	*	Draw a bomb on the overview
	*/
	private GameObject DrawBomb(float degrees, float timer)
	{
		float radians = degrees * (Mathf.PI / 180);
		Vector3 position = new Vector3 (Mathf.Cos(radians), Mathf.Sin(radians), 0);
		position.Normalize ();
		position *= OVERVIEW_RADIUS;		//a unit vector times radius

		//instantiate the new bomb to draw on screen
		GameObject overview = (GameObject) Instantiate(overviewBomb, Vector3.zero, overviewBomb.transform.rotation);
		overview.transform.SetParent(circle.transform, false);
		overview.SetActive(true);
		overview.transform.SendMessage("UpdateTimer", timer);

		overview.GetComponent<RectTransform> ().position = position + circle.rectTransform.position;

		return overview;
	}
	
	
	/**************************************************************************
	**************************   BOMB DEFUSING   ******************************
	**************************************************************************/
	
	/*
	*	Check if the given solution is correct
	*/
	public bool VerifySolution (int id, int solution)
	{
		BombEntity target = GetBombEntity(id);
		
		if (target.solution == solution) {
			// correct solution. GJ BombBot
			target.overview.SendMessage("Correct");
			Destroy (target.overview, 1f);
			RemoveBomb(id);
			AudioSource.PlayClipAtPoint(correctSound, gameObject.transform.position);
			
			return true;
		}
		else {
			// incorrect solution. BombBot you dun goofed.
			target.overview.SendMessage("Explode");
			Destroy(target.overview, 1f);		// destroy bomb in 1 sec
			AudioSource.PlayClipAtPoint(explodeSound, gameObject.transform.position);
			RemoveBomb(id);
			
			return false;
		}
	}
	
	/*
	*	DEBUG USE: Print out solution for debug
	*/
	public string PrintSolution() 
	{/*
		if (currentBomb == -1)
			return "\nCurrent: -1";
		else {
			BombEntity tmp = GetBombEntity(currentBomb);
			return 	"\nCurrent: " + currentBomb + " Sol: " + tmp.solution + " " + tmp.colour + "/" + tmp.shape;
		}*/
		return "";
	}
}
