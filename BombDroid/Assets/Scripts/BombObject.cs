using UnityEngine;
using System.Collections;

public class BombObject {

	public int id { get; set; }
	public int type { get; set; }
	public float degrees { get; set; }
	public GameObject obj { get; set; }

	/*
	 * Create a new BombObject
	 */
	public BombObject(int id, int type, float degrees, GameObject obj) {
		this.id = id;
		this.type = type;
		this.degrees = degrees;
		this.obj = obj;
	}
}
