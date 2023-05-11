using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private IEnumerator currentAudioCountdown;

    public void Start()
    {

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayAudio(ScriptableSoundFile soundFile)
    {
        if (currentAudioCountdown != null)
        {
            StopCoroutine(currentAudioCountdown);
        }
        audioSource.Stop();

        IEnumerator StopAudioAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            audioSource.Stop();
            
            if (soundFile.NextSoundFile != null)
            {
                PlayAudio(soundFile.NextSoundFile);
            }
        }

        audioSource.loop = soundFile.IsLooping;
        audioSource.clip = soundFile.Clip;
        audioSource.time = soundFile.StartTime;
        audioSource.volume = soundFile.Volume;
        audioSource.Play();
        
        if (!soundFile.IsLooping)
        {
            currentAudioCountdown = StopAudioAfterTime(soundFile.EndTime - soundFile.StartTime);
            StartCoroutine(currentAudioCountdown);
        }
    }

    public void StopCurrentAudio()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }

    public void PlayCurrentLoaded()
    {
        audioSource.Play();
    }
}
