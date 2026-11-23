using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Musica")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Efectos de Fuego")]
    public AudioClip flamethrowerLoop;
    public AudioClip fireballLaunch;
    public AudioClip fireballImpact;
    public AudioClip infiniteFuel;

    [Header("Efectos de Agua")]
    public AudioClip waterStreamLoop;
    public AudioClip waterBombLaunch;
    public AudioClip waterBombImpact;
    public AudioClip waterDive;

    [Header("Combate")]
    public AudioClip damageHit;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private List<AudioSource> activeLoops = new List<AudioSource>();

    public float MusicVolume { get; private set; } = 0.7f;
    public float SFXVolume { get; private set; } = 1f;
    public bool IsMusicMuted { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
            LoadAudioSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    private void SetupAudioSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }

    private void LoadAudioSettings()
    {
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        IsMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;

        musicSource.volume = MusicVolume;
        musicSource.mute = IsMusicMuted;
        sfxSource.volume = SFXVolume;
    }

    // --- CONTROLES DEL MENU DE CONFIGURACION ---

    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
        musicSource.volume = MusicVolume;
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp01(volume);
        sfxSource.volume = SFXVolume;

        for (int i = activeLoops.Count - 1; i >= 0; i--)
        {
            if (activeLoops[i] != null) activeLoops[i].volume = SFXVolume;
            else activeLoops.RemoveAt(i);
        }

        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicMuted(bool muted)
    {
        IsMusicMuted = muted;
        musicSource.mute = IsMusicMuted;
        PlayerPrefs.SetInt("MusicMuted", IsMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // --- MÚSICA ---

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayGameMusic() => PlayMusic(gameMusic);

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // --- EFECTOS PUNTUALES ---

    private void PlaySFX(AudioClip clip, bool randomPitch = true)
    {
        if (clip == null || SFXVolume <= 0f) return;

        sfxSource.pitch = randomPitch ? Random.Range(0.94f, 1.06f) : 1f;
        sfxSource.PlayOneShot(clip, SFXVolume);
    }

    public void PlayFireballLaunch() => PlaySFX(fireballLaunch);
    public void PlayFireballImpact() => PlaySFX(fireballImpact);
    public void PlayInfiniteFuel() => PlaySFX(infiniteFuel, false);

    public void PlayWaterBombLaunch() => PlaySFX(waterBombLaunch);
    public void PlayWaterBombImpact() => PlaySFX(waterBombImpact);
    public void PlayWaterDive() => PlaySFX(waterDive, false);

    public void PlayDamageHit() => PlaySFX(damageHit);

    // --- EFECTOS CONTINUOS EN BUCLE ---

    public void StartFlamethrower(AudioSource source) => StartLoop(source, flamethrowerLoop);
    public void StartWaterStream(AudioSource source) => StartLoop(source, waterStreamLoop);

    private void StartLoop(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;

        if (!activeLoops.Contains(source)) activeLoops.Add(source);

        source.clip = clip;
        source.loop = true;
        source.volume = SFXVolume;

        if (!source.isPlaying) source.Play();
    }

    public void StopLoop(AudioSource source)
    {
        if (source != null && source.isPlaying) source.Stop();
    }
}