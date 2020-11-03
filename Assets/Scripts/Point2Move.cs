using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point2Move : MonoBehaviour {


    public GameObject point1;
    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (transform.name == "PDown")
        { 
        float X = point1.transform.position.x;
        float Y = point1.transform.position.y - 1.5f;
        float Z = point1.transform.position.z;
        transform.position = new Vector3(X, Y, Z);
        }
        if (transform.name == "PUp")
        {
            float X = point1.transform.position.x;
            float Y = point1.transform.position.y + 1.5f;
            float Z = point1.transform.position.z;
            transform.position = new Vector3(X, Y, Z);
        }
        if (transform.name == "PRight")
        {
            float X = point1.transform.position.x + 1.5f;
            float Y = point1.transform.position.y;
            float Z = point1.transform.position.z;
            transform.position = new Vector3(X, Y, Z);
        }
        if (transform.name == "PLeft")
        {
            float X = point1.transform.position.x - 1.5f;
            float Y = point1.transform.position.y;
            float Z = point1.transform.position.z;
            transform.position = new Vector3(X, Y, Z);
        }
        if (transform.name == "PRightZ")
        {
            float X = point1.transform.position.x;
            float Y = point1.transform.position.y;
            float Z = point1.transform.position.z + 1.5f;
            transform.position = new Vector3(X, Y, Z);
        }
        if (transform.name == "PLeftZ")
        {
            float X = point1.transform.position.x;
            float Y = point1.transform.position.y;
            float Z = point1.transform.position.z - 1.5f;
            transform.position = new Vector3(X, Y, Z);
        }

    }
}
