using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematic : MonoBehaviour {

    public GameObject Base;
    public List<GameObject> Joints = new List<GameObject>();
    public GameObject Końcówka;
    public Vector3 KońcówkaPos;
    public GameObject ForwardKineamticObj;
    public ForwardKinematic FK;

    // J = [Jv Jw]
    // Revolute Joint
    // Jw = R0i * [0 0 1] u nas Vertical [0 1 0] Horizontal [1 0 0]  
    // Use this for initialization

    public Matrix Jw,Jv,J,JT,JI;

    public float Kąt1, Kąt2, Kąt3, Kąt4;

    public Matrix dO;


    public GameObject TargetObj;
    public Vector3 TargetPos;
    public Vector3 TargetSeqPos;
    public Vector3 LastTargetSeqPos;
    public Vector3 V; // V = T -E;
    public Matrix V2; // V = T -E;

    public bool set = false;
    public bool reset = false;
    public bool SetBase = false;
    public bool end = false;
    public bool down= false;

    public float speed = 0.05f;
    public Vector3 move; // V = T -E;

    void Start () {
        FK = ForwardKineamticObj.GetComponent<ForwardKinematic>();
    }

    public bool j1_reverse = false;
    public float katsum = 0;
    public int y = 1;
    public bool JaI = false;
    public bool JaT = true;



    public float[] result;

    void FixedUpdate() {
        Joints.Clear();

        if (Base != null)
        {
            Joints = GetJointsWihtTag(Base, "JointObject");
            KońcówkaPos = Końcówka.transform.position;

            if (JaI == true)
                JaT = false;
            else
                JaT = true;

            Jw = CreateJacobianW();
            V = TargetSeqPos - KońcówkaPos;
            Jv = CreateJacobianV();

            if (Jw == null)
                return;
            if (Jv == null)
                return;


            J = CreateJacobian();
            JT = J.T;

            V2 = new Matrix(6, 1);
            V2.mat[0][0] = V.x;
            V2.mat[1][0] = V.y;
            V2.mat[2][0] = V.z;


            if (JaI == true)
            {
                result = new float[JI.rows];

                for (int i = 0; i < JI.rows; ++i) // each row of A
                    for (int j = 0; j < JI.cols; ++j) // each col of B
                    {
                        result[i] += JI.mat[i][j] * V2.mat[j][0];
                    }
            }

            if (JaT == true)
            {
                result = new float[JT.rows];
                for (int i = 0; i < JT.rows; ++i) // each row of A
                    for (int j = 0; j < JT.cols; ++j) // each col of B
                    {
                        result[i] += JT.mat[i][j] * V2.mat[j][0];
                    }
            }

            for (int i = 0; i < Joints.Count; i++)
            {
                if (SetBase)
                {
                  
           
                    end = false;
                    katsum = 0;
                    for (int y = 0; y < Joints.Count; y++)
                    {
                        Kąt1 = Joints[y].GetComponent<RevoluteJoint>().UstawKąt;

                        if (Mathf.Abs(Kąt1) > 1)
                        {
                            if (Kąt1 <= 0)
                                Joints[y].GetComponent<RevoluteJoint>().UstawKąt += 1.2f;

                            if (Kąt1 > 0)
                                Joints[y].GetComponent<RevoluteJoint>().UstawKąt -= 1.2f;
                        }
                        katsum += Mathf.Abs(Kąt1);
                    }

                    if (Mathf.Abs(katsum) < 5f)
                    {
                        SetBase = false;
                        end = true;
                        break;
                    }
                }

                if (down == false)
                {
                    if (Vector3.Distance(TargetSeqPos, KońcówkaPos) > 0.3)
                    {
                        end = false;

                        if (set)
                        {
                            katsum = 0;

                            Debug.Log(Joints.Count);
                            for (int y = 0; y < Joints.Count; y++)
                            {

                                Kąt1 = Mathf.Abs(Joints[y].GetComponent<RevoluteJoint>().UstawKąt);
                                katsum += Kąt1;

                                if (katsum > 1000)
                                {
                                    Joints[y].GetComponent<RevoluteJoint>().UstawKąt = 0;
                                }


                                if (result[y] > 1.5)
                                    result[y] = 1.5f;
                                if (result[y] < -1.5f)
                                    result[y] = -1.5f;


                                if (JaT)
                                {
                                    if (Vector3.Distance(TargetSeqPos, KońcówkaPos) > 1)
                                        Joints[y].GetComponent<RevoluteJoint>().UstawKąt += result[y] * speed;

                                    if (Vector3.Distance(TargetSeqPos, KońcówkaPos) <= 1)
                                        Joints[y].GetComponent<RevoluteJoint>().UstawKąt += result[y] * speed / 2;

                                }

                                if (JaI)
                                {
                                    if (Vector3.Distance(TargetSeqPos, KońcówkaPos) < 0.4)
                                        Joints[y].GetComponent<RevoluteJoint>().UstawKąt += result[y] * 1f;

                                    if (Vector3.Distance(TargetSeqPos, KońcówkaPos) > 0.4)
                                        Joints[y].GetComponent<RevoluteJoint>().UstawKąt += result[y] * 1f;
                                }
                            }

                        }
                    }
                    else
                    {
                        y = 1;
                        set = false;
                        end = true;
                    }
                }else
                {
                    if (Vector3.Distance(TargetPos, KońcówkaPos) > 0.3)
                    {
                        end = false;

                        if (set)
                        {
                            katsum = 0;

                            Debug.Log(Joints.Count);
                            for (int y = 0; y < Joints.Count; y++)
                            {

                                Kąt1 = Mathf.Abs(Joints[y].GetComponent<RevoluteJoint>().UstawKąt);
                                katsum += Kąt1;

                                if (katsum > 1000)
                                {
                                    Joints[y].GetComponent<RevoluteJoint>().UstawKąt = 0;
                                }


                                if (result[y] > 1.5)
                                    result[y] = 1.5f;
                                if (result[y] < -1.5f)
                                    result[y] = -1.5f;


                                if (JaT)
                                {
                                    if (Vector3.Distance(TargetSeqPos, KońcówkaPos) > 1)
                                        Joints[y].GetComponent<RevoluteJoint>().UstawKąt += result[y] * speed;

                                    if (Vector3.Distance(TargetSeqPos, KońcówkaPos) <= 1)
                                        Joints[y].GetComponent<RevoluteJoint>().UstawKąt += result[y] * speed/2;


                                }

                                if (JaI)
                                {
                                    if (Vector3.Distance(TargetSeqPos, KońcówkaPos) < 0.4)
                                        Joints[y].GetComponent<RevoluteJoint>().UstawKąt += result[y] * 1f;

                                    if (Vector3.Distance(TargetSeqPos, KońcówkaPos) > 0.4)
                                        Joints[y].GetComponent<RevoluteJoint>().UstawKąt += result[y] * 1f;
                                }
                            }

                        }
                    }
                    else
                    {
                        y = 1;
                        set = false;
                        end = true;
                    }
                }
            }
        }
        }


    Matrix CreateJacobianW()
    {

        // row --->
        //colum |

        Matrix M = new Matrix(3, Joints.Count);
        List<Vector3> rotYs = new List<Vector3>();
        
        for (int i = 0; i < Joints.Count; i++)
        {
            var r = Joints[i].GetComponent<RevoluteJoint>();
            if (r.innerTag == "HorizontalRevoluteJoint")
            {
                rotYs.Add(r.HM.GetColumn(0));
            }
            else
            {
                rotYs.Add(r.HM.GetColumn(1));
            }
        }

        for (int i = 0; i < rotYs.Count; i++)
        {
            M.mat[0][i] = rotYs[i].x;
            M.mat[1][i] = rotYs[i].y;
            M.mat[2][i] = rotYs[i].z;
        }
        return M;
    }


    Matrix CreateJacobian()
    {
        Matrix J = new Matrix(6, 6);

        for (int i = 0; i < Joints.Count; i++)
        {
            J.mat[0][i] = Jv.mat[0][i];
            J.mat[1][i] = Jv.mat[1][i];
            J.mat[2][i] = Jv.mat[2][i];
            J.mat[3][i] = Jw.mat[0][i];
            J.mat[4][i] = Jw.mat[1][i];
            J.mat[5][i] = Jw.mat[2][i];
        }

        for (int i = 0; i < 6; i++)
        {
            if (J.mat[i][i] == 0)
                J.mat[i][i] = 1;
        }

        return J;
    }

    Matrix CreateJacobianV()
    {
        Matrix Jv = new Matrix(3, Joints.Count);
        List<Vector3> rotYs = new List<Vector3>();
        Vector3 v1,v2;

        Debug.Log("Jointds: " + Joints.Count.ToString());
        Debug.Log("FK: " + FK.NextJointsPosition.Count.ToString());
        if (FK.NextJointsPosition.Count == 0)
            return null;

        for (int i = 0; i < Joints.Count; i++)
        {
            //if (i == 0)
            v1 = GetColumnFromMatrix(Jw, i);
          
            if (i == 0)
            {
                v2 = KońcówkaPos;
            }
            else
            {
                v2 = KońcówkaPos - FK.NextJointsPosition[i-1];
            }
            

            Vector3 v3 = Vector3.Cross(v1, v2);
            Jv.mat[0][i] = v3.x;
            Jv.mat[1][i] = v3.y;
            Jv.mat[2][i] = v3.z;
        }

        return Jv;
    }

    public Vector3 GetColumnFromMatrix(Matrix M,int c)
    {
        Vector3 v;
        float x, y, z;
        x = M.mat[0][c];
        y = M.mat[1][c];
        z = M.mat[2][c];
        v = new Vector3(x, y, z);
        return v;
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
