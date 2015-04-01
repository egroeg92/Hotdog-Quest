using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

	public Enemy host;
	public Vector3 moveVector;
	public Vector3 direction;
	private float minDistance;
	private int currentIndex = 0;

	public EnemyMovement(Enemy host) {
		this.host = host;
		this.currentIndex = 0;
		this.minDistance = 0.5f;
	}

	public Vector3 update(){
		return(Move());
	}

	public void getWaypoints(){
		//GameObject results[] = GameObject.FindGameObjectsWithTag("Waypoint");
	}

	private Vector3 Move(){
		// if at waypoint, this will update it
		checkIfAtWaypoint();

		direction = host.getCurrentWaypoint().position - host.getPosition();
		moveVector = direction.normalized * host.getMaxVelocity();
		return(moveVector);
	}

	private void checkIfAtWaypoint() {
		if (Vector3.Distance(host.getCurrentWaypoint().position, host.getPosition()) < minDistance) {
			//TODO: determine rotation
			//transform.rotation *= Quaternion.Euler(0, 90, 0);
			int index = host.getCurrentIndex() +1;
			if (index > host.getTrack().Length -1) {
				index = 0;
			}
			host.setCurrentWaypoint(host.getTrack()[index]);
			host.setCurrentIndex(index);
		}
	}


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}
