using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GoogleARCore;
using GoogleARCoreInternal;
using UnityEngine;
using Multiplay;

public class MajPrefab : MonoBehaviour
{
    /// <summary>
    /// The AugmentedImage to visualize.
    /// </summary>
    public AugmentedImage Image;

    public GameObject table;
    public GameObject[] cards_prefab;
    private GameObject[] cards;
    public int CARD_NUM = 136;

    
    public GameObject FrameLowerLeft;
    public GameObject FrameLowerRight;
    public GameObject FrameUpperLeft;
    public GameObject FrameUpperRight;
    private bool[] hasUpdate;
    public Vector3[] CardsPosition;
    public Quaternion[] CardsRotation;

    public void Start()
    {
        cards = new GameObject[CARD_NUM];
        hasUpdate = new bool[CARD_NUM];
        CardsPosition = new Vector3[CARD_NUM];
        CardsRotation = new Quaternion[CARD_NUM];
        for (int i = 0; i < CARD_NUM; i++)
        {
            cards[i] = Instantiate(cards_prefab[i / 4], new Vector3(0,-10000,0), Quaternion.Euler(0,0,0));
            Destroy(cards[i].GetComponent<BoxCollider>());
            Destroy(cards[i].GetComponent<Rigidbody>());
            cards[i].transform.parent = this.transform;
            CardsPosition[i].y = -10000f;
        }
        /*for (int i = 0; i < 14; i++)
        {
            CardsPosition[i] = new Vector3(0.7f, 0.0f, -0.35f + i * 0.05f);
            CardsRotation[i] = new Vector3(-90f, 0f, -90f);
            CardsPosition[i + 28] = new Vector3(-0.7f, 0.0f, 0.35f - i * 0.05f);
            CardsRotation[i + 28] = new Vector3(-90f, 0f, 90f);
            CardsPosition[i + 14] = new Vector3(0.35f - i * 0.05f, 0.0f, 0.7f);
            CardsRotation[i + 14] = new Vector3(90f, 0f, 0f);
            CardsPosition[i + 42] = new Vector3(-0.35f + i * 0.05f, 0.0f, -0.7f);
            CardsRotation[i + 42] = new Vector3(-90f, 0f, 0f);
        }*/
    }
    public void init()
    {
        NetworkClient.Connect("10.0.0.8");
        //table.transform.Rotate(0f, 0f, 0f);
    }
    public void Update()
    {
        /*if (Image == null || Image.TrackingState != TrackingState.Tracking)
        {
            table.SetActive(false);
            foreach (var card in cards)
                card.SetActive(false);
            return;
        }


        float WidthDiv2 = Image.ExtentX / 2;
        table.transform.localPosition = new Vector3(0f, -1f, 0f);
        for (int i = 0; i < CARD_NUM; i++)
        {
            cards[i].transform.localPosition = (WidthDiv2 * i*Vector3.left) ;
        }
        table.SetActive(true);
        foreach (var card in cards)
            card.SetActive(true);*/
        if (Image == null || Image.TrackingState != TrackingState.Tracking)
        {
            FrameLowerLeft.SetActive(false);
            FrameLowerRight.SetActive(false);
            FrameUpperLeft.SetActive(false);
            FrameUpperRight.SetActive(false);
            return;
        }

        float halfWidth = Image.ExtentX * 3.0f  / 2;
        float halfHeight = Image.ExtentZ * 3.0f / 2;
        FrameLowerLeft.transform.localPosition =
            (halfWidth * Vector3.left * 1.05f) + (halfHeight * Vector3.back * 1.05f);
        FrameLowerRight.transform.localPosition =
            (halfWidth * Vector3.right * 1.05f) + (halfHeight * Vector3.back * 1.05f);
        FrameUpperLeft.transform.localPosition =
            (halfWidth * Vector3.left * 1.05f) + (halfHeight * Vector3.forward * 1.05f);
        FrameUpperRight.transform.localPosition =
            (halfWidth * Vector3.right * 1.05f) + (halfHeight * Vector3.forward * 1.05f);

        //table.transform.localPosition = Image.ExtentZ * Vector3.forward;
        table.transform.localScale = new Vector3(Image.ExtentX*3, 0.001f, Image.ExtentZ * 3);

        FrameLowerLeft.SetActive(true);
        FrameLowerRight.SetActive(true);
        FrameUpperLeft.SetActive(true);
        FrameUpperRight.SetActive(true);

        float s = Math.Min(Image.ExtentX, Image.ExtentZ) * 0.01f;
        for(int i = 0; i < CARD_NUM; i++)
        {
            cards[i].transform.localScale = new Vector3(s, s, s);
            cards[i].transform.localPosition = (CardsPosition[i].x * halfWidth * Vector3.right) + (halfHeight * CardsPosition[i].z * Vector3.forward) + CardsPosition[i].y*new Vector3(0f,1f,0f);
            cards[i].transform.localRotation = CardsRotation[i];
        }
    }
    public static int x = 0;
    public void parse(CardInfo result)
    {
        int len = result.changeList.Length;
        //Info.Instance.Print("parse game len is "+ len.ToString(), true);
        Info.Instance.Print("try to parse game len is " + len + " id is " + (++x).ToString(), true);
        for (int i = 0; i < len; i++)
        {
            CardsPosition[result.changeList[i]].x = result.positionx[i];
            CardsPosition[result.changeList[i]].y = result.positiony[i];
            CardsPosition[result.changeList[i]].z = result.positionz[i];
            CardsRotation[result.changeList[i]].x = result.rotationx[i];
            CardsRotation[result.changeList[i]].y = result.rotationy[i];
            CardsRotation[result.changeList[i]].z = result.rotationz[i];
            CardsRotation[result.changeList[i]].w = result.rotationw[i];
        }
    }
}
