using UnityEngine;
using System.Collections;

public class SteeringManager : MonoBehaviour {
    public Vector3 steering;
    public NPC host;
    public float MAX_FORCE = 1.0f;

    // The constructor
    public SteeringManager(NPC host) {
        this.host   = host;
        this.steering   = Vector3.zero;
    }

    // The public API (one method for each behavior)
    public void seek(Vector3 target, int slowingRadius=3){
		steering += doSeek(target, slowingRadius);
	}

    public void flee(Vector3 target){
        steering += doFlee(target);
    }

    public void wander(){}
    public void evade(NPC target){}
    public void pursuit(NPC target){}

    // The update method.
    // Should be called after all behaviors have been invoked
    public Vector3 update(){
		Vector3 velocity = host.getVelocity();

        steering = truncate(steering, MAX_FORCE);
		velocity += steering;
		velocity = truncate(velocity, host.getMaxVelocity());

        return velocity;
	}

    // Reset the internal steering force.
    public void reset() {}

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

        return Vector3.zero;
    }
    private Vector3 doEvade(NPC target){
        return Vector3.zero;
    }
    private Vector3 doPursuit(NPC target){
        return Vector3.zero;
    }

    private Vector3 truncate (Vector3 vector, float max) {
        float i;
        i = max / vector.magnitude;
        i = i < 1.0f ? i : 1.0f;
        vector *= i;
        return vector;
    }
}
