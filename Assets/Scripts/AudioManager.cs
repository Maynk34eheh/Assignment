using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip backgroundMusicClip;
    public AudioClip reelSpinClip;
    public AudioClip winClip;
    public AudioClip coinRainClip;
    public AudioClip buttonClickClip;

    [Header("Audio Sources")]
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource reelSource;
    private AudioSource coinRainSource;

    private static AudioManager instance;
    private bool musicPlaying = false;

    private void Awake()
    {
        // Make sure only one AudioManager exists at a time
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Find all the audio sources attached as children
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();

        foreach (AudioSource source in sources)
        {
            if (source.gameObject.name == "MusicSource")
                musicSource = source;
            else if (source.gameObject.name == "SFXSource")
                sfxSource = source;
            else if (source.gameObject.name == "ReelSource")
                reelSource = source;
            else if (source.gameObject.name == "CoinRainSource")
                coinRainSource = source;
        }

        // If children aren't named, just grab them in order
        if (sources.Length >= 1 && musicSource == null) musicSource = sources[0];
        if (sources.Length >= 2 && sfxSource == null) sfxSource = sources[1];
        if (sources.Length >= 3 && reelSource == null) reelSource = sources[2];
        if (sources.Length >= 4 && coinRainSource == null) coinRainSource = sources[3];

        if (musicSource == null) Debug.LogError("AudioManager: MusicSource not found!");
        if (sfxSource == null) Debug.LogError("AudioManager: SFXSource not found!");
        if (reelSource == null) Debug.LogError("AudioManager: ReelSource not found!");
        if (coinRainSource == null) Debug.LogError("AudioManager: CoinRainSource not found!");
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusicClip == null) return;

        if (!musicPlaying)
        {
            musicSource.clip = backgroundMusicClip;
            musicSource.loop = true;
            musicSource.Play();
            musicPlaying = true;
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            musicPlaying = false;
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = Mathf.Clamp01(volume);
    }

    public void PlayReelSpinSound()
    {
        if (reelSource == null || reelSpinClip == null) return;

        reelSource.clip = reelSpinClip;
        reelSource.loop = true;
        reelSource.Play();
    }

    public void StopReelSpinSound()
    {
        if (reelSource != null)
            reelSource.Stop();
    }

    // Changes how fast the reel sound plays (1.0 = normal speed)
    public void SetReelPitch(float pitch)
    {
        if (reelSource != null)
            reelSource.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
    }

    public void ResetReelPitch()
    {
        if (reelSource != null)
            reelSource.pitch = 1.0f;
    }

    public void SetReelVolumeWhileSpinning(float speed)
    {
        if (reelSource != null)
            reelSource.volume = Mathf.Clamp01(speed / 800f);
    }

    public void PlayWinSound()
    {
        if (sfxSource == null || winClip == null) return;

        sfxSource.clip = winClip;
        sfxSource.loop = false;
        sfxSource.Play();
    }

    public void PlayCoinRainSound()
    {
        if (coinRainSource == null || coinRainClip == null) return;

        coinRainSource.clip = coinRainClip;
        coinRainSource.loop = false;
        coinRainSource.Play();
    }

    public void StopCoinRainSound()
    {
        if (coinRainSource != null)
            coinRainSource.Stop();
    }

    public void PlayButtonClickSound()
    {
        if (sfxSource == null || buttonClickClip == null) return;

        sfxSource.clip = buttonClickClip;
        sfxSource.loop = false;
        sfxSource.Play();
    }

    public static AudioManager Instance
    {
        get { return instance; }
    }
}
