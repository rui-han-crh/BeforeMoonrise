using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HitBehaviour : MonoBehaviour
{
    [SerializeField]
    private EntityIdentifier entityIdentifier;

    [SerializeField]
    private ScriptableSoundFile hitSound;

    [SerializeField]
    private Animator animator;

    private AudioSource hitAudioSource;

    private IEnumerator KinematicPause(float time)
    {
        animator.speed = 0;
        yield return new WaitForSeconds(time);
        animator.speed = 1;
    }

    private IEnumerator StopAfterSeconds(float time) {
        yield return new WaitForSeconds(time);
        hitAudioSource.Stop();
    }

    private void Start() {
        hitAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EntityIdentifier>() == null)
        {
            return;
        }

        EntityIdentifier otherEntityIdentifier = collision.gameObject.GetComponent<EntityIdentifier>();
        if (otherEntityIdentifier.Type != entityIdentifier.Type)
        {
            StartCoroutine(KinematicPause(0.07f));
            
            hitAudioSource.clip = hitSound.Clip;
            hitAudioSource.volume = hitSound.Volume;
            hitAudioSource.time = hitSound.StartTime;
            hitAudioSource.Play();
            StartCoroutine(StopAfterSeconds(hitSound.EndTime));

            EntityVitals otherEntityVitals = collision.gameObject.GetComponent<EntityVitals>();
            otherEntityVitals.CurrentHealth -= 10;
        }
    }
}
