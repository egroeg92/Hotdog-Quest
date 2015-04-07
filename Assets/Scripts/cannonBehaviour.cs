using UnityEngine;
using System.Collections;

public class cannonBehaviour : MonoBehaviour {
	public bool enabled;

	Vector3 mousePos;
	Vector3 worldPos;
	Vector3 lookAt;

	void Start(){
		gameObject.renderer.material.color = Color.black;

	}
	// Update is called once per frame
	void Update () {
		if (enabled) {
			mousePos = Input.mousePosition;
			//mousePos.z = -(transform.position.x - Camera.mainCamera.transform.position.x);
			worldPos = Camera.mainCamera.ScreenToWorldPoint (mousePos);
			worldPos = new Vector3(worldPos.x, transform.position.y, worldPos.z);

			Vector3 parentPos = transform.parent.transform.position;
			//transform.position = new Vector3(parentPos.x, transform.position.y, parentPos.x +.5f);
			transform.LookAt (worldPos);

			//Debug.Log (transform.RotateAround(transform.parent.transform.position, Vector3.up, 20 * Time.deltaTime));


		}
	}
}
