using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System;
using UnityEngine.UI;
using System;
using System.IO;

public class Prefs : MonoBehaviour {

    public GameObject imageConnection;
    private InputField ipfiled;
    private InputField portfiled;
    public string dataPath;

    // Use this for initialization

    void Start ()
    {
        SetBrokerPrefs("192.168.1.1", "1883");
        ipfiled = GameObject.Find("InputIP").GetComponent<InputField>();
        portfiled = GameObject.Find("InputPort").GetComponent<InputField>();
        dataPath = Path.Combine(Application.persistentDataPath, "net.txt");
        LoadNet();
    }

    public void ChangePrefs()
    {
        SetBrokerPrefs(ipfiled.text, portfiled.text);
        SaveNetData dat = new SaveNetData();
        dat.BrokerAdres = ipfiled.text;
        dat.BrokerPort = portfiled.text;
        SaveNetData(dat, dataPath);
    }

    private void SetBrokerPrefs(string ip, string port)
    {
        PlayerPrefs.SetString("BrokerAdres", ip);
        PlayerPrefs.SetString("BrokerPort", port);
    }

    public void TestConnectMqtt()
    {
        string ip = PlayerPrefs.GetString("BrokerAdres");
        string port = PlayerPrefs.GetString("BrokerPort");

        MqttClient client = new MqttClient(IPAddress.Parse(ip), int.Parse(port), false, null);
        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId, "unityTest", "", true, 5);

        if (client != null && client.IsConnected)
        {
            imageConnection.GetComponent<Image>().color = Color.green;
        }
        client.Disconnect();
    }


    static void SaveNetData(SaveNetData data, string path)
    {
        string jsonString = JsonUtility.ToJson(data);

        using (StreamWriter streamWriter = File.CreateText(path))
        {
            streamWriter.Write(jsonString);
        }
    }

    static SaveNetData LoadDat(string path)
    {
        using (StreamReader streamReader = File.OpenText(path))
        {
            string jsonString = streamReader.ReadToEnd();
            return JsonUtility.FromJson<SaveNetData>(jsonString);
        }
    }

    public void LoadNet()
    { 
        SaveNetData dat = LoadDat(dataPath);
        SetBrokerPrefs(dat.BrokerAdres, dat.BrokerPort);
        ipfiled.text = dat.BrokerAdres;
        portfiled.text = dat.BrokerPort;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
