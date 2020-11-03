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


public class mqttTest : MonoBehaviour {
	private MqttClient client;
    public GameObject imageConnection;
    public GameObject Target;
    public GameObject cObject;
    public GameObject Base;
    public GameObject IKToggle;
    private Toggle tggl;
    public List<GameObject> Joints = new List<GameObject>();
    public Vector3 TargetchangeVec = new Vector3();
    public Vector3 ObjectPosition = new Vector3();
    float x = 0, y = 0, z = 0;
    public Font m_Font;
    const string EXTERNALDEVICE_TARGETSHIFT = "Ext/Target/Shift";
    const string EXTERNALDEVICE_TARGETRESET = "Ext/Target/Reset";
    const string EXTERNALDEVICE_IKRUN = "Ext/IK/Run";

    string myLog;
    Queue myLogQueue = new Queue();

    const string SYM_TARGETPOSITION  = "Sym/Target/Position";
    const string SYM_JOINTSANGLE = "Sym/Joints/Angle";
    const string SYM_OBJECTPOS = "Sym/Object/Position";

    string Joints_Angle_Text;
    string[] split_Joints_Angles = {"0","45","0" };
    string[] split_Positions;
    // Use this for initialization

    public InputField ipfield;
    public static string IPADRES = "192.168.43.237";

    public void ChanageIP()
    {
        Debug.Log("Zmieniiono");
        IPADRES = ipfield.text;
        client = new MqttClient(IPAddress.Parse(IPADRES), 1883, false, null);
    }

    void Start () {
        
        Joints = GetJointsWihtTag(Base, "JointObject");
        client = new MqttClient(IPAddress.Parse(IPADRES), 1883, false, null);
        //client = new MqttClient(IPAddress.Parse("192.168.8.100"), 1883, false, null);
        Screen.SetResolution(600, 1024, true);
        tggl = IKToggle.GetComponent<Toggle>();
    }
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {

        if (e.Topic == SYM_OBJECTPOS)
        {
            string Position_Text = System.Text.Encoding.UTF8.GetString(e.Message);
            split_Positions = Position_Text.Split(new string[] { " " }, StringSplitOptions.None);
            ObjectPosition = new Vector3(float.Parse(split_Positions[0]), float.Parse(split_Positions[1]), float.Parse(split_Positions[2]));
        }

        if (e.Topic == SYM_TARGETPOSITION)
        {
            string Position_Text = System.Text.Encoding.UTF8.GetString(e.Message);
            split_Positions = Position_Text.Split(new string[] { " " }, StringSplitOptions.None);
            TargetchangeVec = new Vector3(float.Parse(split_Positions[0]), float.Parse(split_Positions[1]), float.Parse(split_Positions[2]));
        }

        if (e.Topic == SYM_JOINTSANGLE)
        {
            Joints_Angle_Text = System.Text.Encoding.UTF8.GetString(e.Message);
            split_Joints_Angles = Joints_Angle_Text.Split(new string[] {" "}, StringSplitOptions.None);
          
        }

        Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message)+" -"+e.Topic+"-");
    }
    GUIStyle customButtonStyle;
    void OnGUI(){
       
        if (customButtonStyle == null)
        {
            customButtonStyle = new GUIStyle(GUI.skin.button);
            customButtonStyle.font = m_Font;
            customButtonStyle.fontSize = 22;
        }


        if (client != null && !client.IsConnected)
        {
            if (GUI.Button(new Rect(10,60,100, 30), "Połącz", customButtonStyle))
            {
                //client = new MqttClient(IPAddress.Parse("192.168.43.237"), 1883, false, null);
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                string clientId = Guid.NewGuid().ToString();
                client.Connect(clientId,"unity","",true,5);
                string[] topic = {"Sym/#"};
                byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE};
                client.Subscribe(topic, qosLevels);
                split_Joints_Angles = new string[] { "0", "45", "0" };

                //client.Subscribe(new string[] { "Target/x" , "Target/y" , "Target/z" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                Debug.Log(client.WillTopic);
            }


            if (IKToggle.active == true)
            {
                IKToggle.SetActive(true);
            }
        }
        else if (client.IsConnected)
        {

            if (GUI.Button(new Rect(10, 130, 240, 30), "Resetuj pozycje celu", customButtonStyle))
            {
                client.Publish(EXTERNALDEVICE_TARGETRESET, System.Text.Encoding.UTF8.GetBytes("1"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }

            if (GUI.Button(new Rect(10, 170, 180, 30), "Uruchom IK", customButtonStyle))
            {
                client.Publish(EXTERNALDEVICE_IKRUN, System.Text.Encoding.UTF8.GetBytes("1"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }

            if (GUI.Button(new Rect(10, 60, 100, 30), "Odłącz", customButtonStyle))
            {
                split_Joints_Angles = new string[] { "0","0","0" };
                SetJointsAngle();
                client.Disconnect();
            }

            if (IKToggle.active == false)
            {
                IKToggle.SetActive(true);
            }

        }
    }

    public void LoadDetector()
    {
        StartCoroutine(LoadYourAsyncScene());
    }
    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("2");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


    void PublishTargetShift()
    {
       // Vector2 v = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal1"), CrossPlatformInputManager.GetAxis("Vertical1"));
       // Vector2 v2 = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal2"), CrossPlatformInputManager.GetAxis("Vertical2"));
       // string txt = (v.x * 0.3f).ToString() + ":" + (v.y * 0.3f).ToString() + ":" + (v2.y * 0.3f).ToString() + ":";
       //  client.Publish(EXTERNALDEVICE_TARGETSHIFT , System.Text.Encoding.UTF8.GetBytes(txt), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
    }

    void targetPos()
    {
        TargetchangeVec = new Vector3(x, y, z);
        Target.transform.SetPositionAndRotation(new Vector3(Target.transform.position.x + TargetchangeVec.x, Target.transform.position.y + TargetchangeVec.y, Target.transform.position.z + TargetchangeVec.z), transform.rotation);
        x = 0;
        y = 0;
        z = 0;
        TargetchangeVec = new Vector3(0, 0, 0);
    }

    void SetTargetPosition()
    {
        //Debug.Log(TargetchangeVec);
        Target.transform.SetPositionAndRotation(new Vector3(TargetchangeVec.x,TargetchangeVec.y,TargetchangeVec.z), transform.rotation);
    }

    void SetJointsAngle()
    {
        for (int i = 0; i < Joints.Count; i++)
        {
            var r = Joints[i].GetComponent<RevoluteJoint>();
            r.UstawKąt = float.Parse(split_Joints_Angles[i]);
        }
    }

    void SetObjectPosition()
    {
        //cObject.transform.SetPositionAndRotation(new Vector3(ObjectPosition.x, ObjectPosition.y, ObjectPosition.z), cObject.transform.rotation);
        cObject.transform.SetPositionAndRotation(new Vector3(5, 0, ObjectPosition.z), cObject.transform.rotation);
    }


    // Update is called once per frame
    void Update () {
        SetObjectPosition();
        if (client != null && client.IsConnected)
        {
            PublishTargetShift();
            imageConnection.GetComponent<Image>().color = Color.green;
            SetTargetPosition();
            SetJointsAngle();


            if (tggl.isOn)
                client.Publish(EXTERNALDEVICE_IKRUN, System.Text.Encoding.UTF8.GetBytes("1"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
           // targetPos();
        }
        else
        {
            imageConnection.GetComponent<Image>().color = Color.red;
        }
    }

    void OnApplicationQuit()
    {
        if (client.IsConnected)
            client.Disconnect();
        client = null;

    }

    public List<GameObject> GetJointsWihtTag(GameObject Basee, string tag)
    {
        List<GameObject> ret = new List<GameObject>();
        bool done = false;
        GameObject g = Basee;
        ret.Add(Basee);
        while (done == false)
        {
            g = GetChildObject(g.transform, tag);
            if (g != null)
            {
                ret.Add(g);
            }
            else
            {
                done = true;
            }
        }
        return ret;
    }

    public GameObject GetChildObject(Transform parent, string _tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == _tag)
            {
                return child.gameObject;
            }
            if (child.childCount > 0)
            {
                GetChildObject(child, _tag);
            }
        }
        return null;
    }
}
