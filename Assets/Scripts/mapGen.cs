using UnityEngine;
using System.Collections;

public class mapGen : MonoBehaviour {
	public float dim;
	public int divisionDim;
	public float courtProb;
	public float neighbourhoodSizeNoise;

	float MIN_BUILDING_DIM = 1;

	public GameObject plane;

	ArrayList lines = new ArrayList();
	public ArrayList buildingSizes = new ArrayList();
	public ArrayList buildingPositions = new ArrayList();

	// Use this for initialization
	void Start () {
		plane = GameObject.CreatePrimitive (PrimitiveType.Plane) as GameObject;
		plane.transform.position = Vector3.zero;
		plane.transform.localScale = new Vector3(dim/10, 1, dim/10);
		plane.tag = "city";

		GameObject w = GameObject.CreatePrimitive (PrimitiveType.Cube) as GameObject;
		GameObject e = GameObject.CreatePrimitive (PrimitiveType.Cube) as GameObject;
		GameObject n = GameObject.CreatePrimitive (PrimitiveType.Cube) as GameObject;
		GameObject s = GameObject.CreatePrimitive (PrimitiveType.Cube) as GameObject;

		n.transform.position = new Vector3 (0, 1, plane.renderer.bounds.max.z);
		s.transform.position = new Vector3 (0, 1, plane.renderer.bounds.min.z);
		e.transform.position = new Vector3 (plane.renderer.bounds.max.x, 1, 0);
		w.transform.position = new Vector3 (plane.renderer.bounds.min.x, 1, 0);

		n.transform.localScale = new Vector3 (plane.renderer.bounds.max.x - plane.renderer.bounds.min.x, 3, .1f);
		s.transform.localScale = new Vector3 (plane.renderer.bounds.max.x - plane.renderer.bounds.min.x, 3, .1f);
		e.transform.localScale = new Vector3 (.1f, 3, plane.renderer.bounds.max.z - plane.renderer.bounds.min.z);
		w.transform.localScale = new Vector3 (.1f, 3, plane.renderer.bounds.max.z - plane.renderer.bounds.min.z);

		n.tag = "city";
		s.tag = "city";
		e.tag = "city";
		w.tag = "city";


		Vector3 p00 = plane.renderer.bounds.min;
		Vector3 p10 = new Vector3(plane.renderer.bounds.max.x , 0 , plane.renderer.bounds.min.z);
		Vector3 p01 = new Vector3(plane.renderer.bounds.min.x, 0, plane.renderer.bounds.max.z);
		Vector3 p11 = plane.renderer.bounds.max;


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

				xMag -= Random.Range(1,5); 
				zMag -= Random.Range(1,5);

				xMag = xMag < MIN_BUILDING_DIM ? MIN_BUILDING_DIM : xMag;
				zMag = zMag < MIN_BUILDING_DIM ? MIN_BUILDING_DIM : zMag;


				GameObject building = GameObject.CreatePrimitive (PrimitiveType.Cube);

				building.transform.localScale = new Vector3(xMag, yMag, zMag);



				building.transform.position = new Vector3(x,building.transform.localScale.y/2,z);
				building.name = "Building";
				building.tag = "city";

				buildingPositions.Add(building.transform.position);
				buildingSizes.Add(building.transform.localScale);
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

			Vector3 mb = new Vector3 (xM, 0, p00.z);
			Vector3 mt = new Vector3 (xM, 0, p11.z);


			divideMap (level - 1, p00, mb, p01, mt, !horiz);
			divideMap (level - 1, mb, p10, mt, p11, !horiz);

			
			Vector3[] line = {mb,mt};
			
			lines.Add (line);

		} else {
			zM = (p11.z + p00.z) / 2;
			zM += range;
			
			zM = (zM >= plane.renderer.bounds.max.z) ? plane.renderer.bounds.max.z : zM;
			zM = (zM <= plane.renderer.bounds.min.z) ? plane.renderer.bounds.min.z : zM;

			Vector3 ml = new Vector3 (p00.x, 0, zM);
			Vector3 mr = new Vector3 (p11.x, 0, zM);



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

}
