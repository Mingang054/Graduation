using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource; //효과음
    [SerializeField] private AudioSource voiceSource;   //UI효과음
    [SerializeField] private AudioSource uiSource;

    [SerializeField] private AudioClip coinClip;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지되게
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayVoice(AudioClip clip)
    {
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void PlayUI(AudioClip clip)
    {
        uiSource.PlayOneShot(clip);
    }

    public void StopBGM() => bgmSource.Stop();
    public void StopVoice() => voiceSource.Stop();

    public void PlayCoin()
    {
        PlaySFX(coinClip);
    }
}
