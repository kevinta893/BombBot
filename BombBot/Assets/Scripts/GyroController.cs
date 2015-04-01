// ***********************************************************
// Modified from original written by 
// Heyworks Unity Studio http://unity.heyworks.com/
// ***********************************************************
using UnityEngine;

/// <summary>
/// Gyroscope controller that works with any device orientation.
/// </summary>
public class GyroController : MonoBehaviour 
{
	#region [Private fields]
	
	private const float lowPassFilterFactor = 0.2f;
	
	private readonly Quaternion baseIdentity =  Quaternion.Euler(90, 0, 0);
	private Quaternion cameraBase =  Quaternion.identity;
	private Quaternion calibration =  Quaternion.identity;
	private Quaternion baseOrientation =  Quaternion.Euler(90, 0, 0);
	private Quaternion baseOrientationRotationFix =  Quaternion.identity;
	private Quaternion referanceRotation = Quaternion.identity;
	
	private bool debug = true;
	RuntimePlatform platform = Application.platform;
	
	private const float RAYCAST_LOOK_DISTANCE = 100.0f;			//for raycast test of bomb looking
	#endregion
	
	#region [Unity events]

	public ClientNetwork client;

	protected void Start () 
	{
		if (platform == RuntimePlatform.Android)
			AttachGyro();
	}

	//private int ddd =0;

	protected void Update() 
	{
		RaycastBombs ();

		// rotate camera with gyro
		if (platform == RuntimePlatform.Android)
		{
			transform.rotation = Quaternion.Slerp (transform.rotation,
   			cameraBase * (ConvertRotation (referanceRotation * Input.gyro.attitude) * Quaternion.identity),
            lowPassFilterFactor);
		}
		
		// send camera data to network
		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			client.SendCameraData(transform.rotation);

		}
	}

	/*
	* 	Used for keyboard camera control testing
	*/
	protected void FixedUpdate(){

		if (platform != RuntimePlatform.Android) {
			float horizontal = Input.GetAxis ("Horizontal");
			float vertical = Input.GetAxis ("Vertical");
	
			if (horizontal != 0.0) {
				camera.transform.RotateAround(camera.transform.position, Vector3.up, 1.0f * horizontal);
			}
			if (vertical != 0.0) {
				Vector3 left = GetRightVector(camera.transform.rotation) * -1.0f;
				camera.transform.RotateAround(camera.transform.position, left, 1.0f * vertical);
			}
		}
	}



	private void RaycastBombs(){

		RaycastHit objLooked;
		
		bool lookBomb =  Physics.Raycast(new Ray(camera.transform.position, GetForwardVector(camera.transform.rotation)), out objLooked, RAYCAST_LOOK_DISTANCE);
		if (lookBomb == true) {
			//looking at a particular bomb
			GameObject bombLook = objLooked.transform.gameObject;
			Debug.Log (bombLook.tag);

			if (bombLook.CompareTag("Cube_Bomb") || bombLook.CompareTag("Sphere_Bomb") || bombLook.CompareTag("Tetra_Bomb")) {
				SetActiveDefusePanel(true);
				client.SendCurrentBomb(bombLook);
			}
			else {	// looking at nothing/non-bomb
				SetActiveDefusePanel(false);
				client.SendCurrentBomb(null);
			}
		} else { // looking at nothing/non-bomb
			SetActiveDefusePanel(false);
			client.SendCurrentBomb(null);
		}
	}


	public GameObject defusePanel;

	private void SetActiveDefusePanel(bool val){
		defusePanel.SetActive (val);
	}



	//Taken from http://nic-gamedev.blogspot.ca/2011/11/quaternion-math-getting-local-axis.html?m=1
	Vector3 GetForwardVector(Quaternion q) 
	{
		return new Vector3( 2 * (q.x * q.z + q.w * q.y), 
		                   2 * (q.y * q.x - q.w * q.x),
		                   1 - 2 * (q.x * q.x + q.y * q.y));
	}
	Vector3 GetRightVector(Quaternion q)
	{
		return new Vector3( 1 - 2 * (q.y * q.y + q.z * q.z),
		               2 * (q.x * q.y + q.w * q.z),
		               2 * (q.x * q.z - q.w * q.y));
	}



	protected void OnGUI()
	{
		GUIStyle customStyle = new GUIStyle ();
		
		// set font size accordingly	
		if (platform == RuntimePlatform.Android) {
			customStyle.fontSize = 30;
		}
		else {
			customStyle.fontSize = 12;
		}
		
		if (!debug)
			return;
		
		GUILayout.Label("Orientation: " + Screen.orientation, customStyle);
		GUILayout.Label("Calibration: " + calibration, customStyle);
		GUILayout.Label("Camera base: " + cameraBase, customStyle);
		GUILayout.Label("input.gyro.attitude: " + Input.gyro.attitude, customStyle);
		GUILayout.Label("transform.rotation: " + transform.rotation, customStyle);
		GUILayout.Label ("My IP: " + Network.player.ipAddress, customStyle);
		
	}
	
	#endregion
	
	#region [Public methods]
	
	/// <summary>
	/// Attaches gyro controller to the transform.
	/// </summary>
	private void AttachGyro()
	{
		Input.gyro.enabled = true;
		ResetBaseOrientation();
		UpdateCalibration(true);
		UpdateCameraBaseRotation(true);
		RecalculateReferenceRotation();
	}
	
	#endregion
	
	#region [Private methods]
	
	/// <summary>
	/// Update the gyro calibration.
	/// </summary>
	private void UpdateCalibration(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = (Input.gyro.attitude) * (-Vector3.forward);
			fw.z = 0;
			if (fw == Vector3.zero)
			{
				calibration = Quaternion.identity;
			}
			else
			{
				calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
			}
		}
		else
		{
			calibration = Input.gyro.attitude;
		}
	}
	
	/// <summary>
	/// Update the camera base rotation.
	/// </summary>
	/// <param name='onlyHorizontal'>
	/// Only y rotation.
	/// </param>
	private void UpdateCameraBaseRotation(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = transform.forward;
			fw.y = 0;
			if (fw == Vector3.zero)
			{
				cameraBase = Quaternion.identity;
			}
			else
			{
				cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
			}
		}
		else
		{
			cameraBase = transform.rotation;
		}
	}
	
	/// <summary>
	/// Converts the rotation from right handed to left handed.
	/// </summary>
	/// <returns>
	/// The result rotation.
	/// </returns>
	/// <param name='q'>
	/// The rotation to convert.
	/// </param>
	private static Quaternion ConvertRotation(Quaternion q)
	{
		//return new Quaternion(q.x, q.y, -q.z, -q.w);	
		return new Quaternion(0, q.y, 0, -q.w);	
	}
	
	/// <summary>
	/// Recalculates reference system.
	/// </summary>
	private void ResetBaseOrientation()
	{
		baseOrientation = Quaternion.identity * baseIdentity;
	}
	
	/// <summary>
	/// Recalculates reference rotation.
	/// </summary>
	private void RecalculateReferenceRotation()
	{
		referanceRotation = Quaternion.Inverse(baseOrientation)*Quaternion.Inverse(calibration);
	}
	
	#endregion
}
