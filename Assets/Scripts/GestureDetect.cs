using UnityEngine;
using System.Collections;
using Leap;

public class GestureDetect : MonoBehaviour
{
    private HandUpdate mHandUpdate;
    private Hand mLeftHand;
    private Hand mRightHand;

    private Vector grabPosition = Vector.Zero;

    private Vector resultPosition;
    private const int OPER_TYPE_COUNT = 8; 

    private int[] validFrames;
    private int totalFrames = 0;
    private bool choose = false;

    protected float deltaVelocity = 0.7f;
    protected float deltaNormal = 0.8f;
    private int waitFrames = 0;

    private int mOperID = 0;
    private int mGrabID = 0;

    [Tooltip("Velocity (m/s) move toward ")]

    private static GestureDetect mInstance;

    public static GestureDetect Instance
    {
        get
        {
            if(mInstance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "GestureDetect";
                mInstance = obj.AddComponent<GestureDetect>();
                mInstance.Init();
            }

            return mInstance;
        }
    }

    public void Init()
    {
        mHandUpdate = HandUpdate.Instance;
        mLeftHand = mHandUpdate.LeftHand;
        mRightHand = mHandUpdate.RightHand;

        validFrames = new int[OPER_TYPE_COUNT];

        for (int i = 0; i < OPER_TYPE_COUNT; ++i)
        {
            validFrames[i] = 0;
        }

    }

    private int PositionToID(Vector pos)
    {
        int res = 0;
        res = (int)((pos.x + 0.35f) / 0.05f);
        if (res < 0)
            return 0;
        if (res >= 14)
            return 13;
        return res;
    }

    public int GrabID
    {
        get
        {
            return mGrabID;
        }

        private set
        {
            if(value == mGrabID)
                return;
            mGrabID = value;
            OnGrabIDChange?.Invoke(value);
        }
    }

    public OperType Type { get; private set; } = OperType.NO_OP;

    public int OperID
    {
        get
        {
            return mOperID;
        }

        private set
        {
            if (value == mOperID)
                return;
            mOperID = value;
            OnVariableChange?.Invoke(value);
        }
    }

    public delegate void OnOperIDChangeDelegate(int newVal);

    // 监听新操作的ID变化
    public event OnOperIDChangeDelegate OnVariableChange;
    public event OnOperIDChangeDelegate OnGrabIDChange;

    public bool IsGrabRightHand()
    {
        if (mRightHand == null)
        {
            return false;
        }
        return mRightHand.GrabStrength > 0.5f;
    }

    public bool IsRightHandOpen()
    {
        if(mRightHand == null)
        {
            return false;
        }
        return mRightHand.GrabStrength == 0;
    }

    public bool IsGrabLeftHand()
    {
        if (mLeftHand == null)
        {
            return false;
        }
        return mLeftHand.GrabStrength > 0.8f;
    }

    public bool RightHandleStable()
    {
        if(mRightHand == null)
        {
            return false;
        }
        return mRightHand.PalmVelocity.Magnitude < 0.1f;
    }

    public bool RightHandLeftSwipe()
    {
        if (mRightHand == null)
        {
            return false;
        }
        return mRightHand.PalmVelocity.x < -deltaVelocity;
    }

    public bool RightHandRightSwipe()
    {
        if (mRightHand == null)
        {
            return false;
        }
        return mRightHand.PalmVelocity.x > deltaVelocity;
    }

    public bool RightHandUpSwipe()
    {
        if (mRightHand == null)
        {
            return false;
        }
        return mRightHand.PalmVelocity.y > deltaVelocity;
    }

    public bool RightHandDownSwipe()
    {
        if (mRightHand == null)
        {
            return false;
        }
        return mRightHand.PalmVelocity.y < -deltaVelocity;
    }

    public bool RightHandLeftNormal()
    {
        if(mRightHand == null)
        {
            return false;
        }

        return mRightHand.PalmNormal.x < -deltaNormal;
    }

    public bool RightHandRightNormal()
    {
        if (mRightHand == null)
        {
            return false;
        }
        return mRightHand.PalmNormal.x > deltaNormal;
    }

    public bool RightHandUpNormal()
    {
        if (mRightHand == null)
        {
            return false;
        }
        return mRightHand.PalmNormal.y > deltaNormal;
    }

    public bool RightHandDownNormal()
    {
        if (mRightHand == null)
        {
            return false;
        }
        return mRightHand.PalmNormal.y < -deltaNormal;
    }

    public bool LeftHandLeftNormal()
    {
        if (mLeftHand == null)
        {
            return false;
        }
        return mLeftHand.PalmNormal.x < -deltaNormal;
    }

    public bool LeftHandRightNormal()
    {
        if (mLeftHand == null)
        {
            return false;
        }
        return mLeftHand.PalmNormal.x > deltaNormal;
    }

    public bool LeftHandUpNormal()
    {
        if (mLeftHand == null)
        {
            return false;
        }
        return mLeftHand.PalmNormal.y > deltaNormal;
    }

    public bool LeftHandDownNormal()
    {
        if (mLeftHand == null)
        {
            return false;
        }
        return mLeftHand.PalmNormal.y < -deltaNormal;
    }

    public enum OperType
    {
        CHI,
        PENG,
        GANG,
        LIZHI,
        CHOOSE,
        UNCHOOSE,
        THROW,
        HU,

        NO_OP,
    }

    // Update is called once per frame
    void Update()
    {  
        if(waitFrames > 0)
        {
            --waitFrames;
            return;
        }


        mRightHand = mHandUpdate.RightHand;
        mLeftHand = mHandUpdate.LeftHand;

        if(mRightHand == null && mLeftHand == null && totalFrames == 0)
        {
            return;
        }

        if(choose)
        {
            if(RightHandLeftNormal() && RightHandLeftSwipe())
            {
                validFrames[(int)OperType.THROW]++;
            }
            else if(IsRightHandOpen() && RightHandDownNormal() && RightHandleStable())
            {
                validFrames[(int)OperType.UNCHOOSE]++;
            }
        } 
        else
        {
            if(mRightHand != null) 
            {
                //grabPosition = mRightHand.PalmPosition;
                mGrabID = PositionToID(mRightHand.PalmPosition);
            }

            if (IsGrabRightHand())
            {
                //grabPosition = mRightHand.PalmPosition;
                validFrames[(int)OperType.CHOOSE]++;
            }
            else if (RightHandLeftNormal() && RightHandLeftSwipe())
            {
                validFrames[(int)OperType.CHI]++;
            }
            else if (RightHandLeftNormal() && RightHandRightSwipe())
            {
                validFrames[(int)OperType.PENG]++;
            }
            else if (RightHandUpNormal() && RightHandUpSwipe())
            {
                validFrames[(int)OperType.GANG]++;
            }
            else if (RightHandDownNormal() && RightHandDownSwipe())
            {
                validFrames[(int)OperType.HU]++;
            }
            else if (RightHandUpNormal() && RightHandDownSwipe())
            {
                validFrames[(int)OperType.LIZHI]++;
            }
        }

        totalFrames++;

        if(totalFrames >= 10)
        {
            // print(totalFrames);
            for(int i = 0; i < OPER_TYPE_COUNT; ++i)
            {
                // print(validFrames[i]);
                if ((float)(validFrames[i]) / totalFrames > 0.2f)
                {
                    Type = (OperType)i;
                    resultPosition = grabPosition;
                    waitFrames = 200;
                    OperID = (OperID + 1) % 100;
                    if (i == (int)OperType.CHOOSE)
                        choose = true;
                    else if (i == (int)OperType.UNCHOOSE || i == (int)OperType.THROW)
                        choose = false;
                    

                    // print(validFrames[i]);
                    print(Type);
                    print(GrabID);
                    if(mRightHand != null)
                        print(mRightHand.Id);
                    break;
                }
            }

            totalFrames = 0;
            for(int i = 0; i < OPER_TYPE_COUNT; ++i)
            {
                validFrames[i] = 0;
            }
            grabPosition = Vector.Zero;

        }
        
    }
}
