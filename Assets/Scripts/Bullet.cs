using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Destroy bullets after 5 seconds
		Destroy(this, 5.0f);
		//rigidbody.AddForce(Vector3.forward * 10);
	}

	// Update is called once per frame
	void Update () {

	}


}
