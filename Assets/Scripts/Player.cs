using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	GameController game;

	public float speed;
	public Vector3 pastVelocity;
	public Vector3 velocity;
	public Vector3 lastPosition, lastlastPosition;

	public Vector3 shootPoint;

	public cannonBehaviour cannon;

	bool enabled = false;


	// Use this for initialization
	void Start () {
		game = GameObject.Find ("GameController").GetComponent<GameController> ();
		gameObject.rigidbody.freezeRotation = true;

		//disablePlayerControls ();
		lastPosition = transform.position;
		lastlastPosition = transform.position;
		speed = game.playerSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		if (enabled) {
			pastVelocity = velocity;
			velocity = MoveVector ();
			velocity = velocity.normalized * speed;
			transform.position += velocity * Time.deltaTime;

			shoot();
		}

	}

	public void updateVelocity(){

		pastVelocity = velocity;

	}

	private bool shoot(){
		if (Input.GetMouseButtonDown(0)){

			return true;
		}
		return false;
	}

	private Vector3 MoveVector() {
		//Keybaord controls
		Vector3 velocity = Vector3.zero;
		
		// position += velocity
		if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			velocity = new Vector3(1,0,0);
		}
		if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			velocity = new Vector3(-1,0,0);
		}
		if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			velocity = new Vector3(0,0,-1);
		}
		if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			velocity = new Vector3(0,0,1);
		}
		
		return velocity;
	}

	public void disablePlayerControls(){
		enabled = false;
		cannon.enabled = false;
	}
	public void enablePlayerControls(){
		enabled = true;
		cannon.enabled = true;
	}
}
