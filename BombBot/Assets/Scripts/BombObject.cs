using UnityEngine;
using System.Collections;

public class BombObject {

	public int id { get; set; }
	public float degrees { get; set; }
	public GameObject obj { get; set; }

	/*
	 * Create a new BombObject
	 */
	public BombObject(int id, float degrees, GameObject obj) {
		this.id = id;
		this.degrees = degrees;
		this.obj = obj;
	}
}
