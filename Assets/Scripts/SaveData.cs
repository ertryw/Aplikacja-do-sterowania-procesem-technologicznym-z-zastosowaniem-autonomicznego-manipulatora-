using System;
using UnityEngine;
using System.IO;

[Serializable]
public class SaveData {
    public int       ElementsCount;
    public string[]  ElementName;
    public string[]  ElementInnerTag;
    public float[]   ElementLength;
    public float[]   ElementX;
    public float[]   ElementY;
    public float[]   ElementZ;
    public float[]   ElementRotX;
    public float[]   ElementRotY;
    public float[]   ElementRotZ;
    public float[]   Param1;

}

[Serializable]
public class SaveProgramData
{
    public int SeqCount;
    public Actions[] SeqName;
    public Dirs[] SeqDir;
    public float[] X;
    public float[] Y;
    public float[] Z;
    public float[] time;
    public bool[] jump;
    public int[] jumpID;
    public Object[] objecttag;
}


[Serializable]
public class SaveNetData
{
    public string BrokerAdres;
    public string BrokerPort;
    public string ObjectPos;
    public string JointsRotation;
}

public class ClassData
{

    public static void SaveProgramData(SaveProgramData data, string path)
    {
        string jsonString = JsonUtility.ToJson(data);

        using (StreamWriter streamWriter = File.CreateText(path))
        {
            streamWriter.Write(jsonString);
        }
    }


    public static SaveProgramData LoadProgramData(string path)
    {
        using (StreamReader streamReader = File.OpenText(path))
        {
            string jsonString = streamReader.ReadToEnd();
            return JsonUtility.FromJson<SaveProgramData>(jsonString);
        }
    }


}