using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
*	This class manages the game state and logic of the bombs.
*/

public class BombManager : MonoBehaviour {

	public Server server;
	public GameObject overviewBomb;
	public Image circle;
	public Text boom;		// a poor man's explosion	

	private int idCount;
	private float spawnTimer;
	private float boomTimer;
	private ArrayList bombList = new ArrayList (Server.MAX_BOMBS);
	private float OVERVIEW_RADIUS;
	
	//Random parameters
	private float BOOM_PERIOD = 1.0f;
	private float SPAWN_PERIOD = 5.0f;
	private const int MIN_BOMB_TIMER = 10;
	private const int MAX_BOMB_TIMER = 30;			//inclusive
	
	
	// defusing
	public int currentBomb { get; set; }

	void Start()
	{
		idCount = 1;
		spawnTimer = 0;
		boomTimer = 0;
		OVERVIEW_RADIUS = Screen.height * 0.25f;
		currentBomb = -1;
		Random.seed = System.DateTime.Now.Millisecond;
	}

	void Update()
	{
		// decrement timers
		UpdateSpawnTimer(Time.deltaTime);
		UpdateBombTimers(Time.deltaTime);
		UpdateBoomTimer(Time.deltaTime);
		
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
				Destroy(cursor.overview);
				boomTimer = BOOM_PERIOD;
			}
		} 	
		
		// remove the bombs after iterating through them to prevent counter inconsistency
		for (int i = 0; i < toRemove.Count; i++)
		{
			bombList.Remove(toRemove[i]);
		}
	}
	
	
	/*
	*	Show the boom text for a short time then remove
	*/
	private void UpdateBoomTimer(float dec)
	{
		boomTimer -= dec;
		
		// no boom
		if (boomTimer <= 0)
			boom.text = "";
		else
			boom.text = "BOOM!";
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
		// params: int shape, int colour, int solution, float timer
		AddBomb(Random.Range (0, 3),
		        Random.Range (0, 3),
		        Random.Range (1, 4),
		        Random.Range (MIN_BOMB_TIMER, MAX_BOMB_TIMER + 1));
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
	
	
	/**************************************************************************
	**************************   BOMB DRAWING   *******************************
	**************************************************************************/

	/*
	*	Draw a bomb on the overview
	*/
	private GameObject DrawBomb(float degrees, float timer)
	{
		// TODO ADD MATH FOR PROPER POSITIONING!
		float radians = degrees * (Mathf.PI / 180);
		Vector3 position = new Vector3 (Mathf.Cos(radians), Mathf.Sin(radians), 0) * OVERVIEW_RADIUS;		//a unit vector times radius
		
		GameObject overview = (GameObject) Instantiate(overviewBomb, position, overviewBomb.transform.rotation);
		overview.transform.SetParent(circle.transform, false);
		overview.SetActive(true);
		overview.transform.SendMessage("UpdateTimer", timer);
		
		return overview;
	}
}
