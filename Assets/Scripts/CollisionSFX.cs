using System.Collections;
using UnityEngine;

/// <summary>
/// Plays a sound effect on collision with dynamic volume based on impact velocity.
/// The object will ensure it can receive collision callbacks when hit by falling bodies.
/// </summary>
public class CollisionSFX : MonoBehaviour
{
    private const float VELOCITY_SCALE_FACTOR = 50.0f;
    private const float MIN_COLLISION_VELOCITY = 1.5f;

    private AudioSource audioSource;
    private Rigidbody2D rigidBody2D;
    private bool canPlaySound = true;
    private Coroutine playbackLockRoutine;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        if (rigidBody2D == null)
        {
            rigidBody2D = gameObject.AddComponent<Rigidbody2D>();
            rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
            rigidBody2D.gravityScale = 0f;
            rigidBody2D.simulated = true;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        //audioSource.spatialBlend = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 impactPoint = transform.position;
        if (collision.contactCount > 0)
        {
            impactPoint = collision.GetContact(0).point;
        }

        PlayImpactSFX(collision.relativeVelocity.magnitude, impactPoint);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            PlayImpactSFX(other.attachedRigidbody.linearVelocity.magnitude, other.transform.position);
            return;
        }

        PlayImpactSFX(0f, transform.position);
    }

    private void PlayImpactSFX(float impactVelocity, Vector2 worldPosition)
    {
        /*Debug.Log($"CollisionSFX: impact={impactVelocity:F2}, canPlaySound={canPlaySound}, hasAudioSource={audioSource != null}, hasClip={audioSource != null && audioSource.clip != null}");

        if (audioSource == null)
        {
            Debug.LogWarning("CollisionSFX: no AudioSource attached to this GameObject.");
            return;
        }

        if (!canPlaySound)
        {
            Debug.Log("CollisionSFX: playback lock active, skipping sound.");
            return;
        }

        if (impactVelocity < MIN_COLLISION_VELOCITY)
        {
            Debug.Log($"CollisionSFX: impact velocity {impactVelocity:F2} below threshold {MIN_COLLISION_VELOCITY}.");
            return;
        }*/

        float audioLevel = impactVelocity / VELOCITY_SCALE_FACTOR;
        audioLevel = Mathf.Clamp01(audioLevel);

        float pan = 0f;
        Camera camera = Camera.main;
        if (camera != null)
        {
            Vector3 screenPoint = camera.WorldToScreenPoint(worldPosition);
            float normalizedX = screenPoint.x / Mathf.Max(Screen.width, 1);
            pan = Mathf.Clamp01(normalizedX) * 2f - 1f;
        }

        //audioSource.panStereo = pan;
        audioSource.volume = audioLevel;
        audioSource.Play();
//        Debug.Log($"CollisionSFX: playing audio from attached AudioSource at volume {audioLevel:F2}");

        float clipDuration = 0.2f;
        canPlaySound = false;

        if (playbackLockRoutine != null)
        {
            StopCoroutine(playbackLockRoutine);
        }

        playbackLockRoutine = StartCoroutine(ReleasePlaybackLock(clipDuration));
    }

    private IEnumerator ReleasePlaybackLock(float delay)
    {
        yield return new WaitForSeconds(delay);
        canPlaySound = true;
        playbackLockRoutine = null;
    }
}
