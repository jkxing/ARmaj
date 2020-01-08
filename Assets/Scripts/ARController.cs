using GoogleARCore;
using Multiplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARController : MonoBehaviour
{
    public MajPrefab AugmentedImageVisualizerPrefab;
    public MajPrefab visualizer;
    /// <summary>
    /// The overlay containing the fit to scan user guide.
    /// </summary>
    public GameObject FitToScanOverlay;

    private Dictionary<int, MajPrefab> m_Visualizers
        = new Dictionary<int, MajPrefab>();

    private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();

    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake()
    {
        // Enable ARCore to target 60fps camera capture frame rate on supported devices.
        // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// The Unity Update method.
    /// </summary>
    bool isFirstAndTest = false;
    public bool ready = false;
    public bool playing = false;
    public bool Connected = false;
    public float remain_time = 30f;
    public void changeUI()
    {
        if (Connected)
        {
            GameObject.Find("Canvas/Connected/Text").GetComponent<Text>().text = "已连接";
        }
        else
        {
            GameObject.Find("Canvas/Connected/Text").GetComponent<Text>().text = "未连接";
        }
        if (ready)
        {
            GameObject.Find("Canvas/Image/Text").GetComponent<Text>().text = "已准备";
        }
        else
        {
            GameObject.Find("Canvas/Image/Text").GetComponent<Text>().text = "准备";
        }
        if (playing)
        {
            GameObject.Find("Canvas/Image").SetActive(false);
        }
        else
        {
            GameObject.Find("Canvas/Image").SetActive(true);
        }
    }
    public void Update()
    {
        changeUI();
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        // Get updated augmented images for this frame.
        Session.GetTrackables<AugmentedImage>(
            m_TempAugmentedImages, TrackableQueryFilter.Updated);

        // Create visualizers and anchors for updated augmented images that are tracking and do
        // not previously have a visualizer. Remove visualizers for stopped images.
        foreach (var image in m_TempAugmentedImages)
        {
            visualizer = null;
            m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);
            if (image.TrackingState == TrackingState.Tracking && visualizer == null)
            {
                // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                Anchor anchor = image.CreateAnchor(image.CenterPose);
                visualizer = (MajPrefab)Instantiate(
                    AugmentedImageVisualizerPrefab, anchor.transform);
                visualizer.Image = image;
                visualizer.init();
                m_Visualizers.Add(image.DatabaseIndex, visualizer);
            }
            else if (image.TrackingState == TrackingState.Stopped && visualizer != null)
            {
                m_Visualizers.Remove(image.DatabaseIndex);
                GameObject.Destroy(visualizer.gameObject);
            }
        }

        // Show the fit-to-scan overlay if there are no images that are Tracking.
        foreach (var visualizer in m_Visualizers.Values)
        {
            if (visualizer.Image.TrackingState == TrackingState.Tracking)
            {
                FitToScanOverlay.SetActive(false);
                return;
            }
        }
        FitToScanOverlay.SetActive(true);
    }
    public GameObject he, lizhi, chi, peng, gang, time_left_box;
    public Text[] playerId;
    public Text[] playerScore;
    public Text time_left;
    public void parse(GameInfo result)
    {
        if (result.currentPlayer == result.yourPlayer)
        {
            he.SetActive(result.he);
            lizhi.SetActive(result.lizhi);
            chi.SetActive(result.chi);
            peng.SetActive(result.peng);
            gang.SetActive(result.gang);
            time_left_box.SetActive(true);
            time_left.text = result.time.ToString();
        }
        for(int i = 0; i < 4; i++)
        {
            playerId[i].text = result.rank_list[i].ToString();
            playerScore[i].text = result.score_list[i].ToString();
        }
    }
}
