using UnityEngine;
using System.Collections;

public class SteeringManager : MonoBehaviour {
    public Vector3 steering;
    public NPC host;
    public float MAX_FORCE = 4.0f;
    public float MAX_SEE_AHEAD = 1.0f;
    public float MAX_AVOID_FORCE = 1.0f;
    public float CIRCLE_DISTANCE = 1.0f;
    public float CIRCLE_RADIUS = 1.5f;
    private float ANGLE_CHANGE = 15;
    private float wanderAngle;
    private Vector3 ahead;
    private Vector3 ahead2;

    // The constructor
    public SteeringManager(NPC host) {
        this.host   = host;
        this.steering   = Vector3.zero;
        this.wanderAngle = 0f;
    }

    // The public API (one method for each behavior)
    public void seek(Vector3 target, int slowingRadius=3){
		steering += doSeek(target, slowingRadius);
	}

    public void flee(Vector3 target){
        steering += doFlee(target);
    }

    public void wander(){
        steering += doWander();
    }
    public void evade(NPC target){}
    public void pursuit(NPC target){}

    // The update method.
    // Should be called after all behaviors have been invoked
    public Vector3 update(){
        // Set steering vector back to zero
		Vector3 velocity = host.getVelocity();

        // always do collision avoidance
        steering += collisionAvoidance();

        steering = truncate(steering, MAX_FORCE);
		velocity += steering;
		velocity = truncate(velocity, host.getMaxVelocity());

        return velocity;
	}

	protected Vector3 collisionAvoidance(){
        float dynamic_length = host.getVelocity().magnitude / host.getMaxVelocity();
		ahead = host.getPosition() + host.getVelocity().normalized * dynamic_length;
		ahead2 = Vector3.zero;
        ahead2.x = ahead.x *0.5f;
        ahead2.z = ahead.z *0.5f;

		Debug.DrawRay(host.getPosition(), ahead, Color.green);
		Debug.DrawRay(host.getPosition(), ahead2, Color.red);

		GameObject mostThreatening = (GameObject) findMostThreateningObstacle();
	    Vector3 avoidance = new Vector3(0, 0, 0);

	    if (mostThreatening != null) {
	        avoidance.x = ahead.x - mostThreatening.transform.position.x;
	        avoidance.z = ahead.z - mostThreatening.transform.position.z;

	        avoidance = avoidance.normalized;
	        avoidance *= MAX_AVOID_FORCE;
	    } else {
	        avoidance *= 0;
	    }

	    return avoidance;
	}

    private bool lineIntersectsObstacle(Vector3 ahead, Vector3 ahead2, GameObject obstacle) {
        // the property "center" of the obstacle is a Vector3D.
        Vector3 center = obstacle.GetComponent<Renderer>().bounds.center;
        float radius = obstacle.GetComponent<Renderer>().bounds.extents.magnitude;
        return Vector3.Distance(center, ahead) <= radius
                    || Vector3.Distance(center, ahead2) <= radius;
    }

    private GameObject findMostThreateningObstacle(){
		GameObject mostThreatening = null;
		GameObject[] obstacles;
		obstacles = GameObject.FindGameObjectsWithTag("city");

		foreach (GameObject o in obstacles) {
			bool collision = lineIntersectsObstacle(ahead, ahead2, o);
            if (collision && (mostThreatening == null
                    || Vector3.Distance(host.getPosition(), o.transform.position) < Vector3.Distance(host.getPosition(), mostThreatening.transform.position))) {
	            mostThreatening = o;
	        }
	    }

		return mostThreatening;
	}

    // Reset the internal steering force.
    public void reset() {
        steering = Vector3.zero;
    }

    // The internal API
    private Vector3 doSeek(Vector3 target, int slowingRadius){
		Vector3 force;
		float distance;

		Vector3 desired = target - host.transform.position;
		distance = desired.magnitude;
		desired = desired.normalized;

		if (distance <= slowingRadius) {
			desired *= (host.getMaxVelocity() * distance/slowingRadius);
		} else {
			desired *= host.getMaxVelocity();
		}

		force = desired - host.getVelocity();
		return force;
	}
    private Vector3 doFlee(Vector3 target){
		Vector3 force;
		float distance;

        // the same math as seek, but with the direciton of the vector reversed
		Vector3 desired = host.transform.position - target;
		distance = desired.magnitude;
		desired = desired.normalized;

		desired *= host.getMaxVelocity();

		force = desired - host.getVelocity();
		return force;
    }

    private Vector3 doWander(){
        Vector3 circleCenter = host.getVelocity();
        circleCenter = circleCenter.normalized;
        circleCenter *= CIRCLE_DISTANCE;

        Vector3 displacement = new Vector3(1, 0, -1) * CIRCLE_RADIUS;

        displacement = setAngle(displacement, wanderAngle);

        wanderAngle += Random.Range(0,1.1f) * ANGLE_CHANGE - ANGLE_CHANGE *0.5f;

        Vector3 wanderForce = circleCenter + displacement;
        Debug.DrawRay(host.getPosition(), wanderForce, Color.magenta);
        return wanderForce;

    }

    private Vector3 doEvade(NPC target){
        return Vector3.zero;
    }
    private Vector3 doPursuit(NPC target){
        return Vector3.zero;
    }

    private Vector3 setAngle(Vector3 vector, float val) {
        float len = vector.magnitude;
        vector.x = Mathf.Cos(val) * len;
        vector.z = Mathf.Sin(val) * len;

        return vector;
    }

    private Vector3 truncate (Vector3 vector, float max) {
        float i;
        i = max / vector.magnitude;
        i = i < 1.0f ? i : 1.0f;
        vector *= i;
        return vector;
    }
}
