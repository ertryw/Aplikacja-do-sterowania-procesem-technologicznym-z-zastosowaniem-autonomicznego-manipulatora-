using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Bone : MonoBehaviour {

    public Color startColor;
    public Color mouseOverColor;
    public Text czlontxt;
    public InputField dlugosctxt;

    public bool isBuild = false;

    // Use this for initialization
    void Start () {
        GameObject main = GameObject.Find("Uklad");
        isBuild = main.transform.tag == "Build";
        if (isBuild)
        {
            czlontxt = GameObject.FindGameObjectWithTag("CzlonTxt").GetComponent<Text>();
            dlugosctxt = GameObject.FindGameObjectWithTag("dlugoscTxt").GetComponent<InputField>();
            Debug.Log("Build");
        }
    }
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localScale.y/2, transform.localPosition.z);
	}

    void OnMouseOver()
    {
        GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
        if (Input.GetMouseButton(0) && isBuild)
        {
            czlontxt.text = transform.parent.name;
            dlugosctxt.text = transform.localScale.y.ToString();
        }

    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.SetColor("_Color", startColor);
    }
}
