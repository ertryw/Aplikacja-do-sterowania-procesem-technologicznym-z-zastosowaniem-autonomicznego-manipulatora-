using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class MainBuildProgram : MonoBehaviour {

    private RobotClass robot;
    private InputField dlugosctxt;
    private Text czlontxt;

    private Slider sliderX;
    private Slider sliderY;
    private Slider sliderZ;


    private InputField RotX;
    private InputField RotY;
    private InputField RotZ;

    private InputField Param;

    void Start () {
        robot = GetComponent<RobotClass>();
        czlontxt = GameObject.FindGameObjectWithTag("CzlonTxt").GetComponent<Text>();
        dlugosctxt = GameObject.FindGameObjectWithTag("dlugoscTxt").GetComponent<InputField>();
        sliderX = GameObject.Find("SliderX").GetComponent<Slider>();
        sliderY = GameObject.Find("SliderY").GetComponent<Slider>();
        sliderZ = GameObject.Find("SliderZ").GetComponent<Slider>();

        RotX = GameObject.Find("InputX").GetComponent<InputField>();
        RotY = GameObject.Find("InputY").GetComponent<InputField>();
        RotZ = GameObject.Find("InputZ").GetComponent<InputField>();
        Param = GameObject.Find("InputParam").GetComponent<InputField>();
    }


    public void ChangePos()
    {
        GameObject obj = GameObject.Find(czlontxt.text);
        if (!czlontxt.text.Contains("Cz"))
        {
            obj.transform.position = new Vector3(sliderX.value - 10, sliderY.value, sliderZ.value - 10);
        }
    }

    public void ChangeRotation()
    {
        GameObject obj = GameObject.Find(czlontxt.text);
        if (czlontxt.text.Contains("Platform")|| czlontxt.text.Contains("Camera"))
        {
            obj.transform.rotation = Quaternion.Euler(int.Parse(RotX.text), int.Parse(RotY.text), int.Parse(RotZ.text));
        }
    }

    public void ChangeParam()
    {
        if (czlontxt.text.Contains("Platform"))
        {
            GameObject obj = GameObject.Find(czlontxt.text);
            obj.GetComponent<Platform>().speed = float.Parse(Param.text);
        }
    }

    public void ChangeLength()
    {
        Debug.Log("Zmiana długości " + czlontxt.text + "- na " + float.Parse(dlugosctxt.text));
        if (czlontxt.text.Contains("Cz"))
        {
            Debug.Log("Zmiana członu");
            try
            {
                GameObject obj = GameObject.Find(czlontxt.text);
                Transform child = obj.transform.GetChild(1);
                if (child.name != "Bone")
                    child = obj.transform.GetChild(0);

                //child.transform.localScale.Set(child.localScale.x, float.Parse(dlugosctxt.text), child.localScale.z);
                child.transform.localScale = new Vector3(child.localScale.x, float.Parse(dlugosctxt.text), child.localScale.z);
                Debug.Log(child.name);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        if (czlontxt.text.Contains("Platform"))
        {
            Debug.Log("Zmiena platfromy");
            try
            {
                GameObject obj = GameObject.Find(czlontxt.text);
                //child.transform.localScale.Set(child.localScale.x, float.Parse(dlugosctxt.text), child.localScale.z);
                obj.transform.localScale = new Vector3(float.Parse(dlugosctxt.text), obj.transform.localScale.y, obj.transform.localScale.z);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        if (czlontxt.text.Contains("WorkPlace"))
        {
            Debug.Log("Zmiena workplace");
            try
            {
                GameObject obj = GameObject.Find(czlontxt.text);
                //child.transform.localScale.Set(child.localScale.x, float.Parse(dlugosctxt.text), child.localScale.z);
                obj.transform.localScale = new Vector3(float.Parse(dlugosctxt.text), float.Parse(dlugosctxt.text), float.Parse(dlugosctxt.text));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }


    }


    // Update is called once per frame
    void Update () {
		
	}
}
