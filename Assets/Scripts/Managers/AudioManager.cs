using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource Source;
    AudioClip NormalBg;
    private void Awake()
    {
        NormalBg = Source.clip;
        GameManager.instance.AudioM = this;
    }

    public void ChangeMusic(AudioClip clip)
    {
        Source.Stop();
        Source.clip = clip;
        Source.Play();
    }

    public void RemoveMusic()
    {
        Source.Stop();
        Source.clip = NormalBg;
        Source.Play();
    }
}
