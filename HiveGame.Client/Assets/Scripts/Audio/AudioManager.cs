using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip clip;

    private AudioSource source;

    void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 0f;
        source.playOnAwake = false;

    }

    private void OnEnable()
    {
        ServiceLocator.Services.EventAggregator.PlaySound += Play;
    }

    private void OnDisable()
    {
        ServiceLocator.Services.EventAggregator.PlaySound -= Play;
    }

    public void Play()
    {
        source.Play();
    }
}