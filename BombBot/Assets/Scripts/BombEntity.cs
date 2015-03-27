using UnityEngine;
using System.Collections;

public class BombEntity {
	
	public int id { get; set; }
	public int shape { get; set; }	// 0 for square, 1 for circle, 2 for tetrahedron
	public int colour { get; set; }	// 0 for red, 1 for blue, 2 for green
	public float degrees { get; set; }
	public int solution { get; set; }
	public float timer { get; set; }
	
	/*
	 * Create a new BombObject
	 */
	public BombEntity(int id, int shape, int colour, float degrees, int solution, float timer) {
		this.id = id;
		this.shape = shape;
		this.colour = colour;
		this.degrees = degrees;
		this.solution = solution;
		this.timer = timer;
	}
}
