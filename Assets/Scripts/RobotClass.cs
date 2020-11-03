using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;


public class RobotClass : MonoBehaviour {

    public GameObject RevoluteVertical;
    public GameObject HorizontalRevolute;
    public GameObject Effector;
    public GameObject Platform;
    public GameObject Container;
    public GameObject CameraObj;
    public GameObject WorkPlace;
    public GameObject mqtt;

    public Vector3 Last_EndJointPosition;
    public GameObject[] limbs;
    public int limsLimit = 6;
    public int lastlimbindex = 0;
    public int lastjointindex = 0;

    string dataPath;
    SaveData dat;
    public bool EndEffector = false;
    public static EndEffector EffectorClass;

    public EndEffector EffectorClass2;

    public GameObject Point2;
    private InputField dlugosctxt;
    private Text czlontxt;
    public bool isBuild = false;
    GameObject main;
    void Start () {
        limbs = new GameObject[limsLimit];
        dataPath = Path.Combine(Application.persistentDataPath, "robot.txt");

        main = GameObject.Find("Uklad");
        isBuild = main.transform.tag == "Build";
        if (isBuild)
        {
            czlontxt = GameObject.FindGameObjectWithTag("CzlonTxt").GetComponent<Text>();
            dlugosctxt = GameObject.FindGameObjectWithTag("dlugoscTxt").GetComponent<InputField>();
            Debug.Log("Build");
        }
        Debug.Log(dataPath);
    }

    public void SaveRobot()
    {
        SaveData dat = new SaveData();
        dat.ElementsCount = lastlimbindex;
        dat.ElementName = new string[lastlimbindex];
        dat.ElementLength = new float[lastlimbindex];
        dat.ElementInnerTag = new string[lastlimbindex];
        dat.ElementX = new float[lastlimbindex];
        dat.ElementY = new float[lastlimbindex];
        dat.ElementZ = new float[lastlimbindex];
        dat.ElementRotX = new float[lastlimbindex];
        dat.ElementRotY = new float[lastlimbindex];
        dat.ElementRotZ = new float[lastlimbindex];
        dat.Param1 = new float[lastlimbindex];

        for (int i = 0; i < lastlimbindex; i++)
        {
            GameObject limb = limbs[i];
            dat.ElementName[i] = limb.transform.name;

            if (limb.transform.name.Contains("Cz"))
            {
                dat.ElementLength[i] = limb.GetComponent<RevoluteJoint>().długośćKości;
                dat.ElementInnerTag[i] = limb.GetComponent<RevoluteJoint>().innerTag;
            }
            else if (limb.transform.name == "Efektor")
            {
                dat.ElementLength[i] = 0;
                dat.ElementInnerTag[i] = limb.tag;
            }
            else if (limb.transform.name.Contains("Platform"))
            {
                dat.ElementLength[i] = limb.GetComponent<Platform>().transform.localScale.x;
                dat.ElementX[i] = limb.transform.position.x;
                dat.ElementY[i] = limb.transform.position.y;
                dat.ElementZ[i] = limb.transform.position.z;
                dat.ElementRotX[i] = limb.transform.rotation.eulerAngles.x;
                dat.ElementRotY[i] = limb.transform.rotation.eulerAngles.y;
                dat.ElementRotZ[i] = limb.transform.rotation.eulerAngles.z;
                dat.Param1[i] = limb.GetComponent<Platform>().speed;

                dat.ElementInnerTag[i] = limb.tag;
            }
            else if (limb.transform.name.Contains("Container") || limb.transform.name.Contains("Camera") || limb.transform.name.Contains("WorkPlace"))
            {
                Debug.Log(limb.transform.name);
                dat.ElementLength[i] = limb.transform.localScale.x;
                dat.ElementX[i] = limb.transform.position.x;
                dat.ElementY[i] = limb.transform.position.y;
                dat.ElementZ[i] = limb.transform.position.z;
                dat.ElementRotX[i] = limb.transform.rotation.eulerAngles.x;
                dat.ElementRotY[i] = limb.transform.rotation.eulerAngles.y;
                dat.ElementRotZ[i] = limb.transform.rotation.eulerAngles.z;
                dat.ElementInnerTag[i] = limb.tag;
            }
        }
        SaveDat(dat, dataPath);
    }

    public void LoadRobot()
    {
        dat = new SaveData();
        dat = LoadDat(dataPath);
        StartCoroutine(LoadRobotLoop());
    }





    public void AddLimbVertical()
    {
        if (lastjointindex == 0)
        {
            GameObject limb = Instantiate(RevoluteVertical, transform.position, Quaternion.identity) as GameObject;
            limb.name = "Człon " + lastlimbindex.ToString();
            limb.transform.parent = transform;
            limbs[lastlimbindex] = limb;
            lastlimbindex++;
            lastjointindex++;
        }
        else
        {
            GameObject limb = Instantiate(RevoluteVertical, Last_EndJointPosition, Quaternion.identity) as GameObject;
            limb.name = "Człon " + lastlimbindex.ToString();
            limb.transform.parent = limbs[FindLastJointIndex()].transform;
            limbs[lastlimbindex] = limb;
            lastlimbindex++;
            lastjointindex++;
        }
    }

    public void AddLimbHorizontal()
    {
        if (lastjointindex == 0)
        {
            GameObject limb = Instantiate(HorizontalRevolute, transform.position, Quaternion.identity) as GameObject;
            limb.name = "Człon " + lastlimbindex.ToString();
            limb.transform.parent = transform;
            limbs[lastlimbindex] = limb;
            lastlimbindex++;
            lastjointindex++;
        }
        else
        {
            GameObject limb = Instantiate(HorizontalRevolute, Last_EndJointPosition, Quaternion.identity) as GameObject;
            limb.name = "Człon " + lastlimbindex.ToString();
            limb.transform.parent = limbs[FindLastJointIndex()].transform;
            limbs[lastlimbindex] = limb;
            lastlimbindex++;
            lastjointindex++;
        }
    }

    public void AddEffector()
    {
        if (lastlimbindex != 0)
        {
            GameObject limb = Instantiate(Effector, Last_EndJointPosition, Quaternion.identity) as GameObject;
            limb.name = "Efektor";
            limb.transform.parent = limbs[FindLastJointIndex()].transform;
            limb.GetComponent<EndEffector>().Target = Point2;

            //if (limbs[0].GetComponent<RevoluteJoint>().innerTag == "VerticalRevoluteJoint")
            //    limb.GetComponent<EndEffector>().DefaultLength = Vector3.Magnitude(limb.transform.position);//- limbs[0].GetComponent<RevoluteJoint>().długośćKości;
            limbs[lastlimbindex] = limb;
            EffectorClass = limb.GetComponent<EndEffector>();
            lastlimbindex++;
            EndEffector = true;
        }
    }

    public void AddPlatform(float speed)
    {
            GameObject limb = Instantiate(Platform, new Vector3(0,0,0), Quaternion.identity) as GameObject;
            limb.name = "Platform" + lastlimbindex.ToString();
            limb.transform.parent = main.transform;
            limb.GetComponent<Platform>().speed = speed;

            limbs[lastlimbindex] = limb;
            lastlimbindex++;
    }

    public void AddWorkPalce()
    {
        GameObject limb = Instantiate(WorkPlace, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        limb.name = "WorkPlace" + lastlimbindex.ToString();
        limb.transform.parent = main.transform;

        limbs[lastlimbindex] = limb;
        lastlimbindex++;
    }

    public void AddContainer()
    {
        GameObject limb = Instantiate(Container, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        limb.name = "Container" + lastlimbindex.ToString();
        limb.transform.parent = main.transform;

        limbs[lastlimbindex] = limb;
        lastlimbindex++;
    }

    public void AddCamera()
    {
        GameObject limb = Instantiate(CameraObj, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        limb.name = "Camera" + lastlimbindex.ToString();
        limb.transform.parent = main.transform;

        limbs[lastlimbindex] = limb;
        lastlimbindex++;
    }

    public void ChangeLength(float length, string czlontxt)
    {
        //Debug.Log("Zmiana długości członu na " + length.ToString());
        try
        {
            GameObject obj = GameObject.Find(czlontxt);
            Transform child = obj.transform.GetChild(1);
            if (child.name != "Bone")
                child = obj.transform.GetChild(0);

            child.transform.localScale = new Vector3(child.localScale.x, length, child.localScale.z);
            //Debug.Log(child.name);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void ChangePlatformLength(float length, string czlontxt)
    {
        //Debug.Log("Zmiana długości(platformy) członu na " + length.ToString());
        try
        {
            GameObject obj = GameObject.Find(czlontxt);
            obj.transform.localScale = new Vector3(length, obj.transform.localScale.y, obj.transform.localScale.z);
            //Debug.Log(obj.name);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        if (czlontxt.Contains("WorkPlace"))
        {
            //Debug.Log("Zmiena workplace");
            try
            {
                GameObject obj = GameObject.Find(czlontxt);
                obj.transform.localScale = new Vector3(length,length,length);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

    }

    public void ChangePlatformPosition(Vector3 pos, string czlontxt)
    {
        //Debug.Log("Zmiana pozycjiplatformy  " + pos.ToString());
        try
        {
            GameObject obj = GameObject.Find(czlontxt);
            obj.transform.position = pos;
            //Debug.Log(obj.name);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void ChangeRotation(Vector3 rot, string czlontxt)
    {
        //Debug.Log("Zmiana rotacji " + rot.ToString());
        try
        {
            GameObject obj = GameObject.Find(czlontxt);
            obj.transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
           // Debug.Log(obj.name);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }


    public int FindLastJointIndex()
    {
        int z = 0;
        for (int i = 0; i < lastlimbindex; i++)
        {
            GameObject limb = limbs[i];
            if (limb.transform.name.Contains("Cz"))
            {
                z = i;
            }
        }
        return z;
    }

    public int FindFirstJointIndex()
    {
        int z = 0;
        for (int i = 0; i < lastlimbindex; i++)
        {
            GameObject limb = limbs[i];
            if (limb.transform.name.Contains("Cz"))
            {
                return z = i;
            }
        }
        return -1;
    }

    public GameObject FindEfectorObj()
    {
        int z = 0;
        for (int i = 0; i < lastlimbindex; i++)
        {
            GameObject limb = limbs[i];
            if (limb.transform.name.Contains("Efektor"))
            {
                z = i;
            }
        }
        return limbs[z];
    }

    public void DeleteLastJoint()
    {
        Destroy(limbs[lastlimbindex - 1], 0.1f);
        lastlimbindex--;
        if (EndEffector == true)
        {
            EndEffector = false;
        }
    }
    
    public IEnumerator LoadRobotLoop()
    {
        for (int i = 0; i < dat.ElementsCount; i++)
        {
            yield return new WaitForSeconds(0.4f);
            if (dat.ElementInnerTag[i] == "VerticalRevoluteJoint")
            {
                AddLimbVertical();
                ChangeLength(dat.ElementLength[i], dat.ElementName[i]);
            }
            else if (dat.ElementInnerTag[i] == "HorizontalRevoluteJoint")
            {
                AddLimbHorizontal();
                ChangeLength(dat.ElementLength[i], dat.ElementName[i]);
            }
            else if (dat.ElementInnerTag[i] == "Platform")
            {
                AddPlatform(dat.Param1[i]);
                ChangePlatformLength(dat.ElementLength[i], dat.ElementName[i]);
                ChangePlatformPosition(new Vector3(dat.ElementX[i], dat.ElementY[i], dat.ElementZ[i]), dat.ElementName[i]);
                ChangeRotation(new Vector3(dat.ElementRotX[i], dat.ElementRotY[i], dat.ElementRotZ[i]), dat.ElementName[i]);
            }
            else if (dat.ElementInnerTag[i] == "Container")
            {
                AddContainer();
                ChangePlatformPosition(new Vector3(dat.ElementX[i], dat.ElementY[i], dat.ElementZ[i]), dat.ElementName[i]);
            }
            else if (dat.ElementInnerTag[i] == "CCamera")
            {
                AddCamera();
                ChangePlatformPosition(new Vector3(dat.ElementX[i], dat.ElementY[i], dat.ElementZ[i]), dat.ElementName[i]);
                ChangeRotation(new Vector3(dat.ElementRotX[i], dat.ElementRotY[i], dat.ElementRotZ[i]), dat.ElementName[i]);
                yield return new WaitForSeconds(0.4f);
                if (isBuild == false)
                    mqtt.GetComponent<mqttsym>().cameraClass = limbs[lastlimbindex - 1].GetComponent<CCamera>();
            }
            else if (dat.ElementInnerTag[i] == "workplace")
            {
                AddWorkPalce();
                ChangePlatformLength(dat.ElementLength[i], dat.ElementName[i]);
                ChangePlatformPosition(new Vector3(dat.ElementX[i], dat.ElementY[i], dat.ElementZ[i]), dat.ElementName[i]);
            }
            else if (dat.ElementInnerTag[i] == "Finger")
            {
                AddEffector();
            }
        }
        yield return new WaitForSeconds(1f);
    }

    static SaveData LoadDat(string path)
    {
        using (StreamReader streamReader = File.OpenText(path))
        {
            string jsonString = streamReader.ReadToEnd();
            return JsonUtility.FromJson<SaveData>(jsonString);
        }
    }

    static void SaveDat(SaveData data, string path)
    {
        string jsonString = JsonUtility.ToJson(data);

        using (StreamWriter streamWriter = File.CreateText(path))
        {
            streamWriter.Write(jsonString);
        }
    }

    void Update () {

        if (lastlimbindex > 0 && EndEffector != true)
        {
            //Debug.Log(FindLastJointIndex());
            RevoluteJoint joint = limbs[FindLastJointIndex()].GetComponent<RevoluteJoint>();
            Last_EndJointPosition = joint.NextJointPosition;
        }
        if (EndEffector == true)
        {

            if (EffectorClass2 == null)
            {
                EffectorClass2 = FindEfectorObj().GetComponent<EndEffector>();
            }

            RevoluteJoint joint = limbs[FindLastJointIndex()].GetComponent<RevoluteJoint>();
            Last_EndJointPosition = joint.NextJointPosition;
            EffectorClass.LastPosition = limbs[FindLastJointIndex()].transform.position;
           // Debug.Log(FindLastJointIndex());
        }
    }
}
