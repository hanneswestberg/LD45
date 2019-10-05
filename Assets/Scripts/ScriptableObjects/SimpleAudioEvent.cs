using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent {
    public AudioClip[] clips;

    public RangedFloat volume;

    [MinMaxRange(0, 2)]
    public RangedFloat pitch;

    private AudioClip lastPlayedClip;

    public override void Play(AudioSource source) {
        if (clips.Length == 0)
            return;

        source.clip = ChooseClip();
        lastPlayedClip = source.clip;
        source.volume = Random.Range(volume.minValue, volume.maxValue);
        source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
        source.Play();
    }

    public AudioClip ChooseClip() {
        if (clips.Length > 1) {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            return (clip == lastPlayedClip) ? ChooseClip() : clip;
        } else {
            return clips[Random.Range(0, clips.Length)];
        }
    }
}

