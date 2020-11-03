using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;

public enum Dirs {Center, Up, Down, Left, Right, LeftZ, RightZ};


public class MainSymProgram : MonoBehaviour {

    public class Seq : MonoBehaviour
    {
        public Actions action;
        public Dirs  dir;
        public float X;
        public float Y;
        public float Z;
        public float time;
        public bool  jump;
        public int   jumpID;
        public Object objectTag;
    }


    private RobotClass robot;
    public Seq[] Sequences;
    public Vector2[] SequencePos;
    private ForwardKinematic fk;
    private InverseKinematic ik;
    public static EndEffector EffectorClass;

    private InputField timefiled;
    private InputField speedfiled;
    private InputField loadfiled;
    private InputField savefiled;
    private InputField jumpfiled;
    private Dropdown dropobject;
    private Text seqclickk;

    public GameObject SeqPanel;
    public GameObject Point;
    public GameObject Point2;
    public GameObject CurrentSeqObj;

    public Dirs CurrentDir = Dirs.Up;



    public int lastseqindex = 0;
    public int lastseqindextxt = 0;
    public int lastifindex = 0;
    public int CurrentSeq;
    public int ClickedSeq = -1;
    public int seqLimit = 12;
    string programPath;
    string programloadPath;

    public bool RunBool = false;
    public bool LoadBool = false;
    public bool ChangeSeq = false;

    public float distance = 12.5f;

    // Use this for initialization
    void Start () {
        robot = GetComponent<RobotClass>();
        fk = GameObject.Find("Kinematyka").GetComponent<ForwardKinematic>();
        ik = GameObject.Find("Kinematyka Odwrotna").GetComponent<InverseKinematic>();
        Debug.Log("test " + ik.speed);
        timefiled  = GameObject.Find("TimeField").GetComponent<InputField>();
        speedfiled = GameObject.Find("SpeedField").GetComponent<InputField>();
        loadfiled = GameObject.Find("LoadField").GetComponent<InputField>();
        savefiled = GameObject.Find("SaveField").GetComponent<InputField>();
        jumpfiled = GameObject.Find("JumpField").GetComponent<InputField>();
        dropobject = GameObject.Find("DropItems").GetComponent<Dropdown>();
        seqclickk = GameObject.Find("SeqClick").GetComponent<Text>();

        Sequences = new Seq[seqLimit];
        SequencePos = new Vector2[seqLimit];
    }

    public void CenterDir(){
        CurrentDir = Dirs.Center;
    }
    public void UPDir()
    {
        CurrentDir = Dirs.Up;
    }
    public void DownDir()
    {
        CurrentDir = Dirs.Down;
    }
    public void LeftDir()
    {
        CurrentDir = Dirs.Left;
    }
    public void RightDir()
    {
        CurrentDir = Dirs.Right;
    }
    public void LeftZDir()
    {
        CurrentDir = Dirs.LeftZ;
    }
    public void RightZDir()
    {
        CurrentDir = Dirs.RightZ;
    }

    // Update is called once per frame
    void Update()
    {
        programPath = Path.Combine(Application.persistentDataPath, savefiled.text + ".txt");
        programloadPath = Path.Combine(Application.persistentDataPath, loadfiled.text + ".txt");

        if (robot.limbs[0] != null && LoadBool == false)
        {
            LoadBool = true;
            Invoke("LoadForwardKineamtic", 3);
        }

        if (ClickedSeq > -1)
        {
            seqclickk.text = ClickedSeq.ToString();
            
        }

    }

    public void LoadForwardKineamtic()
    {
        Debug.LogError("Wczytanie kineamtyki prostej i odwrotnej");
        fk.Base = robot.limbs[robot.FindFirstJointIndex()];
        fk.Końcówka = robot.FindEfectorObj();//robot.limbs[robot.lastlimbindex - 1];

        ik.Base = robot.limbs[robot.FindFirstJointIndex()];
        ik.Końcówka = robot.FindEfectorObj();
    }

    public void MovePoint(float X, float Y, float Z)
    {
        GameObject.Find("Punkt").transform.position = new Vector3(X, Y, Z);
    }

    public Vector3 MovePointOnDir(float X, float Y, float Z, Dirs dir)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        if (dir == Dirs.Center)
        { 
            GameObject.Find("Punkt").transform.position = new Vector3(X, Y, Z);
            pos = new Vector3(X, Y, Z);
        }
        if (dir == Dirs.Up)
        {
            GameObject.Find("Punkt").transform.position = new Vector3(X, Y + distance, Z);
            pos = new Vector3(X, Y + distance, Z);
        }
        if (dir == Dirs.Down)
        {
            GameObject.Find("Punkt").transform.position = new Vector3(X, Y - distance, Z);
            pos = new Vector3(X, Y - distance, Z);
        }
        if (dir == Dirs.Left)
        {
            GameObject.Find("Punkt").transform.position = new Vector3(X - distance, Y, Z);
            pos = new Vector3(X - distance, Y, Z);
        }
        if (dir  == Dirs.Right)
        {
            GameObject.Find("Punkt").transform.position = new Vector3(X + distance, Y, Z);
            pos = new Vector3(X + distance, Y, Z);
        }
        if (dir == Dirs.LeftZ)
        {
            GameObject.Find("Punkt").transform.position = new Vector3(X, Y, Z - distance);
            pos = new Vector3(X, Y, Z - distance);
        }
        if (dir == Dirs.RightZ)
        {
            GameObject.Find("Punkt").transform.position = new Vector3(X, Y, Z -+ distance);
            pos = new Vector3(X, Y, Z - +distance);
        }
        return pos;
    }

    public void DownPoint(float X, float Y, float Z)
    {
        GameObject.Find("Punkt").transform.position = Vector3.MoveTowards(GameObject.Find("Punkt").transform.position, new Vector3(X, Y, Z), Time.deltaTime * 44f);
    }

    public void RunProgram()
    {
        RunBool = true;
        Debug.LogError("");
        StartCoroutine(SeqLoop());
    }
    public void StopProgram()
    {
        RunBool = false;
    }

    public void ChangeJumpID()
    {
        if (ClickedSeq > -1)
        {
            if (jumpfiled.text != "")
                Sequences[ClickedSeq].jumpID = int.Parse(jumpfiled.text);
            else
                Debug.LogError("Nie wpisano Id skoku");
        }
     }

    public void ChangeObjectTag()
    {
        if (ClickedSeq > -1)
        {
            Sequences[ClickedSeq].objectTag = returnObject();
        }
    }

    public Vector3 ReturnDirPos()
    {
        if (CurrentDir == Dirs.Center)
        {
            return GameObject.Find("Punkt").transform.position;
        }
        if (CurrentDir == Dirs.Up)
        {
            return GameObject.Find("PUp").transform.position;
        }
        if (CurrentDir == Dirs.Down)
        {
            return GameObject.Find("PDown").transform.position;
        }
        if (CurrentDir == Dirs.Left)
        {
            return GameObject.Find("PLeft").transform.position;
        }
        if (CurrentDir == Dirs.Right)
        {
            return GameObject.Find("PRight").transform.position;
        }
        if (CurrentDir == Dirs.LeftZ)
        {
            return GameObject.Find("PLeftZ").transform.position;
        }
        if (CurrentDir == Dirs.RightZ)
        {
            return GameObject.Find("PRightZ").transform.position;
        }

        Debug.LogError("Bład zwaracania Dira");
        return new Vector3(0, 0, 0);
    }


    public GameObject FindObjectToPick()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].GetComponent<ObjectC>().CanPick && !objects[i].GetComponent<ObjectC>().placed)
            {
                return objects[i];
            }
        }

        return null;
    }

    public GameObject FindPicketobject()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].GetComponent<ObjectC>().picked)
            {
                return objects[i];
            }
        }

        return null;
    }

    public void CheckJump()
    {
        Seq Sequence = Sequences[CurrentSeq];
        if (Sequence.jump)
        {
            CurrentSeq = Sequence.jumpID;
        } else
        {
            CurrentSeq++;
        }

    }


    public IEnumerator SeqLoop()
    {
        while (RunBool)
        {
            Seq Sequence = Sequences[CurrentSeq];
            ik.speed = float.Parse(speedfiled.text);

             RectTransform currrec = CurrentSeqObj.GetComponent<RectTransform>();
             currrec.anchoredPosition = SequencePos[CurrentSeq];

            if (Sequence.action == Actions.Skok)
            {
                GameObject obj = FindPicketobject();
                yield return new WaitForSeconds(Sequence.time);

                if (obj == null && Sequence.jumpID != 0)
                {
                    CurrentSeq = 0;
                }
                else
                {
                    if (Sequence.jumpID != 0)
                    {
                        ObjectC objc = obj.GetComponent<ObjectC>();

                        if (Sequence.objectTag == objc.objectTag)
                        {
                            Debug.Log("jump do" + Sequence.jumpID.ToString());
                            CurrentSeq = Sequence.jumpID;
                        }
                        else
                        {
                            CurrentSeq++;
                        }
                    }
                    else if (Sequence.jumpID == 0)
                    {
                        CurrentSeq = Sequence.jumpID;
                    }


                }
            }


            if (Sequence.action == Actions.Rusz)
            { 
                ik.set = true;
                MovePoint(Sequence.X, Sequence.Y, Sequence.Z);
                ik.TargetSeqPos = new Vector3(Sequence.X, Sequence.Y, Sequence.Z);
                yield return new WaitForSeconds(Sequence.time);
                if (ik.end == true)
                {
                    CheckJump();
                }
            }

            if (Sequence.action == Actions.RuszNaPunkt)
            {

                DownPoint(Sequence.X, Sequence.Y, Sequence.Z);
                ik.set = true;
                ik.down = true;
                ik.TargetSeqPos = GameObject.Find("Punkt").transform.position;//(new Vector3(Sequence.X, Sequence.Y, Sequence.Z));
                ik.TargetPos = new Vector3(Sequence.X, Sequence.Y, Sequence.Z);

                yield return new WaitForSeconds(0.1f);

                if (ik.end == true)
                {
                    ik.set = false;
                    ik.down = false;
                    yield return new WaitForSeconds(Sequence.time);
                    CheckJump();
                }
            }

            if (Sequence.action == Actions.UstawNadObiekt)
            {

                GameObject obj = FindObjectToPick();
                yield return new WaitForSeconds(0.1f);

                if (obj != null)
                {
                    Debug.LogError(" ");
                    Vector3 pos = MovePointOnDir(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z, Sequence.dir);
                    ik.set = true;
                    ik.TargetSeqPos = GameObject.Find("Punkt").transform.position;
                    robot.FindEfectorObj().GetComponent<EndEffector>().TargetPosition = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);

                    yield return new WaitForSeconds(Sequence.time);
                    if (ik.end == true)
                    {
                        ik.set = false;
                        ik.down = false;
                        yield return new WaitForSeconds(Sequence.time);
                        CheckJump();
                    }
                }
                else
                {
                    Debug.LogError("Nie zanaleziono obiektu!");
                }


            }

            if (Sequence.action == Actions.NaObiekt)
            {
                GameObject obj = FindObjectToPick();
                yield return new WaitForSeconds(0.1f);


                if (obj != null)
                {
                    ObjectC objc = obj.GetComponent<ObjectC>();
                    Debug.LogError(" ");
                    DownPoint(obj.transform.position.x, obj.transform.position.y + objc.heigth, obj.transform.position.z);
                    //Vector3 pos = MovePointOnDir(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z, Sequence.dir);
                    ik.set = true;
                    ik.down = true;
                    ik.TargetSeqPos = GameObject.Find("Punkt").transform.position;//(new Vector3(Sequence.X, Sequence.Y, Sequence.Z));
                    ik.TargetPos = new Vector3(obj.transform.position.x, obj.transform.position.y + objc.heigth, obj.transform.position.z);

                    /*
                    ik.set = true;
                    ik.TargetSeqPos = GameObject.Find("Punkt").transform.position;
                    robot.FindEfectorObj().GetComponent<EndEffector>().TargetPosition = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
                    */
                    yield return new WaitForSeconds(0.05f);
                    if (ik.end == true)
                    {

                        //objc.effectorpos = robot.EffectorClass2;
                        //objc.picked = true;



                        ik.set = false;
                        ik.down = false;
                        yield return new WaitForSeconds(Sequence.time);
                        CheckJump();
                    }
                }
                else
                {
                    Debug.LogError("Nie zanaleziono obiektu!");
                }

            }


            if (Sequence.action == Actions.Bazowanie)
            {
                ik.SetBase = true;
                yield return new WaitForSeconds(Sequence.time);

                if (Mathf.Abs(ik.katsum) < 5f)
                {
                    ik.SetBase = false;

                    CheckJump();
                }
            }

            if (Sequence.action == Actions.Dezaktywuj)
            {
                GameObject obj = FindPicketobject();
                if (obj != null)
                { 
                    ObjectC objc = obj.GetComponent<ObjectC>();
                    objc.picked = false;
                }

                ActionsClass.OpenEffector();
                yield return new WaitForSeconds(Sequence.time);
                CheckJump();
            }

            if (Sequence.action == Actions.Aktywuj)
            {
                GameObject obj = FindObjectToPick();

                if (obj != null)
                {
                    ObjectC objc = obj.GetComponent<ObjectC>();
                    objc.effectorpos = robot.EffectorClass2;
                    objc.picked = true;
                }

                ActionsClass.CloseEffector();
                yield return new WaitForSeconds(Sequence.time);
                CheckJump();
            }

            if (Sequence.action == Actions.EfektorNaCel)
            {
                ActionsClass.SetEffectorOnTarget();
                yield return new WaitForSeconds(Sequence.time);
                CheckJump();
            }

            if (Sequence.action == Actions.EfektorNaBaze)
            {
                ActionsClass.EffectorOnBase();
                yield return new WaitForSeconds(Sequence.time);
                CheckJump();
            }
          

            if (CurrentSeq == lastseqindex)
            {
                CurrentSeq = 0;
            }
        }
    }



    public void ClearProgram()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("text");
        for (int i = 0; i < objects.Length; i++)
        {
            Destroy(objects[i]);
        }

        Sequences = new Seq[seqLimit];
        CurrentSeq = 0;
        lastseqindex = 0;
        lastseqindextxt = 0;
        lastifindex = 0;
    }

    public void SaveProgram()
    {
        SaveProgramData dat = new SaveProgramData();
        dat.SeqCount = lastseqindex;
        dat.SeqName = new Actions[lastseqindex];
        dat.SeqDir = new Dirs[lastseqindex];
        dat.X = new float[lastseqindex];
        dat.Y = new float[lastseqindex];
        dat.Z = new float[lastseqindex];
        dat.time = new float[lastseqindex];
        dat.jump = new bool[lastseqindex];
        dat.jumpID = new int[lastseqindex];
        dat.objecttag = new Object[lastseqindex];
        for (int i = 0; i < lastseqindex; i++)
        {
            dat.SeqName[i] = Sequences[i].action;
            dat.SeqDir[i] = Sequences[i].dir;
            dat.time[i] = Sequences[i].time;
            dat.X[i] = Sequences[i].X;
            dat.Y[i] = Sequences[i].Y;
            dat.Z[i] = Sequences[i].Z;

            dat.jump[i] = Sequences[i].jump;
            dat.jumpID[i] = Sequences[i].jumpID;
            dat.objecttag[i] = Sequences[i].objectTag;
        }
        //ClassSave.SaveProgramData(dat, programPath);
        //SaveDat(dat, programPath);
    }


    GameObject CreateText(Transform canvas_transform, float x, float y, string text_to_print, int font_size, Color text_color)
    {
        GameObject UItextGO = new GameObject("Text2");
        UItextGO.transform.SetParent(canvas_transform);
        UItextGO.transform.localScale = new Vector3(1, 1, 1);

        UItextGO.transform.tag = "text";
        RectTransform trans = UItextGO.AddComponent<RectTransform>();
        trans.sizeDelta = new Vector2(120, 15);
        trans.anchoredPosition = new Vector2(x, y);
        Text text = UItextGO.AddComponent<Text>();
        text.text = text_to_print;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = font_size;
        text.color = text_color;
        SequencePos[lastseqindex] = new Vector2(x,y);
        return UItextGO;
    }


    public Object returnObject()
    {

        if (dropobject.value == 0)
        {
            return Object.object1;
        }
        else if (dropobject.value == 1)
        {
            return Object.object2;
        }
        else if (dropobject.value == 2)
        {
            return Object.object3;
        }
        return Object.nullobj;
    }



    public Seq CreateSequence(Actions action, Vector3 pos)
    {
        Seq Seq = new Seq();
        Seq.action = action;
        Seq.X = pos.x;
        Seq.Y = pos.y;
        Seq.Z = pos.z;
        Seq.time = float.Parse(timefiled.text);
        Seq.dir = CurrentDir;
        if (Seq.action == Actions.Skok)
        {
            Seq.jumpID = int.Parse(jumpfiled.text);
            Seq.jump = true;
            Seq.objectTag = returnObject();
            lastifindex++;
        }
        else
        {
            Seq.jumpID = -1;
            Seq.jump = false;
            Seq.objectTag = Object.nullobj;
            lastseqindextxt++;
        }

        lastseqindex++;
        //lastseqindextxt++;
        return Seq;
    }

    public int posx = 1521211;



    public void ClearOneSeq(string s)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("text");
        for (int i = 0; i < objects.Length; i++)
        {
            string txt = objects[i].GetComponent<Text>().text;
            if (txt.Contains(s))
                Destroy(objects[i]);
        }
    }



    public void AddText(int i)
    {
        Actions action = (Actions)i;
        string actiontxt = action.ToString();
        Debug.Log(actiontxt);
    
        if (action == Actions.Skok)
        {
            CreateText(SeqPanel.transform, 90, posx - (lastifindex * 16), "(" + lastseqindex.ToString() + ") " + actiontxt, 12, Color.black);
            Sequences[lastseqindex] = CreateSequence(action, ReturnDirPos());
        }
        else
        {
            if (!ChangeSeq)
            {
                CreateText(SeqPanel.transform, -25, posx - (lastseqindextxt * 16), "(" + lastseqindex.ToString() + ") " + actiontxt, 12, Color.black);
                Sequences[lastseqindex] = CreateSequence(action, ReturnDirPos());
            }
                else
            {
                ClearOneSeq(ClickedSeq.ToString());
                CreateText(SeqPanel.transform, SequencePos[ClickedSeq].x, SequencePos[ClickedSeq].y, "(" + ClickedSeq.ToString() + ") " + actiontxt, 12, Color.black);
                Sequences[ClickedSeq] = CreateSequence(action, ReturnDirPos());
            }
        }

    }
    

    public void LoadSequence(Seq Seq)
    {
        Sequences[lastseqindex] = Seq;
        string actiontxt = Seq.action.ToString();

        if (Seq.action == Actions.Skok)
            CreateText(SeqPanel.transform, 90, posx - (lastifindex * 16), "(" + lastseqindex.ToString() + ") " + actiontxt, 12, Color.black);
        else
            CreateText(SeqPanel.transform, -25, posx - (lastseqindextxt * 16), "(" + lastseqindex.ToString() + ")" + actiontxt, 12, Color.black);
    }


    public void LoadProgram()
    {
        SaveProgramData datp = new SaveProgramData();
        datp = ClassData.LoadProgramData(programPath); //LoadDatP(programPath);
        for (int i = 0; i < datp.SeqCount; i++)
        {
            Seq s = new Seq();
            s.action = datp.SeqName[i];
            s.dir = datp.SeqDir[i];
            s.X = datp.X[i];
            s.Y = datp.Y[i];
            s.Z = datp.Z[i];
            s.time = datp.time[i];
            s.jump = datp.jump[i];
            s.jumpID = datp.jumpID[i];
            s.objectTag = datp.objecttag[i];

            LoadSequence(s);

            if (s.action == Actions.Skok)
            {
                lastifindex++;
            }
            else
            {
                lastseqindextxt++;
            }

            lastseqindex++;

        }

    }

}
