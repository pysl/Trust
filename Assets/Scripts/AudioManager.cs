using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void playSound(string sound) => audioSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/" + sound));


}
