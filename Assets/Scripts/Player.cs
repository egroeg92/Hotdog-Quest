using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	GameController game;

	// Use this for initialization
	void Start () {
		game = GameObject.Find ("GameController").GetComponent<GameController> ();
		
	}
	
	// Update is called once per frame
	void Update () {
	
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
}
