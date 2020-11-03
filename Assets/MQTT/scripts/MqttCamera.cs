using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System;

public class MqttCamera : MonoBehaviour
{

    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    private Vector3 position;
    private Vector3 position2;
    private Vector3 position3;
    private float width;
    private float height;

    public Texture2D TheTexture;
    private MqttClient client;

    public Scrollbar barY;
    public Scrollbar barX;

    const string EXTERNALDEVICE_IMAGEDATA = "Ext/Image/Data";

    Vector2Int rec;
    void Start()
    {
        defaultBackground = background.texture;


        // Position used for the cube.
        TheTexture = new Texture2D(200, 200);
        position = new Vector3(0.0f, 0.0f, 0.0f);
        WebCamDevice[] devices = WebCamTexture.devices;

        TheTexture = new Texture2D(50, 50, TextureFormat.ARGB32, false);


        Screen.SetResolution(640, 1024, true);

        rec.x = 60;
        rec.y = 60;

        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;


        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                Debug.Log(backCam.isPlaying);
            }

            #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                Debug.Log(backCam.isPlaying);
            #endif
        }

        if (backCam == null)
        {
            Debug.Log("Nie ma kamery przedniej");
            return;
        }

        backCam.Play();
        background.texture = backCam;
        camAvailable = true;
    }



    void OnGUI()
    {
        // Compute a fontSize based on the size of the screen width.
        GUI.skin.label.fontSize = (int)(Screen.width / 20.0f);

        GUI.Label(new Rect(20, 20, width, height * 0.25f), "Adres Brokera: "+mqttTest.IPADRES );

        //GUI.Label(new Rect(20, 20, width, height * 0.25f), "x = " + position.x.ToString("f2") + ", y = " + position.y.ToString("f2"));
        //GUI.Label(new Rect(20, 40, width, height * 0.25f), "x = " + position2.x.ToString("f2") + ", y = " + position2.y.ToString("f2"));
        //GUI.Label(new Rect(20, 60, width, height * 0.25f), "recx = " + rec.x.ToString("f2") + ",recy = " + rec.y.ToString("f2"));
        if (rec.x > 20 && rec.y > 20)
        {
            GUI.DrawTexture(new Rect(10, 740, rec.x, rec.y), TheTexture, ScaleMode.StretchToFill, true, 1.0F);
            //GUI.DrawTexture(new Rect(50, 700, 161, 161), TheTexture, ScaleMode.StretchToFill, true, 1.0F);
        }

    }
    public void LoadDetector()
    {
        if (backCam != null)
            backCam.Stop();
        StartCoroutine(LoadYourAsyncScene());
    }
    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("1");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    int s = 1;


    public void PublishImageData()
    {
        client = new MqttClient(IPAddress.Parse(mqttTest.IPADRES), 1883, false, null);
        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId, "unityimage", "", true, 5);
        byte[] rawbytes = TheTexture.EncodeToPNG();
        s++;
        Debug.Log("Wysyłanie długość" + rawbytes.Length.ToString());
        if (client != null && client.IsConnected )
        {
            client.Publish(EXTERNALDEVICE_IMAGEDATA, rawbytes, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);
            Debug.Log("Polaczony");
        }
        else
        {
            Debug.Log("Nie udało się wysłać");
        }
    }


    public void RectChange()
    {
        rec.x = 60 + (int)(barX.value * 600);
        rec.y = 60 + (int)(barY.value * 300);

        TheTexture = new Texture2D(rec.x, rec.y, TextureFormat.ARGB32, false);
        TheTexture.SetPixels(backCam.GetPixels((int)position2.y, (int)position2.x, rec.x, rec.y));
        TheTexture = rotateTexture(TheTexture, true);
        TheTexture.Apply();

    }
    int ImageX_Pos;
    int ImageY_Pos;


    void Update()
    {
        if (!camAvailable)
            return;

        float ratio = (float)backCam.width / (float)backCam.height;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        //TheTexture = new Texture2D(backCam.width, backCam.height, TextureFormat.ARGB32, false);

        //byte[] bytes = texture.EncodeToPNG();
        //TheTexture.SetPixels(texture.GetPixels());

        int s_y = 380; // windows


        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Cam Width: " + backCam.width.ToString() + "Heigth: " + backCam.height.ToString());
            //Debug.Log("BackGround Width: " + background.rectTransform.sizeDelta.x + "Heigth: " + background.rectTransform.offsetMin);

            float bottom = background.rectTransform.offsetMin.y;
            float top = background.rectTransform.offsetMax.y;
            float right = background.rectTransform.localPosition.x;


            // Touch touch = Input.GetTouch(0);
            position = Input.mousePosition;
            //position = touch.position;

            if (position.y > 340 && position.x < 550)
            {
                ImageX_Pos = (int)(right + position.x) + (0);
                ImageY_Pos = (int)(position.y) + 75;

                position2 = new Vector2(ImageX_Pos, 1024 - ImageY_Pos);

                TheTexture = new Texture2D(rec.x, rec.y, TextureFormat.ARGB32, false);
                TheTexture.SetPixels(backCam.GetPixels((int)position2.y, (int)position2.x, rec.x, rec.y));
                TheTexture = rotateTexture(TheTexture, true);
                TheTexture.Apply();
            }
      


            //Debug.Log("BackGround Pos X:" + ImageX_Pos + " Y:" + ImageY_Pos);

            /*
            if (position2.x > 0 && position2.y > 0)
            {
                TheTexture.SetPixels(backCam.GetPixels(ImageX_Pos, ImageY_Pos, 50, 50));
                // TheTexture = rotateTexture(TheTexture, true);
                TheTexture.Apply();
            }*/

        }




  

        /*
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("Cam Width: " + backCam.width.ToString() + "Heigth: " + backCam.height.ToString());
            //Debug.Log("BackGround Width: " + background.rectTransform.sizeDelta.x + "Heigth: " + background.rectTransform.offsetMin);

            float bottom = background.rectTransform.offsetMin.y;
            float top = background.rectTransform.offsetMax.y;
            float right = background.rectTransform.localPosition.x;

            //Touch touch = Input.GetTouch(0);
            position = Input.mousePosition;
            //position = touch.position;

            int ImageX_Pos = (int)(right + position.x) + (0);

            int ImageY_Pos = (int)(position.y + 75);

            position3 = new Vector2(ImageX_Pos,1024 - ImageY_Pos);
            //Debug.Log("BackGround Pos X:" + ImageX_Pos + " Y:" + ImageY_Pos);

            if (position3.x > 0 && position3.y > 0)
            {
                //TheTexture.SetPixels(backCam.GetPixels(ImageX_Pos, ImageY_Pos, (int)position2.x - ImageX_Pos, (int)position2.y - ImageY_Pos));
                //rec = new Vector2Int(Mathf.Abs((int)position2.x - (int)position3.x), Mathf.Abs((int)position2.y - (int)position3.y));


                #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

                    if (rec.x > 20 && rec.y > 20)
                    {
                    //TheTexture = new Texture2D(rec.x,rec.y, TextureFormat.ARGB32, false);
                    //TheTexture.SetPixels(backCam.GetPixels((int)position2.x, (int)position2.y, rec.x, rec.y));
                    //Debug.Log(Mathf.Abs((int)position2.x - ImageX_Pos) + " " + Mathf.Abs((int)position2.y - ImageY_Pos));
                    //TheTexture = rotateTexture(TheTexture, false);
                    //TheTexture.Apply();
                    }

                #endif

                #if UNITY_ANDROID

                     if (rec.x > 20 && rec.y > 20)
                     {
       
                     TheTexture = new Texture2D(rec.x, rec.y, TextureFormat.ARGB32, false);
                     TheTexture.SetPixels(backCam.GetPixels((int)position2.y, (int)position2.x, rec.x, rec.y));
                      Debug.Log(rec.x + " " + rec.y);
                    TheTexture = rotateTexture(TheTexture, true);
                     TheTexture.Apply();
                }
                #endif
            }
        }
        */



        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);


            //TheTexture = background.texture as Texture2D;

            // Move the cube if the screen has the finger moving.
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 pos = touch.position;
                position = pos;

                /*
                pos.x = (pos.x - width) / width;
                pos.y = (pos.y - height) / height;
                position = new Vector3(-pos.x, pos.y, 0.0f);
                background.texture = backCam;

                if (pos.y > -0.55f && pos.y < 0.55f)
                {
                    int y = (int)(236.9594899f * (pos.y * pos.y) - 710.4643966f * pos.y + 371.8418794f);
                    TheTexture.SetPixels(backCam.GetPixels(y, 50, 50, 50));
                    TheTexture = rotateTexture(TheTexture, true);
                    TheTexture.Apply();
                }*/
                /*
                Vector3[] corners = new Vector3[4];
                background.rectTransform.GetWorldCorners(corners);
                Rect newRect = new Rect(corners[0], corners[2] - corners[0]);
                bool inImg = newRect.Contains(pos);

                if (inImg)
                {
                    position = new Vector3(-pos.x, pos.y, 0.0f);
                }
                */
                // Position the cube.
                // transform.position = position;
            }
        }

    }

    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
}
