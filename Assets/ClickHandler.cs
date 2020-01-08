using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        int i = 0;
        while (i < Input.touchCount)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                this.transform.GetComponent<ARController>().ready = !this.transform.GetComponent<ARController>().ready;
                OurNetworkClient.Instance.sendReady(this.transform.GetComponent<ARController>().ready);
            }
            ++i;
        }
    }
}
