using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Platform : MonoBehaviour {

    public GameObject belt;
    public Transform endpoint;
    public Transform startpoint;
    public float speed;
    public bool stay;
    public float speed2;

    public GameObject mObject1;
    public GameObject mObject2;
    public GameObject mObject3;

    private Text czlontxt;
    private InputField dlugosctxt;
    public bool isBuild = false;
    GameObject main;
    // Use this for initialization
    void Start () {
        main = GameObject.Find("Uklad");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Object")
            StartCoroutine(move(other, speed2));
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Object")
        {
            other.gameObject.GetComponent<ObjectC>().CanPick = false;
            other.gameObject.GetComponent<ObjectC>().CanMove = false;
        }
     }



    private IEnumerator move(Collider other, float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            if (other.gameObject.GetComponent<ObjectC>().CanMove)
                other.gameObject.GetComponent<Rigidbody>().MovePosition(Vector3.MoveTowards(other.transform.position, endpoint.position, speed * Time.deltaTime));
                    //other.transform.position = Vector3.MoveTowards(other.transform.position, endpoint.position, speed * Time.deltaTime);

            stay = false;
            yield return new WaitForSeconds(0.1f);
            stay = true;
        }
    }


    void Update () {
        if (Input.GetMouseButtonDown(0))
        { 
            Ray ray =   Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Cylinder")
                {
                int r = Random.Range(0, 3);
                    GameObject obj = new GameObject();
                    if (r == 0)
                    {
                        obj = Instantiate(mObject1, new Vector3(startpoint.position.x, startpoint.position.y, startpoint.position.z + Random.Range(-2, 2)), Quaternion.identity);
                    }
                    if (r == 1)
                    {
                        obj = Instantiate(mObject2, new Vector3(startpoint.position.x, startpoint.position.y, startpoint.position.z + Random.Range(-2, 2)), Quaternion.identity);
                        var rot = obj.transform.rotation;
                        obj.transform.rotation = rot * Quaternion.Euler(90, 0, 0); // this is 90 degrees around y axis
                    }
                    if (r == 2)
                    {
                        obj = Instantiate(mObject3, new Vector3(startpoint.position.x, startpoint.position.y, startpoint.position.z + Random.Range(-2, 2)), Quaternion.identity);
                    }

                    obj.transform.parent = main.transform;

                }
            }
        }
    }
}
