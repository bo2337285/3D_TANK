using System;
using UnityEngine;

public class AudioEventArgs : EventArgs {
    public AudioClip _audioClip;

    public AudioEventArgs (AudioClip audioClip) {
        _audioClip = audioClip;
    }
}