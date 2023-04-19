using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSound : MonoBehaviour
{
    public AudioClip music;
    private AudioSource source;
    
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = music;
        source.volume = 0.3f;
        source.Play();
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (source.isPlaying || MyLauncher.isConnect())
            {
                source.Stop();
            }
            else
            {
                source.Play();
            }
        }
    }

}
