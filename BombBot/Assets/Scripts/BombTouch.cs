using UnityEngine;
using System.Collections;

public class BombTouch : MonoBehaviour {
	
	RuntimePlatform platform = Application.platform;
	
	// Update is called once per frame
	void Update () {
	
		RaycastHit target;
		
		// detect platform so both touch and mouse work
		if (platform == RuntimePlatform.Android)
		{
			for (int i = 0; i < Input.touchCount; ++i) {
				if (Input.GetTouch(0).phase == TouchPhase.Began) {
					// get target from raycast
					Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
					if (Physics.Raycast(ray, out target))
						Defuse (target);
				}
			}
		}
		else {
			if(Input.GetMouseButtonDown(0)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out target))
					Defuse (target);
			}
		}
		
	}

	/*
	 * Attempt to defuse the target bomb
	 * @param target, info about the target touched
	 */
	private void Defuse(RaycastHit target)
	{
		// only defuse bombs
		if (target.collider.gameObject.CompareTag("Cube_Bomb") || 
		    	target.collider.gameObject.CompareTag("Sphere_Bomb") || 
		    	target.collider.gameObject.CompareTag("Tetra_Bomb"))
		    Destroy(target.collider.gameObject);
	}
	
}
