using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItsRewindTime : MonoBehaviour
{
    public static ItsRewindTime Instance;

    [Header("Time manipulation")]
    [SerializeField] public float rewindMetre = 1.0f;
    [SerializeField] private float maxRewindTime = 30.0f;
    [SerializeField] private float rewindSpeed = 4.0f; //How 


    private float time;
    public bool rewinding;
    private Dictionary<ushort, Rewindable> rewindables = new();
    private ushort rewindableID = 0;

    private List<RewindKeyFrame> keyframes = new();
    private int keyFrameCount = 0;

    //References
    InputAction rewindAction;

    public ushort RegisterRewindable(Rewindable rewindable)
    {
        rewindableID++;
        rewindables.Add(rewindableID, rewindable);
        return rewindableID;
    }

    public void DeRegisterRewindable(ushort rewindableID)
    {
        rewindables.Remove(rewindableID);
    }

    void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
        rewindAction = InputSystem.actions["Rewind"];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (rewindAction.ReadValue<float>() > 0 && rewindMetre > 0 && keyframes.Count > 0)
        {
            rewinding = true;
        }
        else
        {
            rewinding = false;
        }

        //ITS REWIND TIME
        if (rewinding)
        {
            bool restored = false;
            float rewindTime = Time.deltaTime * rewindSpeed;
            time -= rewindTime;

            while (!restored)
            {
                if (keyframes.Count == 0)
                {
                    break;
                }
                RewindKeyFrame frame = keyframes.Last();

                keyframes.Remove(frame);

                if (frame.timeStamp < time)
                {
                    restored = true;
                    foreach (RewindData item in frame.rewindableData)
                    {
                        rewindables[item.rewindID].RestoreRewindData(item);
                    }
                }
            }
        }

        if (!rewinding) time += Time.deltaTime;
    }

    void FixedUpdate()
    {
        RewindKeyFrame frame = new();

        frame.timeStamp = time;
        frame.rewindableData = new RewindData[rewindables.Count];

        for (int i = 0; i < rewindables.Count; i++)
        {
            Rewindable rewindable = rewindables.ElementAt(i).Value;
            frame.rewindableData[i] = rewindable.GetRewindData();
        }

        keyframes.Add(frame);
        keyFrameCount ++;
        //Debug.Log("Frame Captured");
    }

    void OnDestroy()
    {
        rewindables.Clear();
    }
}
