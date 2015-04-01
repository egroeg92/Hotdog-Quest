﻿using UnityEngine;
using System.Collections;

public class mapGen : MonoBehaviour {
	public float dim;
	public int divisionDim;
	public float courtProb;
	public float neighbourhoodSizeNoise;

	float MIN_BUILDING_DIM = 1;

	GameObject plane;

	ArrayList lines = new ArrayList();

	// Use this for initialization
	void Start () {
		plane = GameObject.CreatePrimitive (PrimitiveType.Plane) as GameObject;
		plane.transform.position = Vector3.zero;
		plane.transform.localScale = new Vector3(dim/10, 1, dim/10);

		Vector3 p00 = plane.renderer.bounds.min;
		Vector3 p10 = new Vector3(plane.renderer.bounds.max.x , 0 , plane.renderer.bounds.min.z);
		Vector3 p01 = new Vector3(plane.renderer.bounds.min.x, 0, plane.renderer.bounds.max.z);
		Vector3 p11 = plane.renderer.bounds.max;

		Debug.Log ("bl "+p00);
		Debug.Log ("br "+p10);
		Debug.Log ("tl "+p01);
		Debug.Log ("tr "+p11);
		
		Debug.Log ("+++");

		divideMap (divisionDim, p00, p10, p01, p11, true);
	}
	void divideMap(int level, Vector3 p00, Vector3 p10, Vector3 p01, Vector3 p11, bool horiz){
		if (level == 0) {

			if(Random.Range(0,100) > courtProb){
				float x = (p00.x + p11.x)/2;
				float z = (p00.z + p11.z)/2;
				float xMag = Mathf.Abs(p11.x - p00.x) ;
				float zMag = Mathf.Abs(p11.z - p00.z) ;

				if(xMag < MIN_BUILDING_DIM/2 || zMag < MIN_BUILDING_DIM/2)
					return;


				float yMag = Random.Range( 2, 5);

				xMag -= Random.Range(1,3); 
				zMag -= Random.Range(1,3);

				xMag = xMag < MIN_BUILDING_DIM ? MIN_BUILDING_DIM : xMag;
				zMag = zMag < MIN_BUILDING_DIM ? MIN_BUILDING_DIM : zMag;


				GameObject building = GameObject.CreatePrimitive (PrimitiveType.Cube);

				building.transform.localScale = new Vector3(xMag, yMag, zMag);



				building.transform.position = new Vector3(x,building.transform.localScale.y/2,z);
				building.name = "Building";

			}

			return;
		}
		float xM,zM;


		float range = Random.Range (-neighbourhoodSizeNoise, neighbourhoodSizeNoise);
		if (horiz) {
			xM = (p11.x + p00.x) / 2;
			xM += range;

			xM = (xM >= plane.renderer.bounds.max.x) ? plane.renderer.bounds.max.x : xM;
			xM = (xM <= plane.renderer.bounds.min.x) ? plane.renderer.bounds.min.x : xM;

			Vector3 mb = new Vector3 (xM, 1, p00.z);
			Vector3 mt = new Vector3 (xM, 1, p11.z);








			divideMap (level - 1, p00, mb, p01, mt, !horiz);
			divideMap (level - 1, mb, p10, mt, p11, !horiz);

			
			Vector3[] line = {mb,mt};
			
			lines.Add (line);

		} else {
			zM = (p11.z + p00.z) / 2;
			zM += range;
			
			zM = (zM >= plane.renderer.bounds.max.z) ? plane.renderer.bounds.max.z : zM;
			zM = (zM <= plane.renderer.bounds.min.z) ? plane.renderer.bounds.min.z : zM;

			Vector3 ml = new Vector3 (p00.x, 1, zM);
			Vector3 mr = new Vector3 (p11.x, 1, zM);



			divideMap (level - 1, p00, p10, ml, mr, !horiz);
			divideMap (level - 1, ml, mr, p01, p11, !horiz);

			Vector3[] line = {ml,mr};
			
			lines.Add (line);

		}
		


	}
	void Update(){
		foreach (Vector3[] l in lines)
			Debug.DrawLine (l [0], l [1], Color.red);
	}
	void createNeighbourhood(int level, float x, float y, Vector3 pos, bool cross){
		if (level == 0) {
			if(Random.Range(0, 100) > courtProb){
				GameObject building = GameObject.CreatePrimitive (PrimitiveType.Cube);
				building.transform.position = new Vector3(pos.x, plane.transform.position.y + building.transform.localScale.y/2, pos.z);
		
				float spaceX = (x/4 >= 1) ? x/4 : 1;
				float spaceY = (y/4 >= 1) ? y/4 : 1;

				building.transform.localScale = new Vector3(x-(spaceX), 1, y-(spaceY));
			}
			return;
		}
		float newX = x;
		float newY = y;
		Vector3 pos1, pos2;

		float range = Random.Range (-neighbourhoodSizeNoise, neighbourhoodSizeNoise);


		if (cross) {
			pos1 = new Vector3 (pos.x - x/4 - range/2 , pos.y, pos.z);
			pos2 = new Vector3 (pos.x + x/4 + range/2, pos.y, pos.z);
			newX = x/2 + range/2;
		} else {
			pos1 = new Vector3 (pos.x , pos.y, pos.z - y/4 - range/2);
			pos2 = new Vector3 (pos.x , pos.y, pos.z + y/4 + range/2);
			newY = y/2 + range/2;
		}

		createNeighbourhood (level - 1, newX, newY, pos1, !cross);
		createNeighbourhood (level - 1, newX, newY, pos2, !cross);
	}
}