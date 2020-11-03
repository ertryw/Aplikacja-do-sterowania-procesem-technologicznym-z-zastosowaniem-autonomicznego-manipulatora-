using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkSpace : MonoBehaviour {

    public ObjectC ObjC;
    GameObject Obj;
    // Use this for initialization
    void Start () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
         Debug.Log("entered");
        Obj = other.gameObject;
        if (Obj.transform.tag == "Object")
        {
            ObjC = Obj.GetComponent<ObjectC>();
            ObjC.CanPick = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Obj.transform.tag == "Object")
        {
            ObjC.CanPick = false;
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
