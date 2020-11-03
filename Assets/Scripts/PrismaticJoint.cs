using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class PrismaticJoint : MonoBehaviour {

    public GameObject Connect,Bone;
    public string innerTag ="";
    public Vector3 PozycjaGlobalna;
    public Vector3 RotacjaGlobalna;

    public float Length;
    public float MinLenght = 1;
    public float MaxLenght = 5;
    public Vector3 End;
    private Vector3 scale = new Vector3(1, 1, 1);
    public Matrix4x4 HM;
    public Vector3 NextJointPosition;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        PozycjaGlobalna = transform.position;
        RotacjaGlobalna = transform.eulerAngles;

        Bone = GetChildObject(transform,"Bone")[0];
        if (Bone != null )
        {

            if (innerTag == "PrismaticJointVertical")
            {
                if (Length <= MaxLenght)
                {
                    Bone.transform.localScale = new Vector3(Bone.transform.localScale.x, Length, Bone.transform.localScale.z);
                    Bone.transform.localPosition = new Vector3(Bone.transform.localPosition.x, Bone.transform.localScale.y / 2, Bone.transform.localPosition.z);
                }
                else
                {
                    Length = MaxLenght;
                }
                if (Length <= MinLenght)
                {
                    Length = MinLenght;
                }
                End = new Vector3(Bone.transform.localPosition.x, Bone.transform.localScale.y, Bone.transform.localPosition.z);
            }

            if (innerTag == "PrismaticJointHorizontal")
            {
                if (Length <= MaxLenght)
                {
                    Bone.transform.localScale = new Vector3(Bone.transform.localScale.x, Length, Bone.transform.localScale.z);
                    Bone.transform.localPosition = new Vector3(Bone.transform.localScale.y / 2, Bone.transform.localPosition.y, Bone.transform.localPosition.z);
                }
                else
                {
                    Length = MaxLenght;
                }
                if (Length <= MinLenght)
                {
                    Length = MinLenght;
                }
                End = new Vector3(Bone.transform.localScale.y, Bone.transform.localPosition.y, Bone.transform.localPosition.z);
            }


            Quaternion rot = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
            Vector3 translation = End;
            HM = Matrix4x4.TRS(translation, rot, scale);
 
            Vector3 transl = HM.GetColumn(3);
            transl.x = 0;
            transl.y = Length * Mathf.Cos(transform.localEulerAngles.x * Mathf.Deg2Rad);
            transl.z = Length * Mathf.Sin(transform.localEulerAngles.x * Mathf.Deg2Rad);
            HM.SetColumn(3, new Vector4(transl.x, transl.y, transl.z, 1));

          //  Debug.Log(HM);

            NextJointPosition = (HM.MultiplyPoint(transform.position));




        if (Connect != null)
        {
                transform.SetPositionAndRotation(Connect.GetComponent<PrismaticJoint>().NextJointPosition,transform.rotation);
        }




        }


    }


    public void SetLength()
    {

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
}
