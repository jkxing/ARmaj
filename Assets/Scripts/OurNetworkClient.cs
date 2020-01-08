using UnityEngine;
using Multiplay;

public class OurNetworkClient : MonoBehaviour
{
    private OurNetworkClient() { }
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
    }
   
    private void _GameInfo(byte[] data)
    {
        GameInfo result = NetworkUtils.Deserialize<GameInfo>(data);
        GameObject.Find("Controller").GetComponent<ARController>().playing = true;
        //Info.Instance.Print("arcontroller parse start  ", true);
        GameObject.Find("Controller").GetComponent<ARController>().parse(result);
        //Info.Instance.Print("arcontroller parse success  ", true);
        if (result.currentPlayer == result.yourPlayer || true)
            GameObject.Find("Controller").GetComponent<ARController>().remain_time = result.time;
        MajPrefab obj = GameObject.Find("Controller").GetComponent<ARController>().visualizer;
        //Info.Instance.Print("try to parse game  ", true);
        obj.parse(result);
    }
}