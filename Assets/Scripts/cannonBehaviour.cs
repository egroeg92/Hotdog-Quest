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

			worldPos = Camera.mainCamera.ScreenToWorldPoint (mousePos);
			worldPos = new Vector3(worldPos.x, transform.position.y, worldPos.z);

			Vector3 parentPos = transform.parent.transform.position;

			transform.LookAt (worldPos);


		}
	}
}
