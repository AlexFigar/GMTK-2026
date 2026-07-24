using UnityEngine;

public class ItsRewindTime : MonoBehaviour
{
    private float time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }

    void FixedUpdate()
    {

    }
}
