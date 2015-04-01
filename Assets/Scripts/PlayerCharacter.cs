using UnityEngine;
using System.Collections;

public class PlayerCharacter : NPC {

	public PlayerMovement movement;

	public PlayerCharacter(Vector3 position) : base(position) {
		Debug.Log("Player character created.");
	}

	// Use this for initialization
	void Start () {
		movement = new PlayerMovement(this);
	}

	// Update is called once per frame
	void Update () {
		velocity = movement.update();

		velocity = velocity.normalized;
		velocity *= MAX_VELOCITY;
		velocity = truncate(velocity, MAX_VELOCITY);

		//Update movement
		transform.Translate(velocity * Time.deltaTime);

	}
}
