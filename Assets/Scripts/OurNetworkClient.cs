using UnityEngine;
using Multiplay;

public class OurNetworkClient : MonoBehaviour
{
    private OurNetworkClient() { }
    public static int x = 0;
    public static OurNetworkClient Instance { get; private set; }

    /// <summary>
    /// 注册
    /// </summary>
    public void sendReady(bool ready)
    {
        Ready request = new Ready();
        request.ready = ready;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.Ready, data);
    }

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        NetworkClient.Register(MessageType.GameInfo, _GameInfo);
        NetworkClient.Register(MessageType.CardInfo, _CardInfo);
        NetworkClient.Register(MessageType.AudioInfo, _AudioInfo);
    }
   
    private void _GameInfo(byte[] data)
    {
        Info.Instance.Print("gameinfo parse", true);
        GameInfo result = NetworkUtils.Deserialize<GameInfo>(data);
        GameObject.Find("Controller").GetComponent<ARController>().parse(result);
    }

    private void _CardInfo(byte[] data)
    {
        Info.Instance.Print("cardinfo parse", true);
        CardInfo result = NetworkUtils.Deserialize<CardInfo>(data);
        MajPrefab obj = GameObject.Find("Controller").GetComponent<ARController>().visualizer;
        obj.parse(result);
    }

    private void _AudioInfo(byte[] data)
    {
        Info.Instance.Print("cardinfo parse", true);
        AudioInfo result = NetworkUtils.Deserialize<AudioInfo>(data);
        MajPrefab obj = GameObject.Find("Controller").GetComponent<ARController>().visualizer;
        obj.table.transform.Find("audio").GetComponent<AudioSource>().Play();
    }
}