using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public NPC host;
	public Vector3 movement;

    public PlayerMovement(NPC host) {
        this.host   = host;
        this.movement   = Vector3.zero;
    }

	// Update is called once per frame
	public Vector3 update () {

		return(MoveVector());
	}

	private Vector3 MoveVector() {
		//Keybaord controls
		Vector3 velocity = Vector3.zero;

		// position += velocity
		if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			velocity = new Vector3(host.getMaxVelocity(),0,0);
		}
		if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			velocity = new Vector3(-host.getMaxVelocity(),0,0);
		}
		if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			velocity = new Vector3(0,0,-host.getMaxVelocity());
		}
		if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			velocity = new Vector3(0,0,host.getMaxVelocity());
		}

		// update position
		return velocity;
	}
}
