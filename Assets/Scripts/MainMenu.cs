using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}


    IEnumerator LoadKonf()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scene2");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }



    IEnumerator LoadSym()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("sym1");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void LoadBuild()
    {
        StartCoroutine(LoadBuild1());
    }
    public void Loadsym1()
    {
        StartCoroutine(LoadSym());
    }


    IEnumerator LoadBuild1()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("build1");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}
