using System;
using UnityEngine;

namespace Multiplay
{
    /// <summary>
    /// 消息类型
    /// </summary>
    /// 
    public enum MessageType
    {
        None,         //空类型
        Ready,        //客户端ready
        GameInfo,        //局面信息
        CardInfo,        //牌面信息
        AudioInfo,      //音频信息
        SelectInfo,     //当前选中的牌
    }
    [Serializable]
    public class GameInfo
    {
        public int currentPlayer;
        public int yourPlayer;
        public bool chi,peng,gang,lizhi,he;
        public int time;
        public int[] rank_list;
        public int[] score_list;
        public bool[] lizhi_state;
    }
    [Serializable]
    public class CardInfo
    {
        public int[] changeList;
        public float[] positionx;
        public float[] positiony;
        public float[] positionz;
        public float[] rotationx;
        public float[] rotationy;
        public float[] rotationz;
        public float[] rotationw;
    }
    [Serializable]
    public class Ready
    {
        public bool ready;
    }
    [Serializable]
    public class AudioInfo
    {
        public int player;
    }
    [Serializable]
    public class SelectInfo
    {
        public int id;
    }
}