using UnityEngine;

/// <summary>
/// Plays the ambience and the event sounds. One place to keep the volume balance
/// under control: music sits well below the effects so it never masks them.
/// </summary>
public class AudioDirector : MonoBehaviour
{
    public static AudioDirector Instance { get; private set; }

    [Header("Sources")]
    [Tooltip("Looping source for the ambient music or ambience.")]
    public AudioSource musicSource;

    [Tooltip("Source used for one-shot effects.")]
    public AudioSource effectsSource;

    [Header("Clips")]
    public AudioClip ambientClip;
    public AudioClip pickupClip;
    public AudioClip goalClip;
    public AudioClip jumpClip;
    public AudioClip impactClip;
    public AudioClip lockedClip;

    void Awake()
    {
        Instance = this;

        if (musicSource != null)
        {
            musicSource.clip = ambientClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlayPickup()
    {
        PlayOneShot(pickupClip);
    }

    public void PlayGoal()
    {
        PlayOneShot(goalClip);
    }

    public void PlayJump()
    {
        PlayOneShot(jumpClip);
    }

    public void PlayImpact()
    {
        PlayOneShot(impactClip);
    }

    public void PlayLocked()
    {
        PlayOneShot(lockedClip);
    }

    void PlayOneShot(AudioClip clip)
    {
        if (clip == null || effectsSource == null)
        {
            return;
        }

        effectsSource.PlayOneShot(clip);
    }
}
