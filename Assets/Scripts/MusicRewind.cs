using UnityEngine;
using UnityEngine.InputSystem;

public class MusicRewind : MonoBehaviour
{
    [Header("Rewind Audio")]
    [SerializeField] private AnimationCurve rewindCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float minPitch = -1f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float smoothing = 6f;
    [SerializeField] private float transitionDuration = 0.5f;

    private AudioSource audioSource;
    private InputAction rewindAction;
    private float targetPitch = 1f;
    private bool rewindHeld;
    private bool isTransitioning;
    private float transitionTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.pitch = 1f;
            audioSource.playOnAwake = false;
        }

        rewindAction = InputSystem.actions["Rewind"];
        rewindAction.Enable();
    }

    private void OnDisable()
    {
        if (rewindAction != null)
        {
            rewindAction.Disable();
        }
    }

    private void Update()
    {
        if (audioSource == null || rewindAction == null)
        {
            return;
        }

        bool pressedThisFrame = rewindAction.ReadValue<float>() > 0f;

        if (pressedThisFrame && !rewindHeld)
        {
            isTransitioning = true;
            transitionTime = 0f;
        }

        rewindHeld = pressedThisFrame;

        if (rewindHeld)
        {
            if (isTransitioning)
            {
                transitionTime += Time.deltaTime;
                float progress = Mathf.Clamp01(transitionTime / transitionDuration);
                float curveValue = rewindCurve.Evaluate(progress);
                targetPitch = Mathf.Lerp(maxPitch, minPitch, curveValue);

                if (progress >= 1f)
                {
                    isTransitioning = false;
                    targetPitch = minPitch;
                }
            }
            else
            {
                targetPitch = minPitch;
            }
        }
        else
        {
            isTransitioning = false;
            targetPitch = 1f;
        }

        audioSource.pitch = Mathf.Lerp(audioSource.pitch, targetPitch, Time.deltaTime * smoothing);
    }
}
