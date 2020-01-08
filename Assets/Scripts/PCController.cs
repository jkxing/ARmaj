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
    public int round;
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
            positions[i] = new Vector3(0.7f, 0.04f, -0.35f + i * 0.05f);
            rotations[i] = new Vector3(-90f, 0f, -90f);
            positions[i + 28] = new Vector3(-0.7f, 0.04f, 0.35f - i * 0.05f);
            rotations[i + 28] = new Vector3(-90f, 0f, 90f);
            positions[i + 14] = new Vector3(0.35f - i * 0.05f, 0.04f, 0.7f);
            rotations[i + 14] = new Vector3(90f, 0f, 0f);
            positions[i + 42] = new Vector3(-0.35f + i * 0.05f, 0.04f, -0.7f);
            rotations[i + 42] = new Vector3(-90f, 0f, 0f);

            fulu_positions[i] = new Vector3(0.95f, 0.04f, 0.95f - i * 0.05f);
            fulu_rotations[i] = new Vector3(0f, -90f, 0f);
            fulu_positions[i + 28] = new Vector3(-0.95f, 0.04f, -0.95f + i * 0.05f);
            fulu_rotations[i + 28] = new Vector3(0f, 90f, 0f);
            fulu_positions[i + 14] = new Vector3(-0.95f + i * 0.05f, 0.04f, 0.95f);
            fulu_rotations[i + 14] = new Vector3(0f, 180f, 0f);
            fulu_positions[i + 42] = new Vector3(0.95f - i * 0.05f, 0.04f, -0.95f);
            fulu_rotations[i + 42] = new Vector3(0f, 0f, 0f);
        }

        for (int i = 0; i < 30; i++)
        {
            int c = i % 6;
            int r = i / 6;
            pushedCardPos[i] = new Vector3(0.3f + r * 0.07f, 0.05f, -0.15f + c * 0.05f);
            pushedCardPos[i + 60] = new Vector3(-0.3f - r * 0.07f, 0.05f, 0.15f - c * 0.05f);
            pushedCardPos[i + 30] = new Vector3(0.15f - c * 0.05f, 0.05f, 0.3f + r * 0.07f);
            pushedCardPos[i + 90] = new Vector3(-0.15f + c * 0.05f, 0.05f, -0.3f - r * 0.07f);
        }

        for (int i = 0; i < CARD_NUM; i++)
        {
            cards[i] = Instantiate(cards_prefab[i / 4], new Vector3(-1f, -1f, -1f), Quaternion.Euler(0f, 0f, 0f));
            cards[i].transform.localScale = new Vector3(0.0065f, 0.0065f, 0.0065f);
            cards[i].transform.GetComponent<Rigidbody>().freezeRotation = true;
        }
        network = new Network("10.0.0.8");
        /*for test*/
        StartCoroutine(startSimulation());
        StartCoroutine(sendCardStatus());
        StartCoroutine(sendInfoStatus());
    }
    //player and his handcard
    public void updateHandCard(int playerId, int[] idlist)
    {
        int len = idlist.Length;
        for (int i = 0; i < len; i++)
        {
            cards[idlist[i]].transform.localPosition = positions[14 * playerId + i];
            cards[idlist[i]].transform.localRotation = Quaternion.Euler(rotations[14 * playerId + i].x, rotations[14 * playerId + i].y, rotations[14 * playerId + i].z);
        }
    }
    //player and his fulu card
    public void updateFuluCard(int playerId, int[] idlist)
    {
        int len = idlist.Length;
        for (int i = 0; i < len; i++)
        {
            cards[idlist[i]].transform.localPosition = fulu_positions[14 * playerId + i];
            cards[idlist[i]].transform.localRotation = Quaternion.Euler(fulu_rotations[14 * playerId + i].x, fulu_rotations[14 * playerId + i].y, fulu_rotations[14 * playerId + i].z);
        }
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
    }

    bool diff(Vector3 a, Vector3 b)
    {
        return Math.Abs(a.x - b.x) > 0.001 || Math.Abs(a.y - b.y) > 0.001 || Math.Abs(a.x - b.x) > 0.001;
    }

    bool diff(Quaternion a, Quaternion b)
    {
        return Math.Abs(a.x - b.x) > 0.001 || Math.Abs(a.y - b.y) > 0.001 || Math.Abs(a.x - b.x) > 0.001 || (a.w - b.w) > 0.001;
    }

    public void throwCard(int player, int index, int pos)
    {
        cards[index].transform.GetComponent<Rigidbody>().freezeRotation = false;
        cards[index].transform.position += new Vector3(0f, 0.1f, 0f);
        Vector3 force = pushedCardPos[30 * player + pos] - cards[index].transform.position;
        Rigidbody rigidbody = cards[index].transform.GetComponent<Rigidbody>();
        //Debug.Log(force);
        rigidbody.AddForce(force * 400);
        rigidbody.AddTorque(Vector3.up * 20);
        rigidbody.AddTorque(Vector3.forward * 20);
        rigidbody.AddTorque(Vector3.left * 20);
        StartCoroutine(fixThrowCard(index, player, pos));
    }
    //push chi/peng/gang cards to right corner 
    //player id,card list,pos list
    public void pushCards(int player, int[] id, int[] pos)
    {
        for (int i = 0; i < id.Length; i++)
        {
            cards[id[i]].transform.rotation = Quaternion.Euler(fulu_rotations[14 * player].x, fulu_rotations[14 * player].y, fulu_rotations[14 * player].z);
            cards[id[i]].transform.GetComponent<BoxCollider>().enabled = false;
            cards[id[i]].transform.GetComponent<Rigidbody>().useGravity = false;
        }
        StartCoroutine(_pushCards(player, id, pos, 25));
    }

    private IEnumerator _pushCards(int player, int[] id, int[] pos, int tim)
    {
        if (tim == 0)
        {
            if (Server.Players.Count > player)
            {
                AudioInfo x = new AudioInfo();
                Server.Players[player].Send(MessageType.AudioInfo, NetworkUtils.Serialize(x));
            }
            for (int i = 0; i < id.Length; i++)
            {
                cards[id[i]].transform.GetComponent<BoxCollider>().enabled = true;
                cards[id[i]].transform.GetComponent<Rigidbody>().useGravity = true;
            }
            yield break;
        }
        if (tim > 20)
        {
            for (int i = 0; i < id.Length; i++)
            {
                if (player == 0) cards[id[i]].transform.position = new Vector3(cards[id[i]].transform.position.x + (0.95f - cards[id[i]].transform.position.x) / (tim - 20), cards[id[i]].transform.position.y, cards[id[i]].transform.position.z);
                if (player == 1) cards[id[i]].transform.position = new Vector3(cards[id[i]].transform.position.x, cards[id[i]].transform.position.y, cards[id[i]].transform.position.z + (0.95f - cards[id[i]].transform.position.z) / (tim - 20));
                if (player == 2) cards[id[i]].transform.position = new Vector3(cards[id[i]].transform.position.x + (-0.95f - cards[id[i]].transform.position.x) / (tim - 20), cards[id[i]].transform.position.y, cards[id[i]].transform.position.z);
                if (player == 3) cards[id[i]].transform.position = new Vector3(cards[id[i]].transform.position.x, cards[id[i]].transform.position.y, cards[id[i]].transform.position.z + (-0.95f - cards[id[i]].transform.position.z) / (tim - 20));
            }
        }
        else
        {
            for (int i = 0; i < id.Length; i++)
            {
                Vector3 v = (fulu_positions[player * 14 + pos[i]] - cards[id[i]].transform.position) / tim;
                cards[id[i]].transform.position = cards[id[i]].transform.position + v;
                cards[id[i]].transform.rotation = Quaternion.Euler(fulu_rotations[14 * player].x, fulu_rotations[14 * player].y, fulu_rotations[14 * player].z);
            }
        }
        yield return new WaitForSeconds(0.02f);
        StartCoroutine(_pushCards(player, id, pos, tim - 1));
    }
    IEnumerator fixThrowCard(int index, int player, int pos)
    {
        yield return new WaitForSeconds(3.0f);
        cards[index].transform.position = pushedCardPos[30 * player + pos];
        cards[index].transform.rotation = Quaternion.Euler(fulu_rotations[14 * player].x, fulu_rotations[14 * player].y, fulu_rotations[14 * player].z);
        cards[index].transform.GetComponent<Rigidbody>().freezeRotation = true;
    }

    public IEnumerator sendCardStatus()
    {
        if (playing)
        {
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
            if (id.Count > 0)
            {
                CardInfo x = new CardInfo();
                x.changeList = new int[CARD_NUM];
                x.positionx = new float[CARD_NUM];
                x.positiony = new float[CARD_NUM];
                x.positionz = new float[CARD_NUM];
                x.rotationw = new float[CARD_NUM];
                x.rotationx = new float[CARD_NUM];
                x.rotationy = new float[CARD_NUM];
                x.rotationz = new float[CARD_NUM];
                for (int i = 0; i < CARD_NUM; i++)
                {
                    x.changeList[i] = i;
                    x.positionx[i] = CardsPosition[i].x;
                    x.positiony[i] = CardsPosition[i].y;
                    x.positionz[i] = CardsPosition[i].z;
                    x.rotationx[i] = CardsRotation[i].x;
                    x.rotationy[i] = CardsRotation[i].y;
                    x.rotationz[i] = CardsRotation[i].z;
                    x.rotationw[i] = CardsRotation[i].w;
                }
                for (int i = 0; i < Server.Players.Count; i++)
                    Server.Players[i].Send(MessageType.CardInfo, NetworkUtils.Serialize(x));
            }
        }
        yield return new WaitForSeconds(0.02f);
        StartCoroutine(sendCardStatus());
    }
    public IEnumerator sendInfoStatus()
    {
        GameInfo x = new GameInfo();
        x.chi = chi;
        x.peng = peng;
        x.gang = gang;
        x.lizhi = lizhi;
        x.he = he;
        x.currentPlayer = currentPlayer;
        x.time = time;
        x.rank_list = new int[4];
        x.score_list = new int[4];
        x.lizhi_state = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            x.lizhi_state[i] = lizhi_state[i];
            x.rank_list[i] = rank_list[i];
            x.score_list[i] = score_list[i];
        }
        for (int i = 0; i < Server.Players.Count; i++)
        {
            x.yourPlayer = i;
            Server.Players[i].Send(MessageType.GameInfo, NetworkUtils.Serialize(x));
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(sendInfoStatus());
    }
    public IEnumerator startSimulation()
    {
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < 4; i++)
        {
            int[] id = new int[14];
            for (int j = 0; j < 14; j++) id[j] = i * 14 + j;
            updateHandCard(i, id);
        }
        yield return new WaitForSeconds(2f);
        currentPlayer = 0;
        chi = true;
        peng = true;
        gang = true;
        lizhi = true;
        he = true;
        rank_list[0] = 0;
        rank_list[1] = 1;
        rank_list[2] = 2;
        rank_list[3] = 3;
        score_list[0] = 100;
        score_list[1] = 0;
        score_list[2] = 0;
        score_list[3] = 0;
        time = 30;
        yield return new WaitForSeconds(2f);
        currentPlayer = 0;
        chi = false;
        peng = false;
        gang = false;
        lizhi = false;
        he = false;
        time = 29;
        yield return new WaitForSeconds(2f);
        int[] _id = new int[3];
        int[] pos = new int[3];
        for(int i = 0; i < 3; i++)
        {
            _id[i] = i;
            pos[i] = i;
        }
        pushCards(0, _id, pos);
        yield return new WaitForSeconds(2f);
        for(int i=3;i<14;i++)
        {
            throwCard(0, i, i - 3);
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
}
