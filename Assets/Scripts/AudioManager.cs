using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private int mID = 0;
    private int mCharacter = 0;

    private GameObject mAudioListenerObj;
    private AudioListener mAudioListener;

    private AudioSource bgm;
    private List<AudioSource> sounds;

    private static AudioManager mAudioManager;

    public static AudioManager Instance
    {
        get
        {
            if(mAudioManager == null)
            {
                GameObject obj = new GameObject();
                obj.name = "AudioManager";
                mAudioManager = obj.AddComponent<AudioManager>();
                mAudioManager.Init();
            }

            return mAudioManager;
        }
    }

    public int Character
    {
        get
        {
            return mCharacter;
        }

        set
        {
            if(value == 0 || value == 1)
            {
                mCharacter = value;
            }
        }
    }

    public void SetListenerFollowed(GameObject obj)
    {
        mAudioListener.transform.position = obj.transform.position;   
    }

    private void Init()
    {
        mAudioListenerObj = new GameObject();
        mAudioListenerObj.name = "AudioListener";
        mAudioListenerObj.transform.SetParent(transform, false);
        mAudioListener = mAudioListenerObj.AddComponent<AudioListener>();
        sounds = new List<AudioSource>();
        print("Init OK.");
    }

    public enum AudioCreateType
    {
        ONLY,
        NEW,
    }

    private AudioSource GetAudioSource(string name, AudioCreateType audioCreateType,  GameObject fromPosition, bool loop, float e3d = 0f, float maxRange = 0f)
    {
        AudioClip clip = Resources.Load<AudioClip>("Music/" + name);
        if(audioCreateType == AudioCreateType.ONLY)
        {
            if(bgm == null)
            {
                bgm = fromPosition.AddComponent<AudioSource>();
            }
            else
            {
                if(bgm.isPlaying)
                {
                    bgm.Stop();
                }
            }

            bgm.clip = clip;
            bgm.spatialBlend = e3d;
            bgm.loop = loop;

            return bgm;
        }
        else
        {
            bool bFree = false;
            int i;
            for(i = 0; i < mID; ++i)
            {
                if(!sounds[i].isPlaying)
                {
                    bFree = true;
                    break;
                }
            }
            if(!bFree)
            {
                AudioSource newSource = fromPosition.AddComponent<AudioSource>();
                sounds.Add(newSource);
                mID++;
            }

            sounds[i].clip = clip;
            sounds[i].loop = loop;
            sounds[i].spatialBlend = e3d;
            sounds[i].maxDistance = maxRange;
            sounds[i].transform.position = fromPosition.transform.position;

            return sounds[i];
        }
    }

    public enum SoundType
    {
        START,
        NORMAL,
        MATCH_COMPLETE,
        GAME,
        TOP,

        CHI,
        PENG,
        GANG,
        LIZHI,
        DOUBLE_LIZHI,
        ZIMO,
        RON,

        THROW,
        CLICK,

    }

    public void PlaySound(SoundType type, GameObject fromPosition)
    {
        AudioSource player = null;

        switch(type)
        {
            case SoundType.START:
                player = GetAudioSource("start", AudioCreateType.ONLY, fromPosition, false);
                break;
            case SoundType.NORMAL:
                player = GetAudioSource("bgm", AudioCreateType.ONLY, fromPosition, true);
                break;
            case SoundType.MATCH_COMPLETE:
                player = GetAudioSource("matchcomplete", AudioCreateType.ONLY, fromPosition, false);
                break;
            case SoundType.GAME:
                player = GetAudioSource("game", AudioCreateType.ONLY, fromPosition, true);
                break;

            case SoundType.CHI:
                player = GetAudioSource("act_chi_" + (mCharacter + 1), AudioCreateType.NEW, fromPosition, false, 1.0f, 10.0f);
                break;
            case SoundType.PENG:
                player = GetAudioSource("act_pon_" + (mCharacter + 1), AudioCreateType.NEW, fromPosition, false, 1.0f, 10.0f);
                break;
            case SoundType.GANG:
                player = GetAudioSource("act_kan_" + (mCharacter + 1), AudioCreateType.NEW, fromPosition, false, 1.0f, 10.0f);
                break;
            case SoundType.LIZHI:
                player = GetAudioSource("act_rich_" + (mCharacter + 1), AudioCreateType.NEW, fromPosition, false, 1.0f, 10.0f);
                break;
            case SoundType.DOUBLE_LIZHI:
                player = GetAudioSource("act_drich_" + (mCharacter + 1), AudioCreateType.NEW, fromPosition, false, 1.0f, 10.0f);
                break;
            case SoundType.ZIMO:
                player = GetAudioSource("act_tumo_" + (mCharacter + 1), AudioCreateType.NEW, fromPosition, false, 1.0f, 10.0f);
                break;
            case SoundType.RON:
                player = GetAudioSource("act_ron_" + (mCharacter + 1), AudioCreateType.NEW, fromPosition, false, 1.0f, 10.0f);
                break;

            case SoundType.THROW:
                player = GetAudioSource("btn_click", AudioCreateType.NEW, fromPosition, false);
                break;
            case SoundType.CLICK:
                player = GetAudioSource("mouseclick", AudioCreateType.NEW, fromPosition, false);
                break;
            default:
                break;

        }

        if(player != null)
        {
            player.Play();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if(mAudioListenerObj != null)
        {
            mAudioListenerObj.gameObject.SetActive(!pause);
        }
        
    }
}
