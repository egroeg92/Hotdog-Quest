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
}
