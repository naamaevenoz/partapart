using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;

    [Header("Gameplay SFX Clips")]
    public AudioClip objectPushClip;
    public AudioClip doorMoveClip;
    public AudioClip platformMoveClip;
    public AudioClip pressurePlateClickClip;
    public AudioClip backGroundClip1;

    [Header("Settings")]
    [SerializeField] private bool interruptVoiceLines = true;

    private Coroutine currentVoiceCoroutine = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        if (voiceSource == null) voiceSource = gameObject.AddComponent<AudioSource>();

        if (backGroundClip1 != null)
        {
            PlayMusic(backGroundClip1);
        }
    }

    // ---------- Music ----------
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic() => musicSource?.Stop();

    // ---------- SFX ----------
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // ---------- Gameplay-Specific SFX ----------
    public void PlayPushSound() => PlaySFX(objectPushClip);
    public void PlayDoorMoveSound() => PlaySFX(doorMoveClip);
    public void PlayPlatformMoveSound() => PlaySFX(platformMoveClip);
    public void PlayPressurePlateSound() => PlaySFX(pressurePlateClickClip);
    public void PlayClickSound() => PlayPressurePlateSound();

    // ---------- Voice ----------
    public void PlayVoiceLine(AudioClip clip, float delay = 0f)
    {
        if (voiceSource == null || clip == null) return;

        if (interruptVoiceLines && voiceSource.isPlaying)
        {
            voiceSource.Stop();
            if (currentVoiceCoroutine != null)
            {
                StopCoroutine(currentVoiceCoroutine);
                currentVoiceCoroutine = null;
            }
        }

        if (!interruptVoiceLines && voiceSource.isPlaying) return;

        currentVoiceCoroutine = StartCoroutine(PlayVoiceWithDelay(clip, delay));
    }

    private IEnumerator PlayVoiceWithDelay(AudioClip clip, float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        if (voiceSource != null && clip != null)
        {
            voiceSource.clip = clip;
            voiceSource.Play();
        }

        currentVoiceCoroutine = null;
    }
}
