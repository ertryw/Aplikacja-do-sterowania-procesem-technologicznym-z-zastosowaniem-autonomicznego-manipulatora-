using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StaticObject : MonoBehaviour {

    private Text czlontxt;
    private InputField dlugosctxt;
    private InputField paramtxt;
    GameObject main;
    public bool isBuild = false;
    public bool canChange= true;
    public int click = 0;
    public float catchTime = 0.25f;
    public float lastClickTime = 0;
    // Use this for initialization
    void Start () {
        main = GameObject.Find("Uklad");
        isBuild = main.transform.tag == "Build";
        if (isBuild)
        {
            czlontxt = GameObject.FindGameObjectWithTag("CzlonTxt").GetComponent<Text>();
            dlugosctxt = GameObject.FindGameObjectWithTag("dlugoscTxt").GetComponent<InputField>();
            paramtxt = GameObject.FindGameObjectWithTag("paramtxt").GetComponent<InputField>();
            Debug.Log(czlontxt.text);
        }
    }


    void OnCollisionEnter(Collision col)
    {
        if (transform.tag == "Container")
        {
            if (col.gameObject.tag == "Object")
            {
                col.gameObject.GetComponent<ObjectC>().placed = true;
            }
        }
    }


    void OnMouseOver()
    {
        if (Input.GetButtonDown("Fire1") && isBuild)
        {

            if (canChange == true)
            { 
                czlontxt.text = transform.name;
                dlugosctxt.text = transform.localScale.x.ToString();

                if (transform.tag.Contains("Plat"))
                {
                    paramtxt.text = GetComponent<Platform>().speed.ToString();
                    Debug.Log(transform.tag);
                }
            }

            //Debug.Log(Time.time - lastClickTime);
            if (Time.time - lastClickTime < catchTime)
            {
                canChange = false;
                //double click
                //print("done:" + (Time.time - lastClickTime).ToString());
            }
            else
            {
               // canChange = true;
                //normal click
                //print("miss:" + (Time.time - lastClickTime).ToString());
            }
            lastClickTime = Time.time;
        }

    }

    // Update is called once per frame
    void Update () {
		
	}
}
