using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	public GameController game;
	public float MAX_VELOCITY;
	public Vector3 position;
    public Vector3 velocity;
    public Vector3 pastVelocity;
    public bool livesOnServer;
	private float health;
	public int id;

	protected void Start(){
		game =GameObject.Find ("GameController").GetComponent<GameController> ();
	}
	public NPC (Vector3 position) {
		this.transform.position = position;
		// steering = new SteeringManager(this);
	}

	// Update is called once per frame
	void Update () {
		if (transform.position.y != game.plane.transform.position.y + transform.localScale.y/2){
			Vector3 correctedPos = transform.position;
			correctedPos.y = game.plane.transform.position.y + transform.localScale.y/2;
			transform.position = correctedPos;
		}
	}

	public Vector3 getVelocity() {
		return this.velocity;
	}
	public bool getOnServer() {
		return this.livesOnServer;
	}

	public Vector3 getPastVelocity() {
		return this.pastVelocity;
	}

	public float getMaxVelocity() {
		return this.MAX_VELOCITY;
	}
	public void setMaxVelocity(float max) {
		this.MAX_VELOCITY = max;
	}

	public Vector3 getPosition(){
		return this.transform.position;
	}

	public void setHealth(float hp){
		this.health = hp;
	}
	public float getHealth(){
		return this.health;
	}

	public void takeDamage(int damage) {
		this.health -= damage;
	}

	protected Vector3 truncate (Vector3 vector, float max) {
		float i;
		i = max / vector.magnitude;
		i = i < 1.0f ? i : 1.0f;
		vector *= i;
		return vector;
	}
}
