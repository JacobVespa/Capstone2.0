using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private GameObject bgmSourcePrefab;

    private GameObject bgmRoot;
    private Dictionary<string, AudioSource> bgmTracks = new Dictionary<string, AudioSource>();
    private AudioSource currentTrack;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SpawnBGMRoot();
    }

    private void SpawnBGMRoot()
    {
        if (bgmSourcePrefab == null)
        {
            Debug.LogError("SoundManager: No BGM Source Prefab assigned!");
            return;
        }

        bgmRoot = Instantiate(bgmSourcePrefab, transform);
        DontDestroyOnLoad(bgmRoot);

        CacheTracks();
    }

    private void CacheTracks()
    {
        bgmTracks.Clear();

        AudioSource[] sources = bgmRoot.GetComponentsInChildren<AudioSource>(true);

        foreach (var source in sources)
        {
            bgmTracks[source.gameObject.name] = source;
        }
    }

    public void PlayBGM(string trackName)
    {
        if (!bgmTracks.TryGetValue(trackName, out AudioSource newTrack))
        {
            Debug.LogWarning($"BGM track not found: {trackName}");
            return;
        }

        // Stop current track if one is playing
        if (currentTrack != null)
        {
            currentTrack.Stop();
        }

        currentTrack = newTrack;
        currentTrack.time = 0f;   // restart from beginning
        currentTrack.Play();
    }

    public void StopBGM()
    {
        if (currentTrack != null)
        {
            currentTrack.Stop();
            currentTrack = null;
        }
    }
}