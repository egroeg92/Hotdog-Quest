using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	public float MAX_VELOCITY;
	public Vector3 position;
    public Vector3 velocity;

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

	public float getMaxVelocity() {
		return this.MAX_VELOCITY;
	}

	public Vector3 getPosition(){
		return this.transform.position;
	}

}
