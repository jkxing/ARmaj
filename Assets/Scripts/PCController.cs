using Multiplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCController : MonoBehaviour
{
    public Vector3[] CardsPosition;
    public Quaternion[] CardsRotation;
    public GameObject[] cards;

    public int CARD_NUM = 136;
    public GameObject[] cards_prefab;
    public Vector3[] positions;
    public Vector3[] fulu_positions;
    public Vector3[] rotations;
    public Vector3[] fulu_rotations;
    public Vector3[] pushedCardPos;

    public float timer = 2f;
    public int cnt = 0;
    Network network;
    // Start is called before the first frame update

    // Current info
    public int currentPlayer;
    public int time;
    public bool chi, peng, gang, he, lizhi;
    public bool[] lizhi_state;
    public int[] score_list;
    public int[] rank_list;
    public bool playing;
    void Start()
    {
        cards = new GameObject[CARD_NUM];
        CardsPosition = new Vector3[CARD_NUM];
        CardsRotation = new Quaternion[CARD_NUM];
        positions = new Vector3[56];
        fulu_positions = new Vector3[56];
        rotations = new Vector3[56];
        fulu_rotations = new Vector3[56];
        pushedCardPos = new Vector3[120];
        lizhi_state = new bool[4];
        score_list = new int[4];
        rank_list = new int[4];
        for (int i = 0; i < 14; i++)
        {
            positions[i] = new Vector3(0.7f, 0.05f, -0.35f +i*0.05f);
            rotations[i] = new Vector3(-90f, 0f, -90f);
            positions[i+28] = new Vector3(-0.7f, 0.05f, 0.35f - i * 0.05f);
            rotations[i+28] = new Vector3(-90f,0f,90f);
            positions[i + 14] = new Vector3(0.35f - i * 0.05f, 0.05f, 0.7f);
            rotations[i+14] = new Vector3(90f, 0f, 0f);
            positions[i + 42] = new Vector3(-0.35f + i * 0.05f, 0.05f, -0.7f);
            rotations[i+42] = new Vector3(-90f, 0f, 0f);

            fulu_positions[i] = new Vector3(0.95f, 0.05f, 0.95f - i * 0.05f);
            fulu_rotations[i] = new Vector3(0f,-90f,0f);
            fulu_positions[i+28] = new Vector3(-0.95f, 0.05f, -0.95f + i * 0.05f);
            fulu_rotations[i+28] = new Vector3(0f, 90f, 0f);
            fulu_positions[i + 14] = new Vector3(-0.95f + i * 0.05f, 0.05f, 0.95f);
            fulu_rotations[i + 14] = new Vector3(0f, 180f, 0f);
            fulu_positions[i+42] = new Vector3(0.95f - i * 0.05f, 0.05f, -0.95f);
            fulu_rotations[i+42] = new Vector3(0f, 0f, 0f);
            
        }

        for(int i = 0; i < 30; i++)
        {
            int c = i % 6;
            int r = i / 6;
            pushedCardPos[i] = new Vector3(0.3f + r * 0.07f, 0.05f, -0.15f + c * 0.05f);
            pushedCardPos[i+60] = new Vector3(-0.3f - r * 0.07f, 0.05f, 0.15f - c * 0.05f);
            pushedCardPos[i+30] = new Vector3(0.15f - c * 0.05f, 0.05f, 0.3f + r * 0.07f);
            pushedCardPos[i+90] = new Vector3(-0.15f + c * 0.05f, 0.05f, -0.3f - r * 0.07f);
        }
        for (int i = 0; i < CARD_NUM; i++)
        {
            cards[i] = Instantiate(cards_prefab[i/4], new Vector3(-1f,-1f,-1f), Quaternion.Euler(0f,0f,0f));
            cards[i].transform.localScale = new Vector3(0.007f, 0.007f, 0.007f);
        }
        network = new Network("10.0.0.8");
        /*for test*/
        for (int i = 0; i < 4; i++)
        {
            int[] id = new int[14];
            for (int j = 0; j < 14; j++) id[j] = i * 14 + j;
            updateHandCard(i, id);
        }
        for (int i = 0; i < 4; i++)
        {
            int[] id = new int[14];
            for (int j = 0; j < 14; j++) id[j] = i * 14 + j + 56;
            updateFuluCard(i, id);
        }

    }

    public void updateHandCard(int playerId, int[] idlist)
    {
        int len = idlist.Length;
        for(int i = 0; i < len; i++)
        {
            cards[idlist[i]].transform.localPosition = positions[14 * playerId + i];
            cards[idlist[i]].transform.localRotation = Quaternion.Euler(rotations[14 * playerId + i].x, rotations[14 * playerId + i].y, rotations[14 * playerId + i].z);
        }
    }

    public void updateFuluCard(int playerId, int[] idlist)
    {
        int len = idlist.Length;
        for (int i = 0; i < len; i++)
        {
            cards[idlist[i]].transform.localPosition = fulu_positions[14 * playerId + i];
            cards[idlist[i]].transform.localRotation = Quaternion.Euler(fulu_rotations[14 * playerId + i].x, fulu_rotations[14 * playerId + i].y, fulu_rotations[14 * playerId + i].z);
        }
    }
    public void placeCard(int i,GameObject c)
    {
        cards[i] = Instantiate(c, new Vector3(positions[i].x, 0.1f, positions[i].z), Quaternion.Euler(rotations[i].x, rotations[i].y, rotations[i].z));
    }
    // Update is called once per frame
    void Update()
    {
        if (Server.Players.Count > 0)
        {
            playing = true;
        }
        else
        {
            playing = false;
        }
        if (playing)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                throwCard(cnt, cnt / 14, cnt % 14);
                cnt++;
                timer = 2f;
            }
            List<int> id = new List<int>();
            for (int i = 0; i < CARD_NUM; i++)
            {
                if (diff(CardsPosition[i], cards[i].transform.position) || diff(CardsRotation[i], cards[i].transform.rotation))
                {
                    if (cards[i].transform.position.y > 0)
                    {
                        id.Add(i);
                    }
                }
                CardsPosition[i] = cards[i].transform.position;
                CardsRotation[i] = cards[i].transform.rotation;
            }
            GameInfo x = new GameInfo();
            x.chi = chi;
            x.peng = peng;
            x.gang = gang;
            x.lizhi = lizhi;
            x.he = he;
            x.currentPlayer = 1;
            x.time = 30;
            x.changeList = new int[id.Count];
            x.positionx = new float[id.Count];
            x.positiony = new float[id.Count];
            x.positionz = new float[id.Count];
            x.rotationw = new float[id.Count];
            x.rotationx = new float[id.Count];
            x.rotationy = new float[id.Count];
            x.rotationz = new float[id.Count];
            x.rank_list = new int[4];
            x.score_list = new int[4];
            x.lizhi_state = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                x.lizhi_state[i] = lizhi_state[i];
                x.rank_list[i] = rank_list[i];
                x.score_list[i] = score_list[i];
            }
            Debug.Log("============================");
            for (int i = 0; i < id.Count; i++)
            {
                Debug.Log(id[i].ToString() + " has to change");
                x.changeList[i] = id[i];
                x.positionx[i] = CardsPosition[id[i]].x;
                x.positiony[i] = CardsPosition[id[i]].y;
                x.positionz[i] = CardsPosition[id[i]].z;
                x.rotationx[i] = CardsRotation[id[i]].x;
                x.rotationy[i] = CardsRotation[id[i]].y;
                x.rotationz[i] = CardsRotation[id[i]].z;
                x.rotationw[i] = CardsRotation[id[i]].w;
            }
            for (int i = 0; i < Server.Players.Count; i++)
            {
                x.yourPlayer = i;
                Server.Players[i].Send(MessageType.GameInfo, NetworkUtils.Serialize(x));
            }
        }
    }

    bool diff(Vector3 a,Vector3 b)
    {
        return Math.Abs(a.x - b.x) > 0.1 || Math.Abs(a.y - b.y) > 0.1 || Math.Abs(a.x - b.x) > 0.1;
    }

    bool diff(Quaternion a, Quaternion b)
    {
        return Math.Abs(a.x - b.x) > 0.1 || Math.Abs(a.y - b.y) > 0.1 || Math.Abs(a.x - b.x) > 0.1 || (a.w-b.w)>0.1;
    }

    public void throwCard(int index, int player, int pos)
    {
        cards[index].transform.position += new Vector3(0f, 0.1f, 0f);
        Vector3 force = pushedCardPos[30*player+pos] - cards[index].transform.position;
        Rigidbody rigidbody = cards[index].transform.GetComponent<Rigidbody>();
        Debug.Log(force);
        rigidbody.AddForce(force * 250);
        rigidbody.AddTorque(Vector3.up * 20);
        rigidbody.AddTorque(Vector3.forward * 20);
        rigidbody.AddTorque(Vector3.left * 20);
        StartCoroutine(fixThrowCard(index,player,pos));
    }

    IEnumerator fixThrowCard(int index, int player, int pos)
    {
        //3s后执行Debug.Log;
        yield return new WaitForSeconds(3.0f);
        Debug.Log("启动协程3s后");
        cards[index].transform.position = pushedCardPos[30 * player + pos];
        cards[index].transform.rotation = Quaternion.Euler(fulu_rotations[14 * player].x, fulu_rotations[14 * player].y, fulu_rotations[14 * player].z);
    }

    public void updateSingleCardPosition(int index,Vector3 position, Vector3 rotation)
    {
        cards[index].transform.localPosition = position;
        cards[index].transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

}
