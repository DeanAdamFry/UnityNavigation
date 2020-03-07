using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    [SerializeField]
    Vector2 Size = new Vector2(200,100); //Size of Image/will be scaled

    [SerializeField]
    Vector3 Offset = new Vector3(0, 2.0f,0); //Where in relation to object

    Text mDebugText; //Cached for fast access
    
    CanvasScaler mScaler;   //Cache for fast access
    Canvas mCanvas;

    GameObject mGO; //Add a child Go so we can rotate independant of parent
    Transform mTrack;

    public  string Text { 
        get
        {
            return mDebugText.text; //Get Text
        }
        set
        {
            mDebugText.text = value; //Set text
        }
    }
    public Color Color //Change Debug text colour
    {
        set
        {
            mDebugText.color = value;
        }
        get
        {
            return  mDebugText.color;
        }
    }
    public  bool    Show //show debug text
    {
        set
        {
            mGO.SetActive(value);
        }
        get
        {
            return mGO.activeSelf;
        }
    }
    public Transform TrackCam  //Set where to face, or null if not tracking
    {
        set
        {
            mTrack=value;
        }
        get
        {
            return mTrack;
        }
    }

    // Set up Debug text defaults
    void Start()
    {
        //Set up debug text Canvas
        mGO = new GameObject(); //New GO for Canvas, so we can move & rotate it

        mCanvas = mGO.AddComponent<Canvas>(); //Make a canvas
        mScaler = mGO.AddComponent<CanvasScaler>(); //Add scaler
        mCanvas.worldCamera = Camera.main;
        mCanvas.renderMode = RenderMode.WorldSpace; //Put into WorldSpace
        RectTransform tRT = mGO.GetComponent<RectTransform>();
        tRT.SetParent(transform, true); //Make new child for Canvas
        mGO.name = name + "-DebugText"; //Givename for IDE
        tRT.localPosition = Offset; //Move up to desired offset
        tRT.localScale = new Vector3(0.01f, 0.01f, 1.0f); //Scale down to World Canvas
        tRT.sizeDelta = Size;       //Set Desirres size
        mDebugText = mGO.AddComponent<Text>();    //Add text to canvas
        mDebugText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font; //Get a built in font
        Text = "Hello";
        TrackCam = Camera.main.transform; //Default to tracking on
    }

    //Called after all the other Updates
    void LateUpdate()
    {
        if(mTrack!=null)
        {
            //Face the Camera
            mGO.transform.LookAt(2 * mGO.transform.position - mTrack.position);
        }
    }
}
