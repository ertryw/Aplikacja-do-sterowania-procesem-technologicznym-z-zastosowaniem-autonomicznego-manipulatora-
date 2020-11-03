using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class RevoluteJoint : MonoBehaviour {

    public string innerTag = "";
    public Vector3 PozycjaGlobalna;
    public Vector3 RotacjaGlobalna;
    private int BonesCount = 0;
    private int JointsCount = 0;

    //public bool Ustaw;
    public float UstawKąt = 0;
    public float BierzącyKąt;
    public bool SetBase = false;
    public bool end = false;

    private List<GameObject> Bones = new List<GameObject>();
    private List<GameObject> Joints = new List<GameObject>();

    // Use this for initialization
    private Matrix4x4 PrevHMatrix;
    public float    PoprzedniKąt = 0;
    private string PoprzedniTag = "";

    void Start () {
        Bones = GetChildObject(transform, "Bone");
        BonesCount = Bones.Count;
        Joints = GetChildObject(transform, "Joint");
        JointsCount = Joints.Count;
    }
    private Vector3 translation;
    private Vector3 eulerAngles;
    private Vector3 scale = new Vector3(1, 1, 1);
    public float długośćKości;

    public Vector3 NextJointPosition;
    public Matrix4x4 HM;
    public Matrix4x4 rotationMatrix;


    void Update() {
        PozycjaGlobalna = transform.position;
        RotacjaGlobalna = transform.eulerAngles;

        Quaternion rotation1 = transform.rotation;

        eulerAngles = transform.localEulerAngles;

        //if (UstawKąt > 360)
          //  UstawKąt = 0;

        if (innerTag == "VerticalRevoluteJoint")
        {
            // transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(transform.localEulerAngles.x, UstawKąt, transform.localEulerAngles.z), 1f);
            transform.localEulerAngles = new Vector3(0, UstawKąt, 0);
            BierzącyKąt = transform.localEulerAngles.y;

        }
        else if (innerTag == "HorizontalRevoluteJoint")
        {
            //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(UstawKąt, transform.localEulerAngles.y, transform.localEulerAngles.z), 1f);
            transform.localEulerAngles = new Vector3(UstawKąt, 0, 0);
            BierzącyKąt = transform.localEulerAngles.x;

            if ((Mathf.Abs(UstawKąt) >= 360))
            {
                UstawKąt = 0;
            }
            if ((UstawKąt < -90))
            {
               // UstawKąt = -90;
            }
        }

        // Obliczenia kolejnej kości

        if (BonesCount > 0)
        {
            Quaternion rot = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
            translation = new Vector3(0, Bones[0].transform.localScale.y, 0);
            długośćKości = Bones[0].transform.localScale.y;

            HM = Matrix4x4.TRS(translation, rot, scale);

            if (innerTag == "VerticalRevoluteJoint")
            {
                rotationMatrix = RotateY(BierzącyKąt * Mathf.Deg2Rad);
            }
            if (innerTag == "HorizontalRevoluteJoint")
            { 
            rotationMatrix = RotateX(BierzącyKąt * Mathf.Deg2Rad);

                Vector3 transl = HM.GetColumn(3);
                transl.x = 0;
                transl.y = długośćKości * Mathf.Cos(BierzącyKąt * Mathf.Deg2Rad);
                transl.z = długośćKości * Mathf.Sin(BierzącyKąt * Mathf.Deg2Rad);
                HM.SetColumn(3, new Vector4(transl.x, transl.y, transl.z, 1));
            }

            //Debug.Log(HM);


            NextJointPosition = (HM.MultiplyPoint(transform.position));
            //Debug.Log(HM.MultiplyPoint(transform.position));
         
        }

        Bones = GetChildObject(transform, "Bone");
        BonesCount = Bones.Count;
        Joints = GetChildObject(transform, "Joint");
        JointsCount = Joints.Count;

        SetChildHW(transform);
    }


    public void GoToBase()
    {
        /*
        end = false;
        if (UstawKąt < 0)
            UstawKąt += 0.8f;

        if (UstawKąt > 0)
            UstawKąt -= 0.8f;

        if (Mathf.Abs(UstawKąt) < 0.2f)
        {
            SetBase = false;
            end = true;
        }*/
    }

    public void SetChildHW(Transform parent)
    {
        List<GameObject> childs = GetChildObjectJoint(parent, "HorizontalRevoluteJoint");
        if (childs.Count > 0)
        {
            GameObject child = childs[0];
            Quaternion rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
            child.GetComponent<RevoluteJoint>().PrevHMatrix = Matrix4x4.TRS(translation, rotation, scale);
            child.GetComponent<RevoluteJoint>().PoprzedniKąt = BierzącyKąt;
            child.GetComponent<RevoluteJoint>().PoprzedniTag = innerTag;
        }
    }


    public static Matrix4x4 RotateX(float aAngleRad)
    {
        Matrix4x4 m = Matrix4x4.identity;     //  1   0   0   0 
        m.m11 = m.m22 = Mathf.Cos(aAngleRad); //  0  cos -sin 0
        m.m21 = Mathf.Sin(aAngleRad);         //  0  sin  cos 0
        m.m12 = -m.m21;                       //  0   0   0   1
        return m;
    }
    public static Matrix4x4 RotateY(float aAngleRad)
    {
        Matrix4x4 m = Matrix4x4.identity;     // cos  0  sin  0
        m.m00 = m.m22 = Mathf.Cos(aAngleRad); //  0   1   0   0
        m.m02 = Mathf.Sin(aAngleRad);         //-sin  0  cos  0
        m.m20 = -m.m02;                       //  0   0   0   1
        return m;
    }
    public static Matrix4x4 RotateZ(float aAngleRad)
    {
        Matrix4x4 m = Matrix4x4.identity;     // cos -sin 0   0
        m.m00 = m.m11 = Mathf.Cos(aAngleRad); // sin  cos 0   0
        m.m10 = Mathf.Sin(aAngleRad);         //  0   0   0   0
        m.m01 = -m.m10;                       //  0   0   0   1
        return m;
    }


    public List<GameObject> GetChildObject(Transform parent, string _tag)
    {
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
                if (child.tag == _tag)
                {
                    objs.Add(child.gameObject);
                }
                if (child.childCount > 0)
                {
                    GetChildObject(child, _tag);
                }
        }
        return objs;
    }

    public List<GameObject> GetChildObjectJoint(Transform parent, string _tag)
    {
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.GetComponent<RevoluteJoint>() != null)
            { 
                if (child.GetComponent<RevoluteJoint>().innerTag == _tag)
                {
                    objs.Add(child.gameObject);
                }
                if (child.childCount > 0)
                {
                    GetChildObject(child, _tag);
                }
            }

            if (child.GetComponent<PrismaticJoint>() != null)
            {
                if (child.GetComponent<PrismaticJoint>().innerTag == _tag)
                {
                    objs.Add(child.gameObject);
                }
                if (child.childCount > 0)
                {
                    GetChildObject(child, _tag);
                }
            }
        }
        return objs;
    }

}
