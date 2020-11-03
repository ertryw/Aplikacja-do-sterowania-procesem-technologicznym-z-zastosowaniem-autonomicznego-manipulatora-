using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCamera : MonoBehaviour {

    public GameObject PointS;

    public GameObject Obj;
    public Vector3 ObjPos;
    public Object objTag;

    public GameObject mObject1;
    public GameObject mObject2;
    public GameObject mObject3;

    public GameObject SpawnedObj;
    public bool isBuild = false;
    private Text czlontxt;
    GameObject main;
    public bool spawn;

    // Use this for initialization
    void Start () {
        main = GameObject.Find("Uklad");
        isBuild = main.transform.tag == "Build";
        if (isBuild)
        {
            czlontxt = GameObject.FindGameObjectWithTag("CzlonTxt").GetComponent<Text>();
            Debug.Log(czlontxt.text);
        }
        //Debug.Log(transform.rotation.eulerAngles.);
        var rot = PointS.transform.localRotation;
        PointS.transform.localRotation = Quaternion.Inverse(transform.rotation);//rot * new Quaternion(transform.rotation.eulerAngles.x * -1, transform.rotation.eulerAngles.y * -1, transform.rotation.eulerAngles.z * -1, 0); // this is 90 degrees around y axis
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButton(0) && isBuild)
        {
            czlontxt.text = transform.name;
        }

    }



    public void SpawnObject(Vector3 position, GameObject objj)
    {
        SpawnedObj = Instantiate(objj, new Vector3(0,0,0) , Quaternion.identity);
        SpawnedObj.transform.parent = main.transform;
    }


    // Update is called once per frame
    void Update()
    {
        if (spawn)
        {
            SpawnObject(ObjPos, mObject1);
            spawn = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "CameraCap")
                {
                    int r = Random.Range(0, 3);
                    GameObject obj = new GameObject();
                    if (r == 0)
                    {
                        SpawnObject(ObjPos, mObject1);
                       // obj = Instantiate(mObject1, new Vector3(startpoint.position.x, startpoint.position.y, startpoint.position.z + Random.Range(-2, 2)), Quaternion.identity);
                    }
                    if (r == 1)
                    {
                        SpawnObject(ObjPos, mObject2);

                    }
                    if (r == 2)
                    {
                        SpawnObject(ObjPos, mObject3);
                    }

                    //SpawnObject(ObjPos);
                }
            }
        }
    }
}
