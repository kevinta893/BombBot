using UnityEngine;
using System.Collections;

public class DefusePanel : MonoBehaviour {


	private float START_Y;

	// Use this for initialization
	void Start () {
		START_Y = this.transform.position.y;
		//this.transform.position.Set (this.transform.position.x, START_Y - 1000, this.transform.position.z);
		this.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void HideDefusePanel(){
		Debug.Log ("HIDING");
		this.transform.position.Set (this.transform.position.x, START_Y - 1000, this.transform.position.z);
		this.gameObject.SetActive (false);
	}

	public void ShowDefusePanel(){
		Debug.Log ("SHowing");
		this.transform.position.Set (this.transform.position.x, START_Y + 1000, this.transform.position.z);
		this.gameObject.SetActive (true);
	}
}
