using UnityEngine;
using System.Collections;
using UnityEngine.XR;


public class Player : MonoBehaviour
{
//	private UnityEngine.AI.NavMeshAgent _agent;
	private CharacterController _charControl;
	public float moveSpeed 	 = 3.0F;
	public float runSpeedAdd = 2;
	public float mouseSpeed  = 0.2F;
	public int 	smoothing 	 = 12;
	public float jumpPower   = 9.0f;
	public AudioClip _bluntHit;
	public Vector2 rotation  = new Vector2 (0, 0);
	private float _inputVert = 0.0f;
	private float _inputHorz = 0.0f;
	private float _mouseX;
	private float _mouseY;
	private float _gravity 		= -32f;
	private float _jumpVelocity = 0;
	private bool  _toggleAngle 	= false;


	private float [] _mouseXSamples;
	private float [] _mouseYSamples;
	private int      _mouseSampleCount = 0;
	private int      _mouseSampleIndex = 0;

	public  Camera   myCamera;


	void OnTriggerEnter( Collider icol )
	{
		float strength = Random.Range ( 0.0f, 1.0f );

		switch ( icol.transform.name )
		{
		case "ShotMagic":
			IndicatorProManager.Activate ( "Magic", icol.gameObject.transform.position, strength );
			break;

		case "ShotFire":
			IndicatorProManager.Activate ( "FireBall", icol.gameObject.transform.position, strength );
			break;

		case "ShotClassic":
			IndicatorProManager.Activate ( "Classic", icol.gameObject.transform.position, strength );
			break;
		}
	}

	void Start ()
	{
		if (XRSettings.enabled)
		{
			_toggleAngle = true;
			IndicatorProManager.SetIndicatorAngle(50);
		}

		_charControl = GetComponent<CharacterController>();

		_mouseXSamples = new float[smoothing];
		_mouseYSamples = new float[smoothing];

	}

	void Update ()
	{
		float speed = moveSpeed;

		if (Input.GetMouseButtonDown(0))
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		if ( Input.GetKeyDown("tab") )
		{
			_toggleAngle = !_toggleAngle;

			if ( _toggleAngle )
				IndicatorProManager.SetIndicatorAngle(50);
			else
				IndicatorProManager.SetIndicatorAngle(0);
		}

		if ( Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift) )
			speed += runSpeedAdd;

		_inputHorz = Input.GetAxis("Horizontal") * speed;
		_inputVert = Input.GetAxis("Vertical") * speed;

		_mouseXSamples[_mouseSampleIndex] = Input.GetAxisRaw("Mouse X") * mouseSpeed;
		_mouseYSamples[_mouseSampleIndex] = Input.GetAxisRaw("Mouse Y") * mouseSpeed;

		_mouseSampleIndex = ( _mouseSampleIndex + 1 ) % smoothing;

		if ( _mouseSampleCount < smoothing )
			_mouseSampleCount++;

		_mouseX = 0;
		_mouseY = 0;

		for (int loop = 0; loop < _mouseSampleCount; loop++ )
		{
			_mouseX += _mouseXSamples[loop];
			_mouseY += _mouseYSamples[loop];
		}

		if ( _charControl.isGrounded )
		{
			if ( Input.GetButton("Jump") )
				_jumpVelocity = jumpPower;
			else
				_jumpVelocity = _gravity * Time.deltaTime;
		}
		else
			_jumpVelocity += _gravity * Time.deltaTime;

		Vector3 move	= new Vector3( _inputHorz, 0, _inputVert );
		move 		  	= Vector3.ClampMagnitude(move, speed);

		if (XRSettings.enabled)
			move 		  	= myCamera.transform.TransformVector(move);
		else
			move 		  	= transform.TransformVector(move);

		move.y += _jumpVelocity;

		_charControl.Move( move * Time.deltaTime);

		rotation.y += ( Input.GetAxis ("Mouse X") * mouseSpeed );
		rotation.x += -Input.GetAxis ("Mouse Y") * mouseSpeed;
		rotation.x = Mathf.Clamp(rotation.x, -50f, 50f);
		transform.eulerAngles = (Vector2)rotation;
	}
}
