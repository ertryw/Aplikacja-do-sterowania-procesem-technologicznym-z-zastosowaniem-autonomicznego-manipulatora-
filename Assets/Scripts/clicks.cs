using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class clicks : MonoBehaviour {

    // Normal raycasts do not work on UI elements, they require a special kind
    GraphicRaycaster raycaster;
    GameObject main;
    MainSymProgram sym;
    private InputField jumpfiled;
    private Dropdown dropobject;

    public int click;
    public string txttemp;

    void Start()
    {
        main = GameObject.Find("Uklad");
        sym = main.GetComponent<MainSymProgram>();
        jumpfiled = GameObject.Find("JumpField").GetComponent<InputField>();
        dropobject = GameObject.Find("DropItems").GetComponent<Dropdown>();

    }

     void Awake()
    {
        // Get both of the components we need to do this
        this.raycaster = GetComponent<GraphicRaycaster>();
    }


    public int returnObjectValue()
    {
        //sym.Sequences[sym.ClickedSeq].objectTag
        if (sym.Sequences[sym.ClickedSeq].objectTag == Object.object1)
        {
            return 0;
        }
        else if (sym.Sequences[sym.ClickedSeq].objectTag == Object.object2)
        {
            return 1;
        }
        else if (sym.Sequences[sym.ClickedSeq].objectTag == Object.object3)
        {
            return 2;
        }
        return -1;
    }


    void Update()
    {
        //Check if the left Mouse button is clicked
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);
            //sym.ChangeSeq = false;
            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {

                if (result.gameObject.GetComponent<Text>() != null)
                {
                  
                    if (click == 2)
                    {
                        string txt1 = result.gameObject.GetComponent<Text>().text;
                        if (txt1[0] == '(')
                        {
                            if (txt1 == txttemp)
                            {
                                Debug.Log("Double Click");
                                sym.ChangeSeq = true;
                            }
                            else
                            {
                                sym.ChangeSeq = false;
                            }
                        }

                        click = 0;
                    }
    
                    click++;
                    string txt = result.gameObject.GetComponent<Text>().text;
                    txttemp = txt;
                    if (txt[0] == '(')
                    {
                        string valstr = "";
                        int val = -1;
                        for (int i = 0; i < txt.Length; i++)
                        {
                            if (char.IsDigit(txt[i]))
                                valstr += txt[i];
                        }
                        if (valstr.Length > 0)
                            val = int.Parse(valstr);

                        Debug.Log(val);
                        sym.ClickedSeq = val;
                        jumpfiled.text = sym.Sequences[sym.ClickedSeq].jumpID.ToString();
                        dropobject.value = returnObjectValue();
                    }
                }

  
            }
        }
    }
}
