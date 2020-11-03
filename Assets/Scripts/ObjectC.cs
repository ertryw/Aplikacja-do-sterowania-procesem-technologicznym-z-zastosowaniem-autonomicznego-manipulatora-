using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Object {nullobj, object1, object2, object3 };

public class ObjectC : MonoBehaviour {

    public bool CanPick;
    public float heigth;
    public bool picked;
    public bool placed;
    public bool CanMove;
    public EndEffector effectorpos;
    public Object objectTag;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(picked)
        {
            transform.position = effectorpos.center.transform.position;
        }
		
	}
}
