using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using UnityEngine.UI;
using System.Threading;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class mqttsym : MonoBehaviour
{
    private MqttClient client;
    public GameObject imageConnection;
    public CCamera cameraClass;
    string ip, port;

    public GameObject object1;
    public GameObject object2;
    public GameObject object3;


    const string SYM_TARGETPOSITION = "Sym/Target/Position";
    const string SYM_JOINTSANGLE = "Sym/Joints/Angle";
    const string SYM_OBJECTPOS = "Sym/Object/Position";

    // Use this for initialization
    void Start()
    {

        ip = PlayerPrefs.GetString("BrokerAdres");
        port = PlayerPrefs.GetString("BrokerPort");
        client = new MqttClient(IPAddress.Parse(ip), int.Parse(port), false, null);
        Debug.Log(ip);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void mqttConnect()
    {
        client.MqttMsgPublishReceived += clientReceived;
        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId, "unity", "", true, 5);
        if (client != null && client.IsConnected)
        {
            string[] topic = { "Sym/#" };
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE };
            client.Subscribe(topic, qosLevels);

            imageConnection.GetComponent<Image>().color = Color.green;
        }
    }


    public void clientReceived(object sender, MqttMsgPublishEventArgs e)
    {
        Debug.Log(e.Topic);
        if (e.Topic == SYM_OBJECTPOS)
        {
            Debug.Log(e.Topic);
            string Position_Text = System.Text.Encoding.UTF8.GetString(e.Message);
            string[] split_Positions = Position_Text.Split(new string[] { " " }, StringSplitOptions.None);
            Vector3 pos = new Vector3(float.Parse(split_Positions[0]), float.Parse(split_Positions[1]), float.Parse(split_Positions[2]));

            //cameraClass.SpawnObject(pos,object1);
            cameraClass.ObjPos = pos;
            cameraClass.spawn = true;
        }
    }

}

