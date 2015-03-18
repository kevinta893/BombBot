using UnityEngine;
using System.Collections;

public class BombEntity {
	
	public int id { get; set; }
	public int type { get; set; }
	public float degrees { get; set; }
	public int solution { get; set; }
	public float timer { get; set; }
	
	/*
	 * Create a new BombObject
	 */
	public BombEntity(int id, int type, float degrees, int solution, float timer) {
		this.id = id;
		this.type = type;
		this.degrees = degrees;
		this.solution = solution;
		this.timer = timer;
	}
}
