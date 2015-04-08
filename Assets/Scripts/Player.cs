using UnityEngine;
using System.Collections;

public class Player : NPC {

	Bullet bullet;

	public float speed;
	public Vector3 lastPosition, lastlastPosition;

	public Vector3 shootPoint;


	public Player(Vector3 position) : base(position){}

	// Use this for initialization
	void Start () {
		base.Start ();
		gameObject.rigidbody.freezeRotation = true;

		//disablePlayerControls ();
		lastPosition = transform.position;
		lastlastPosition = transform.position;
		speed = game.playerSpeed;
	}

	// Update is called once per frame
	void Update () {
		if (this.getOnServer()) {
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

	private void shoot(){
		if (Input.GetMouseButtonDown(0)){
			// instantiate a bullet prefab
			Bullet bulletClone;
			Debug.Log(fireDirection());
			Vector3 bulletPosition =  getPosition() + fireDirection();
			bulletClone = Instantiate(Resources.Load("Bullet", typeof(Bullet)), bulletPosition, Quaternion.identity) as Bullet;
			bulletClone.velocity = fireDirection() * game.bulletSpeed;
			game.sendBullet(bulletPosition, bulletClone.velocity);
		}
	}

	private Vector3 fireDirection(){
		Vector3 mousePos = Input.mousePosition;
		//mousePos.z = -(transform.position.x - Camera.mainCamera.transform.position.x);
		Vector3 worldPos = Camera.mainCamera.ScreenToWorldPoint (mousePos);
		worldPos = new Vector3(worldPos.x, transform.position.y, worldPos.z);
		return (worldPos - transform.position).normalized;
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
		this.livesOnServer = false;
	}
	public void enablePlayerControls(){
		this.livesOnServer = true;
	}
}
