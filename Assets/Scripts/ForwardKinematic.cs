using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ForwardKinematic : MonoBehaviour {

    public GameObject Base;
    public List<GameObject> Joints = new List<GameObject>();
    public GameObject Końcówka;
    public Vector3 KońcówkaPos;
    public Vector3 BasePose;

    void Start() {

    }



    void Update() {
        //Joints =   GameObject.FindGameObjectsWithTag("JointObject");
        Joints.Clear();

        if (Base != null)
        {
            Joints = GetJointsWihtTag(Base, "JointObject");
            GetNextJointsPosition();
            KońcówkaPos = Końcówka.transform.position;
            if (Vector3.Magnitude(BasePose) == 0)
                BasePose = new Vector3(KońcówkaPos.x, KońcówkaPos.y + 0.15f, KońcówkaPos.z);
        }

    }


    public List<Vector3> NextJointsPosition = new List<Vector3>();
    public List<Matrix4x4> HMs = new List<Matrix4x4>();
    public List<Matrix4x4> cHMs = new List<Matrix4x4>();

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


    public Vector3 GetJointPosition(int j)
    {
        HMs.Clear();
        cHMs.Clear();


        Matrix4x4 HMG = Matrix4x4.identity;
        for (int i = 0; i < j; i++)
        {
            var r = Joints[i].GetComponent<RevoluteJoint>();
  
                HMG *= r.HM;
                //Debug.Log("Macierz jednorodna");
                //Debug.Log(r.HM);
        }

        //Debug.Log(j);

        return HMG.MultiplyPoint(Joints[0].transform.position);
    }

    public Matrix4x4 GetHomogeneusFrom0(int j)
    {
        HMs.Clear();
        cHMs.Clear();

        Matrix4x4 HMG = Matrix4x4.identity;
        for (int i = 0; i < j; i++)
        {
            var r = Joints[i].GetComponent<RevoluteJoint>();
            if (r.innerTag == "HorizontalRevoluteJoint")
            {
                Vector3 transl = r.HM.GetColumn(3);
                transl.x = 0;
                transl.y = r.długośćKości * Mathf.Cos(r.BierzącyKąt * Mathf.Deg2Rad);
                transl.z = r.długośćKości * Mathf.Sin(r.BierzącyKąt * Mathf.Deg2Rad);
                r.HM.SetColumn(3, new Vector4(transl.x, transl.y, transl.z, 1));
                HMG *= r.HM;
            }
            else
            {
                HMG *= r.HM;
            }
        }
        return HMG;
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Screenmanager Resolution Width", 800);
        PlayerPrefs.SetInt("Screenmanager Resolution Height", 600);
        PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", 0);
    }

    public void GetNextJointsPosition()
    {
        HMs.Clear();
        cHMs.Clear();
        NextJointsPosition.Clear();

        Matrix4x4 HMG = Matrix4x4.identity;
        for (int i = 0; i < Joints.Count; i++)
        {
            var r = Joints[i].GetComponent<RevoluteJoint>();
            if (r.innerTag == "HorizontalRevoluteJoint")
            { 
                Vector3 transl = r.HM.GetColumn(3);
                transl.x = 0;
                transl.y = r.długośćKości * Mathf.Cos(r.BierzącyKąt * Mathf.Deg2Rad);
                transl.z = r.długośćKości * Mathf.Sin(r.BierzącyKąt * Mathf.Deg2Rad);
                r.HM.SetColumn(3, new Vector4(transl.x, transl.y, transl.z, 1));
                HMG *= r.HM;
            }
            else
            {
                HMG *= r.HM;
            }
        }

        for (int i = Joints.Count - 1; i > 0; i--)
        {
            NextJointsPosition.Add(GetJointPosition(Joints.Count - i));
        }

            //NextJointsPosition.Add(GetJointPosition(Joints.Count - 3));
        //NextJointsPosition.Add(GetJointPosition(Joints.Count - 2));
        //NextJointsPosition.Add(GetJointPosition(Joints.Count - 1));
        NextJointsPosition.Add(HMG.MultiplyPoint(Joints[0].transform.position));


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
