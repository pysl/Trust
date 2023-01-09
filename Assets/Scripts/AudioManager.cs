using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource; /*
    AudioClip deathSound;
    AudioClip moveSound;
    AudioClip attackSound;
    AudioClip hurtSound; */
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); /*
        deathSound = Resources.Load<AudioClip>("Sounds/death");
        moveSound = Resources.Load<AudioClip>("Sounds/move");
        attackSound = Resources.Load<AudioClip>("Sounds/attack");
        hurtSound = Resources.Load<AudioClip>("Sounds/hurt"); */
    }

    public void playSound(string sound) => audioSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/" + sound));


}
