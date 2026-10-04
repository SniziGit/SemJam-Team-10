using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Sound Effects")]
    public AudioClip footstepSound;
    public AudioClip attractSound;
    public AudioClip repelSound;
    public AudioClip winSound;
    public AudioClip deathSound;
    public AudioClip playerCollisionSound;

    [Header("Background Music")]
    public AudioClip mainMenuMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        UpdateVolumes();
    }

    private void Start()
    {
        // Don't auto-play music - let LevelMusicManager handle it
    }

    public void PlayBackgroundMusic()
    {
        PlayMusicClip(mainMenuMusic);
    }

    public void PlayMainMenuMusic()
    {
        PlayMusicClip(mainMenuMusic);
    }

    public void PlayLevel1Music()
    {
        PlayMusicClip(level1Music);
    }

    public void PlayLevel2Music()
    {
        PlayMusicClip(level2Music);
    }

    public void PlayLevel3Music()
    {
        PlayMusicClip(level3Music);
    }

    private void PlayMusicClip(AudioClip clip)
    {
        if (clip != null && musicSource != null)
        {
            // If the same clip is already playing, don't restart
            if (musicSource.clip == clip && musicSource.isPlaying)
            {
                return;
            }

            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlayFootstepSound()
    {
        if (footstepSound != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(footstepSound, sfxVolume * 0.5f);
        }
    }

    public void PlayAttractSound()
    {
        if (attractSound != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(0.95f, 1.05f);
            sfxSource.PlayOneShot(attractSound, sfxVolume);
        }
    }

    public void PlayRepelSound()
    {
        if (repelSound != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(0.95f, 1.05f);
            sfxSource.PlayOneShot(repelSound, sfxVolume);
        }
    }

    public void PlayWinSound()
    {
        if (winSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(winSound, sfxVolume);
        }
    }

    public void PlayDeathSound()
    {
        if (deathSound != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(deathSound, sfxVolume);
        }
    }

    public void PlayPlayerCollisionSound()
    {
        if (playerCollisionSound != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(0.95f, 1.05f);
            sfxSource.PlayOneShot(playerCollisionSound, sfxVolume);
        }
    }

    public void UpdateVolumes()
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        }
        else
        {
            if (musicSource != null) musicSource.volume = musicVolume * masterVolume;
            if (sfxSource != null) sfxSource.volume = sfxVolume * masterVolume;
        }
    }
}
