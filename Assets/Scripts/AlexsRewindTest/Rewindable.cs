using Unity.Mathematics;
using UnityEngine;

public class Rewindable : MonoBehaviour
{
    [Header("Rewindable Vars")]
    public bool position = true;
    public bool velocity;
    public bool animationState; //This one is going to be weird sorry!
    public ushort id; //This really is just and index for the ItsRewindTime rewindables list.

    //References
    private Rigidbody2D rb;

    public void Start()
    {
        id = ItsRewindTime.Instance.RegisterRewindable(this);

        if (velocity)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    public void OnDestroy()
    {
        if (ItsRewindTime.Instance) ItsRewindTime.Instance.DeRegisterRewindable(id);
    }

    public RewindData GetRewindData()
    {
        RewindData data = new();
        data.rewindID = id;
        data.position = new float2(transform.position.x, transform.position.y);
        data.rotation = transform.rotation.eulerAngles.z;

        if (velocity)
        {
            data.velocity = rb.linearVelocity;
        }

        return data;
    }

    public void RestoreRewindData(RewindData rewindData)
    {
        if (rewindData.rewindID != id)
        {
            Debug.LogError("Id mismatch");
            return;
        }

        transform.position = new(rewindData.position.x, rewindData.position.y);
        transform.rotation.eulerAngles.Set(0,0,rewindData.rotation);

        if (velocity)
        {
            rb.linearVelocity = rewindData.velocity;
            rb.totalTorque = 0;
            rb.angularVelocity = 0;
        }

    }
}


public struct RewindData
{
    public ushort rewindID;
    public float2 position;
    public float rotation;
    public float2 velocity;
}