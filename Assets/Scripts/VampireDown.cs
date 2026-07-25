using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class VampireDown : MonoBehaviour
{
    [SerializeField] private bool destroyAfterPlay = false;
    [SerializeField] private float destroyDelay = 0.2f;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string killSnapshotName = "CountDown";
    [SerializeField] private string defaultSnapshotName = "Default";
    [SerializeField] private float snapshotTransitionTime = 0.05f;
    [SerializeField] private float returnToDefaultDelay = 5f;

    private AudioSource audioSource;
    private bool hasTriggered;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandleEntry();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            HandleEntry();
        }
    }

    private void HandleEntry()
    {
        if (hasTriggered)
        {
            return;
        }

        hasTriggered = true;

        if (audioSource != null)
        {
            audioSource.Play();
        }

        TriggerKillSnapshot();

        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        foreach (var collider in GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }

        if (destroyAfterPlay)
        {
            float delay = audioSource != null && audioSource.clip != null ? audioSource.clip.length : destroyDelay;
            Destroy(gameObject, delay);
        }
        else
        {
            StartCoroutine(ReturnToDefaultSnapshot());
        }
    }

    private void TriggerKillSnapshot()
    {
        if (mixer == null)
        {
            return;
        }

        AudioMixerSnapshot killSnapshot = mixer.FindSnapshot(killSnapshotName);
        if (killSnapshot != null)
        {
            killSnapshot.TransitionTo(snapshotTransitionTime);
        }
    }

    private IEnumerator ReturnToDefaultSnapshot()
    {
        yield return new WaitForSeconds(returnToDefaultDelay);

        if (mixer == null)
        {
            yield break;
        }

        AudioMixerSnapshot defaultSnapshot = mixer.FindSnapshot(defaultSnapshotName);
        if (defaultSnapshot != null)
        {
            defaultSnapshot.TransitionTo(snapshotTransitionTime);
        }
    }
}
