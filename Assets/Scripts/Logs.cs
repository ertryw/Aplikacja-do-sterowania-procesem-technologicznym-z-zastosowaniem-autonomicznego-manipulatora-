using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class Logs : MonoBehaviour {
	
	string myLog;
	Queue myLogQueue = new Queue();
	public Text label;
	// Use this for initialization
	void Start () {
		
	}

	void OnEnable () {
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable () {
		Application.logMessageReceived -= HandleLog;
	}

	void HandleLog(string logString, string stackTrace, LogType type){
		if (type == LogType.Exception) {
			myLog = logString;

		}
        /*
		if (type == LogType.Warning) {
			myLog = logString;
		}
        */

		if (type == LogType.Error) {
			myLog = logString;
		}

		/*
		string newString = "\n [" + type + "] : " + myLog;
		myLogQueue.Enqueue(newString);
		if (type == LogType.Exception)
		{
			newString = "\n" + stackTrace;
			myLogQueue.Enqueue(newString);
		}
		myLog = string.Empty;
		foreach(string mylog in myLogQueue){
			myLog += mylog;
		}
		*/
	}



	void OnGUI () {

        label.text = myLog;
        //GUI.Label(Rect(650, 650, 300, 50), myLog);

        //	GUILayout.Label
    }

	// Update is called once per frame
	void Update () {
		
	}
}
