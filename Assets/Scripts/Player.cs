using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	GameController game;


	public Vector3 velocity;
	public Vector3 lastPosition, lastlastPosition;



	// Use this for initialization
	void Start () {
		game = GameObject.Find ("GameController").GetComponent<GameController> ();
		//disablePlayerControls ();
		lastPosition = transform.position;
		lastlastPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

	}
	void LateUpdate(){
		if (Time.frameCount % game.positionUpdateframeRate == 0) {
			lastlastPosition = lastPosition;
			lastPosition = transform.position;
		}

	}

	public void disablePlayerControls(){
		CharacterController cc = GetComponent<CharacterController>();
		//MouseLook ml = GetComponent<MouseLook> ();
		
		MonoBehaviour cm = gameObject.GetComponent ("CharacterMotor") as MonoBehaviour;
		//MonoBehaviour fps = gameObject.GetComponent ("FPSInputController") as MonoBehaviour;
		
		//fps.enabled = false;
		cm.enabled = false;
		cc.enabled = false;
		//ml.enabled = false;
	}
	public void enablePlayerControls(){

		CharacterController cc = GetComponent<CharacterController>();
		//MouseLook ml = GetComponent<MouseLook> ();
		
		MonoBehaviour cm = gameObject.GetComponent ("CharacterMotor") as MonoBehaviour;
		//MonoBehaviour fps = gameObject.GetComponent ("FPSInputController") as MonoBehaviour;
		
		//fps.enabled = false;
		cm.enabled = true;
		cc.enabled = true;
	}
}
