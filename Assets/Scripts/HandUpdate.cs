using UnityEngine;
using System.Collections.Generic;
using Leap.Unity;
using Leap;

public class HandUpdate : MonoBehaviour
{
    private LeapServiceProvider lsp;

    private static HandUpdate mInstance;

    public Hand LeftHand { get; private set; }

    public Hand RightHand { get; private set; }

    public static HandUpdate Instance
    {
        get
        {
            if(mInstance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "HandUpdate";
                mInstance = obj.AddComponent<HandUpdate>();
                DontDestroyOnLoad(mInstance);
                mInstance.Init();
            }

            return mInstance;
        }
    }

    private void Init()
    {
        GameObject obj = new GameObject();
        obj.name = "LeapServiceProvider";
        lsp = obj.AddComponent<LeapServiceProvider>();
        DontDestroyOnLoad(lsp);
    }

    public void Close()
    {
        mInstance = null;
    }

    // Use this for initialization
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {

        if(mInstance == null)
        {
            Debug.Log("Update thread not started.");
            return;
        }

        if(!lsp.IsConnected())
        {
            Debug.Log("Leap motion not connected.");
        } 
        else
        {
            List<Hand> handList = lsp.CurrentFrame.Hands;

            if(handList.Count > 0)
            {
                Hand hand = handList[0];
                if(hand.IsLeft)
                {
                    LeftHand = hand;
                    RightHand = null;
                }
                else
                {
                    RightHand = hand;
                    LeftHand = null;
                }       
            }
            else
            {
                RightHand = null;
                LeftHand = null;
            }
        }
    }
}
