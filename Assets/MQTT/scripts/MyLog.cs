using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MyLog : MonoBehaviour
{
    string myLog;
    Queue myLogQueue = new Queue();
    public Text txt;

    void Start()
    {
        InvokeRepeating("Clean", 2.0f,5.0F);
    }

    void Clean()
    {
        myLog = "";
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            myLog = logString;
            string newString = System.DateTime.Now + " \n [" + type + "] : " + myLog;
            myLogQueue.Enqueue(newString);

                myLog = string.Empty;
                foreach (string mylog in myLogQueue)
                {
                    myLog += mylog;
                }

        }
    }

    void Update()
    {
        txt.text = myLog;
    }
}