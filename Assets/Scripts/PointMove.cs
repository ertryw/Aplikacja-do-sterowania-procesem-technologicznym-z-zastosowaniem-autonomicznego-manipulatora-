using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointMove : MonoBehaviour {

    public Slider SliderX;
    public Slider SliderY;
    public Slider SliderZ;
    public GameObject main;

    // Use this for initialization
    void Start () {
        transform.localPosition = new Vector3(0, 0, 0);
    }

	// Update is called once per frame
	void Update () {

        if (!main.GetComponent<MainSymProgram>().RunBool)
        {

            float X = SliderX.value - 50;
            float Y = SliderY.value - 50;
            float Z = SliderZ.value - 50;
            transform.position = new Vector3(X*0.5f, Y * 0.5f, Z * 0.5f);
        }
	}
}
