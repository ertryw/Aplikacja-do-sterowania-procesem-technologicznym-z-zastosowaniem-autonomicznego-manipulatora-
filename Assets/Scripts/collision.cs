using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class collision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name != "Base")
        {
            transform.parent.GetComponent<EndEffector>().Close = false;

        }

    }
}
