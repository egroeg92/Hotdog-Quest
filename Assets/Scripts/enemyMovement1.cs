using UnityEngine;
using System.Collections;

public class enemyMovement1 : MonoBehaviour {
	public float speed;
	public Vector3 velocity;
	public Vector3 pastVelocity;

	public float wanderAngle = 0;
	public float avoidForce = 1f;

	public enemy1 enemy;


	ArrayList visiblePlayers;

	// Use this for initialization
	void Start () {
		visiblePlayers = new ArrayList ();
		
	}
	
	// Update is called once per frame
	void Update () {
		pastVelocity = velocity;
		Vector3 a = avoid ();

		if (canSee ()) {
			velocity += moveTowards (getClosestPlayer ());
			velocity += a;
		} else if (a != Vector3.zero) {
			velocity += a;
		}else {
			velocity += wander ();
		}


		velocity = velocity.normalized * speed;

		//Debug.Log (velocity);
		enemy.transform.position += velocity * Time.deltaTime;


	}
	bool canSee(){
		visiblePlayers.Clear();
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

		foreach (GameObject p in players) {
			RaycastHit hit;
			Physics.Raycast(transform.position,p.transform.position - transform.position , out hit);
			if(hit.transform != null){
				if(hit.transform.gameObject == p)
					visiblePlayers.Add(p);
			}
		
		}
		if(visiblePlayers.Count > 0)
			return true;
		return false;
	}
	Vector3 getClosestPlayer(){
		Vector3 target = transform.position + velocity;
		float dist = float.MaxValue;
		foreach (GameObject p in visiblePlayers) {
			float d = Vector3.Distance (p.transform.position, transform.position);
			if (d < dist) {
				dist = d;
				target = p.transform.position;
			}
		}
		return target;

	}

	Vector3 wander(){
		Vector3 circle = velocity;
		float circleRadius = 1;
		float angle = Random.Range (-wanderAngle, wanderAngle);
		float x = Mathf.Cos (angle) * circleRadius;
		float y = Mathf.Sin (angle) * circleRadius;
		Vector3 displacement = new Vector3(circle.x + x, 0, circle.z + y);

		Debug.DrawRay (transform.position, displacement, Color.red);

		return displacement;
	}

	Vector3 avoid(){
		Vector3 avoidanceForce = Vector3.zero;
		Vector3 forwards = velocity * 3;
		RaycastHit hit;
		Physics.Raycast (enemy.transform.position, forwards, out hit);
		if (hit.transform != null) {
			if (hit.distance < 3) {
				Vector2 obstacleCenter = new Vector2(hit.transform.position.x, hit.transform.position.z);
				Vector2 e = new Vector2(enemy.transform.position.x, enemy.transform.position.z);
				Debug.DrawLine(hit.transform.position, enemy.transform.position);
				avoidanceForce = new Vector3(e.x - obstacleCenter.x, 0, e.y - obstacleCenter.y);
				avoidanceForce = avoidanceForce.normalized * avoidForce;
				Debug.Log (avoidanceForce.x);


			}
		}
		return avoidanceForce;
	}
	Vector3 moveTowards(Vector3 target){
		Vector3 steer = (target - transform.position);
		return steer;
	}
}
