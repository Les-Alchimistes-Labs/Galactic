using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    private AudioSource source;

    public static bool attacksound;
    public static bool healsound;
    public static bool chanGunsound;

    public AudioClip attack;
    public AudioClip heal;
    public AudioClip changeGun;


    private void Start()
    {
        source = GetComponent<AudioSource>();
        attacksound = false;
        healsound = false;
        chanGunsound = false;
    }

    private void Update()
    {
        if (attacksound)
        {
            source.PlayOneShot(attack);
            attacksound = false;
        }

        if (healsound)
        {
            source.PlayOneShot(heal);
            healsound = false;
        }

        if (chanGunsound)
        {
            source.PlayOneShot(changeGun);
            chanGunsound = false;
        }
    }
}
