using System;
using UnityEngine;

namespace Multiplay
{
    /// <summary>
    /// 消息类型
    /// </summary>
    /// 
    [Serializable]
    public class myVector3
    {
        public float x, y, z;
        public myVector3()
        {
            x = 0;
            y = 0;
            z = 0;
        }
    }
    [Serializable]
    public class myQuaternion
    {
        public float x, y, z, w;
        public myQuaternion()
        {
            x = 0;
            y = 0;
            z = 0;
            w = 0;
        }
    }
    public enum MessageType
    {
        None,         //空类型
        Ready,        //客户端ready
        GameInfo,        //客户端ready
    }
    [Serializable]
    public class GameInfo
    {
        public int currentPlayer;
        public int yourPlayer;
        public bool chi,peng,gang,lizhi,he;
        public int time;
        public int[] changeList;
        public float[] positionx;
        public float[] positiony;
        public float[] positionz;
        public float[] rotationx;
        public float[] rotationy;
        public float[] rotationz;
        public float[] rotationw;
        public int[] rank_list;
        public int[] score_list;
        public bool[] lizhi_state;
    }
    [Serializable]
    public class Ready
    {
        public bool ready;
    }
}