using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFAder : MonoBehaviour {

    public Image img;
    

    IEnumerator FadeIn()
    {
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * 0.8f;
            img.color =  new Color(0f,0f,0f,t);
            yield return 0;
        }
    }

    public void fade()
    {
        StartCoroutine(FadeIn());
    }
	void Start () {
        StartCoroutine(FadeIn());
	}
	
	void Update () {
		
	}
}
