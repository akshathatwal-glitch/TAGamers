using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Range(0f, 1f)]
    public float volume = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton – only one MusicManager ever exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // survive scene changes

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
    }

    // Call these from anywhere: MusicManager.Instance.Pause();
    public void Pause()  => audioSource.Pause();
    public void Resume() => audioSource.UnPause();

    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        audioSource.volume = volume;
    }
}