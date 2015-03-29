using UnityEngine;
using System.Collections;

public class BombManager {

	private ArrayList bombList = new ArrayList (Server.MAX_BOMBS);
	
	private int id_count = 1;


	//Random parameters
	private const int MIN_BOMB_TIMER = 3;
	private const int MAX_BOMB_TIMER = 13;			//inclusive


	public BombManager(){


	}



	/*
	 * Ticks all bomb's timers
	 */
	public void UpdateTimers(){

		for (int i =0; i < bombList.Count ; i++){

		}
	}







	/* Find bomb by id
	 * Returns null if not found
	 */
	public BombEntity GetBombEntity(int id){

		//find bomb by id
		for ( int i =0; i < bombList.Count ; i++){
			BombEntity cursor = (BombEntity) bombList[i];
			if (cursor.id == id){
				return cursor;
			}
		}

		return null;
	}



	/*
	 * Generates a random bomb
	 */
	public BombEntity GenerateBomb(){
		return AddBomb (Random.Range (0, 3),
		               Random.Range (0, 3),
		               Random.Range (0, 3), 
		               Random.Range (MIN_BOMB_TIMER, MAX_BOMB_TIMER + 1));
	}


	/*
	 * Generates a bomb given the parameters
	 */
	public BombEntity GenerateBomb(int shape, int colour, int solution, float timer){
		return AddBomb (shape, colour, solution, timer);
	}


	public void RemoveBomb(int id){
		//find bomb by id
		for ( int i =0; i < bombList.Count ; i++){
			BombEntity cursor = (BombEntity) bombList[i];
			if (cursor.id == id){
				bombList.RemoveAt(i);
			}
		}
	}




	//===================================================

	private const float DEGREE_OFFSET = 15.0f;			//Degree spacing to ensure that all bombs dont overlap
	private float INIT_DEGREE_OFFSET = 0.0f;

	private const int MAX_POSITIONS = (int) (360.0f / DEGREE_OFFSET);



	//registers a bomb onto the list
	private BombEntity AddBomb(int shape, int colour, int solution, float timer){

		//get a free location
		int index = GetRandPosition ();

		BombEntity bomb = new BombEntity(id_count, shape, colour, index, PositionToDegrees(index), solution, timer);
		bombList.Add (bomb);

		id_count++;

		return bomb;
	}



	//convert position into a degree
	private float PositionToDegrees(int index){
		return (DEGREE_OFFSET * index) + INIT_DEGREE_OFFSET;
	}


	private const int FAIL_MAX = 100;					//how many times to fail the random position generation before giving up
	/*
	 * Gets a random position in the player circle
	 * Returns -1 if there's no position left
	 */
	private int GetRandPosition (){
		
		int newPos = -1;

		int failCount = 0;

		//find next avaiable open spot
		do{
			newPos = Random.Range(0, MAX_POSITIONS);
			failCount++;
		}
		while ((IsOverlap(newPos) == true) && (failCount < FAIL_MAX));


		return (failCount < FAIL_MAX) ? newPos: -1;
	}


	//checks if the given position has been taken
	private bool IsOverlap(int position){

		for (int i =0; i < bombList.Count; i++) {
			BombEntity cursor = (BombEntity) bombList[i];

			if (cursor.position == position){
				return true;
			}
		}
		return false;
		                                      
	}

}
