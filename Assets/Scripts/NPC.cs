using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	public float MAX_VELOCITY;
	public Vector3 position;
    public Vector3 velocity;
    public Vector3 pastVelocity;
    public bool livesOnServer;
	private int health;

	public NPC (Vector3 position) {
		this.transform.position = position;
		// steering = new SteeringManager(this);
	}

	// Update is called once per frame
	void Update () {
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

	public void setHelath(int hp){
		this.health = hp;
	}
	public int getHealth(){
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
