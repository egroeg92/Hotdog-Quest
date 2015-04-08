using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	public Vector3 velocity;
	void Start () {
		gameObject.tag = "bullet";
		// Destroy bullets after 5 seconds
		Destroy(gameObject, 5.0f);
		//rigidbody.AddForce(Vector3.forward * 10);
	}

	// Update is called once per frame
	void Update () {
		transform.position += velocity * Time.deltaTime;

	}
	void OnCollisionEnter(Collision col){
		Destroy (gameObject);
		
	}
}
