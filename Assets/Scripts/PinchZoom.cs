using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PinchZoom : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.

    public GameObject target;

    public SwipeDirection direction = SwipeDirection.None;
    public enum SwipeDirection { Right, Left, Up, Down, None }
    public float errorRange = 22f;
    public float minSwipeDistance = 2f;
    private Touch initialTouch;

    public bool toachs = false;
    public Slider Slider;

    IEnumerator LoadMenu()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("menu");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void toach()
    {
        toachs = true;
    }


    void CalculateSwipeDirection(float deltaX, float deltaY)
    {
        bool isHorizontalSwipe = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);

        // horizontal swipe
        if (isHorizontalSwipe && Mathf.Abs(deltaY) <= errorRange)
        {
            //right
            if (deltaX > 0)
                direction = SwipeDirection.Right;
            //left
            else if (deltaX < 0)
                direction = SwipeDirection.Left;
        }
        //vertical swipe
        else if (!isHorizontalSwipe && Mathf.Abs(deltaX) <= errorRange)
        {
            //up
            if (deltaY > 0)
                direction = SwipeDirection.Up;
            //down
            else if (deltaY < 0)
                direction = SwipeDirection.Down;
        }
        //diagonal swipe
        else
        {
            swiping = false;
        }
    }

    public bool swiping;

    void Start()
    {
        //transform.LookAt(target.transform.position);
    }
    void Update()
    {


        if (target != null && toachs)
        {
            transform.LookAt(new Vector3(target.transform.position.x, target.transform.position.y + 20, target.transform.position.z));
            transform.RotateAround(target.transform.position, transform.up, Time.deltaTime * 22.4f);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                StartCoroutine(LoadMenu());
                return;
            }
        }

        if (Input.touchCount <= 0)
            return;

        foreach (var touch in Input.touches)
        {

            if (touch.phase == TouchPhase.Began)
            {
                initialTouch = touch;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                var deltaX = touch.position.x - initialTouch.position.x; //greater than 0 is right and less than zero is left
                var deltaY = touch.position.y - initialTouch.position.y; //greater than 0 is up and less than zero is down
                var swipeDistance = Mathf.Abs(deltaX) + Mathf.Abs(deltaY);


                swiping = true;
                CalculateSwipeDirection(deltaX, deltaY);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                initialTouch = new Touch();
                swiping = false;
                toachs = false;
                direction = SwipeDirection.None;
            }
            else if (touch.phase == TouchPhase.Canceled)
            {
                initialTouch = new Touch();
                swiping = false;
                toachs = false;
                direction = SwipeDirection.None;
            }
        }


        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (Camera.main.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                Camera.main.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                Camera.main.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 0.1f, 179.9f);
            }
        }
    }
}
