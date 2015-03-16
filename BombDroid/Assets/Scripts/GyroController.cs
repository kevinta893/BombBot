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
	
	private bool gyroEnabled = true;
	private const float lowPassFilterFactor = 0.2f;
	
	private readonly Quaternion baseIdentity =  Quaternion.Euler(90, 0, 0);
	//private readonly Quaternion landscapeRight =  Quaternion.Euler(0, 0, 90);
	//private readonly Quaternion landscapeLeft =  Quaternion.Euler(0, 0, -90);
	//private readonly Quaternion upsideDown =  Quaternion.Euler(0, 0, 180);
	
	private Quaternion cameraBase =  Quaternion.identity;
	private Quaternion calibration =  Quaternion.identity;
	private Quaternion baseOrientation =  Quaternion.Euler(90, 0, 0);
	private Quaternion baseOrientationRotationFix =  Quaternion.identity;
	
	private Quaternion referanceRotation = Quaternion.identity;
	private bool debug = true;
	
	
	private const float RAYCAST_LOOK_DISTANCE = 100.0f;			//for raycast test of bomb looking
	#endregion
	
	#region [Unity events]

	public ClientNetwork server;

	protected void Start () 
	{
		AttachGyro();
	}

	private int ddd =0;

	protected void Update() 
	{
		RaycastBombs ();

		if (!gyroEnabled)
			return;

		transform.rotation = Quaternion.Slerp(transform.rotation,
		           			cameraBase * ( ConvertRotation(referanceRotation * Input.gyro.attitude) * Quaternion.identity),
		                    lowPassFilterFactor);

		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			server.SendCameraData(transform.rotation);

		}
	}


	protected void FixedUpdate(){

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

	private void RaycastBombs(){

		RaycastHit objLooked;
		
		bool lookBomb =  Physics.Raycast(new Ray(camera.transform.position, GetForwardVector(camera.transform.rotation)), out objLooked, RAYCAST_LOOK_DISTANCE);
		if (lookBomb == true) {
			//looking at a particular bomb
			GameObject bombLook = objLooked.transform.gameObject;
			//Debug.Log (bombLook.tag);
		}
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
		GUIStyle androidStyle = new GUIStyle ();
		androidStyle.fontSize = 30;
		
		if (!debug)
			return;
		
		GUILayout.Label("Orientation: " + Screen.orientation, androidStyle);
		GUILayout.Label("Calibration: " + calibration, androidStyle);
		GUILayout.Label("Camera base: " + cameraBase, androidStyle);
		GUILayout.Label("input.gyro.attitude: " + Input.gyro.attitude, androidStyle);
		GUILayout.Label("transform.rotation: " + transform.rotation, androidStyle);
		GUILayout.Label ("My IP: " + Network.player.ipAddress, androidStyle);
		
		if (GUILayout.Button("On/off gyro: " + Input.gyro.enabled, GUILayout.Height(100)))
		{
			Input.gyro.enabled = !Input.gyro.enabled;
		}
		
		if (GUILayout.Button("On/off gyro controller: " + gyroEnabled, GUILayout.Height(100)))
		{
			if (gyroEnabled)
			{
				DetachGyro();
			}
			else
			{
				AttachGyro();
			}
		}
		/*
		if (GUILayout.Button("Update gyro calibration (Horizontal only)", GUILayout.Height(80)))
		{
			UpdateCalibration(true);
		}

		if (GUILayout.Button("Update camera base rotation (Horizontal only)", GUILayout.Height(80)))
		{
			UpdateCameraBaseRotation(true);
		}

		if (GUILayout.Button("Reset base orientation", GUILayout.Height(80)))
		{
			ResetBaseOrientation();
		}

		if (GUILayout.Button("Reset camera rotation", GUILayout.Height(80)))
		{
			transform.rotation = Quaternion.identity;
		}
		*/
	}
	
	#endregion
	
	#region [Public methods]
	
	/// <summary>
	/// Attaches gyro controller to the transform.
	/// </summary>
	private void AttachGyro()
	{
		gyroEnabled = true;
		ResetBaseOrientation();
		UpdateCalibration(true);
		UpdateCameraBaseRotation(true);
		RecalculateReferenceRotation();
	}
	
	/// <summary>
	/// Detaches gyro controller from the transform
	/// </summary>
	private void DetachGyro()
	{
		gyroEnabled = false;
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
