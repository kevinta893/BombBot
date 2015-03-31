using UnityEngine;
using System.Collections;

/*
*	This class manages the game state and logic of the bombs.
*/

public class BombManager : MonoBehaviour {

	public Server server;

	private int idCount;
	private float spawnTimer;
	private ArrayList bombList = new ArrayList (Server.MAX_BOMBS);
	
	//Random parameters
	private float SPAWN_PERIOD = 5.0f;
	private const int MIN_BOMB_TIMER = 3;
	private const int MAX_BOMB_TIMER = 13;			//inclusive

	void Start()
	{
		idCount = 1;
		spawnTimer = SPAWN_PERIOD;
		Random.seed = System.DateTime.Now.Millisecond;
	}

	void Update()
	{
		// decrement timers
		spawnTimer -= Time.deltaTime;
		UpdateBombTimers(Time.deltaTime);
		CheckBombTimers();
	
		// on spawn timer expiry, spawn bomb and reset timer if connected to network
		if ((Network.peerType != NetworkPeerType.Disconnected) && (spawnTimer < 0)) {
			ConstructBomb();
			spawnTimer = SPAWN_PERIOD;
		}
		else if (spawnTimer < 0) {	// reset timer anyways even if not connected
			spawnTimer = SPAWN_PERIOD;
		}
	}

/**************************************************************************
******************   PRIVATE BOMB MANAGEMENT METHODS   ********************
**************************************************************************/

	/*
	 * 	Tick all bomb's timers by dec amount
	 */
	private void UpdateBombTimers(float dec)
	{
		for (int i = 0; i < bombList.Count ; i++){
			// TODO
		}
	}
	
	/*
	*	Check all bomb timers to see if any has expired.
	*	Prompt server to send RPC call to client if a bomb expires.
	*/
	private void CheckBombTimers() {
		// TODO
	}


	/* Find bomb by id
	 * Returns null if not found
	 TODO determine if this is ever used at all
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
		// params: int shape, int colour, float timer
		AddBomb(Random.Range (0, 3),
		        Random.Range (0, 3),
		        Random.Range (MIN_BOMB_TIMER, MAX_BOMB_TIMER + 1));
	}
	
	/*Debug Use only
	 *	Generates a bomb given the parameters
	 */
	public void ConstructBomb(int shape, int colour, int solution, float timer)
	{
		AddBomb(shape, colour, timer);
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




	/**************************************************************************
	***********************   BOMB CONSTRUCTION   *****************************
	**************************************************************************/

	private const float DEGREE_OFFSET = 15.0f;			//Degree spacing to ensure that all bombs dont overlap
	private float INIT_DEGREE_OFFSET = 0.0f;

	private const int MAX_POSITIONS = (int) (360.0f / DEGREE_OFFSET);
	private const int FAIL_MAX = 100;					// how many times to fail the random position generation before giving up

	/*
	*	Add a bomb with the given parameter to the Bomb list
	*/
	private void AddBomb(int shape, int colour, float timer)
	{
		//get a free location
		int index = GetRandPosition ();

		BombEntity bomb = new BombEntity(idCount, shape, colour, index, PositionToDegrees(index), timer);
		bombList.Add(bomb);

		idCount++;

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

}
