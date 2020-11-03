using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 0 ,1 
public enum Actions  {Rusz,Bazowanie,Dezaktywuj,Aktywuj,EfektorNaCel, EfektorNaBaze, RuszNaPunkt,UstawNadObiekt,NaObiekt, Skok};
//public enum ActionsTxt { M , Still, Open, Close, EffectorTarget, EffectorBase, MoveUP, Down, MoveOnObj, DownOnObj, Jump };


public class ActionsClass : RobotClass {


    // Use this for initialization
    void Start () {
		
	}

    public static void CloseEffector()
    {
        EffectorClass.Close = true;
        EffectorClass.Open = false;
    }

    public static void OpenEffector()
    {
        EffectorClass.Close = false;
        EffectorClass.Open = true;
    }

    public static void SetEffectorOnTarget()
    {
        EffectorClass.SetEfectorOnTarget = true;
        EffectorClass.SetEfectorBase = false;
    }

    public static void EffectorOnBase()
    {
        EffectorClass.SetEfectorOnTarget = false;
        EffectorClass.SetEfectorBase = true;
    }




    // Update is called once per frame
    void Update () {
		
	}
}
