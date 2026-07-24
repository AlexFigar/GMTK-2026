using System.Collections.Generic;
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
    private bool rewinding;
    private Dictionary<ushort, Rewindable> rewindables = new();
    private ushort rewindableID = 0;

    //References
    InputAction rewindAction;

    public ushort RegisterRewindable(Rewindable rewindable)
    {
        rewindableID++;
        rewindables.Add(rewindableID,rewindable);
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
        if (rewindAction.ReadValue<float>() > 0 && rewindMetre > 0)
        {
            rewinding = true;
        }
        else
        {
            rewinding = false;
        }

        if (!rewinding) time += Time.deltaTime;
    }

    void FixedUpdate()
    {
        foreach (var item in rewindables)
        {

        }
    }

    void OnDestroy()
    {
        rewindables.Clear();
    }
}
