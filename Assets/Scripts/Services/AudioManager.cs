using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip deselectClip;
    [SerializeField] private AudioClip matchClip;
    [SerializeField] private AudioClip noMatchClip;
    [SerializeField] private AudioClip destroyClip;
    [SerializeField] private AudioClip makeGemClip;

    private AudioSource _audioSource;

    private void Awake() 
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick() => PlayRandomPitch(clickClip);
    public void PlayDeselect() => _audioSource.PlayOneShot(deselectClip);
    public void PlayMatch() => _audioSource.PlayOneShot(matchClip);
    public void PlayeNoMatch() => _audioSource.PlayOneShot(noMatchClip);

    void PlayRandomPitch(AudioClip clip)
    {
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(clip);
        _audioSource.pitch = 1f;
    }

}
