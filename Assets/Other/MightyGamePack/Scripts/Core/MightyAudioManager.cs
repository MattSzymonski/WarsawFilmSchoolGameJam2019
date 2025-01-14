﻿/* 
 * Add sounds in inspector.
 * If you want to play some sound just use: audioManager.PlaySound("SoundName");
 * That's it.
 * 
 * Dependencies: https://github.com/dbrizov/NaughtyAttributes
*/


using UnityEngine.Audio;
using UnityEngine;
using System;

using NaughtyAttributes;

namespace MightyGamePack
{
    public enum SoundType
    {
        Effect,
        Music
    };

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1.0f;
        [Range(0.1f, 3f)]
        public float pitch = 1.0f;

        public bool playOnAwake;
        public bool loop;


        public SoundType soundType;

        [HideInInspector]
        public AudioSource source;
    }

    public class MightyAudioManager : MonoBehaviour
    { 
        [ReorderableList]
        public Sound[] sounds;

        MightyGameManager gameManager;

        [HideInInspector]
        public static MightyAudioManager instance;

        public bool muteSounds;
        [Space(10)]
        public AudioMixerGroup musicMixerGroup;
        public AudioMixerGroup effectsMixerGroup;

        void Awake()
        {
            if (instance == null) //Make AudioManager a singleton
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            //DontDestroyOnLoad(gameObject);

            if (musicMixerGroup == null || effectsMixerGroup == null)
            {
                Debug.LogError("Sound mixer groups not set");
            }

            foreach (Sound sound in sounds)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
                sound.source.playOnAwake = sound.playOnAwake;

                if (sound.soundType == SoundType.Effect)
                {
                    sound.source.outputAudioMixerGroup = effectsMixerGroup;
                }
                if (sound.soundType == SoundType.Music)
                {
                    sound.source.outputAudioMixerGroup = musicMixerGroup;
                }
                if (sound.playOnAwake && !muteSounds)
                {
                    sound.source.Play();
                }
            }
        }

        public void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<MightyGameManager>();
        }

        public void PlaySound(string soundName)
        {
            if (!muteSounds)
            {
                Sound sound = Array.Find(sounds, soundFind => soundFind.name == soundName);
                if (sound == null)
                {
                    Debug.LogWarning("Sound: " + soundName + " not found!");
                    return;
                }
                sound.source.Play();
            }    
        }

        public void StopSound(string soundName)
        {

            Sound sound = Array.Find(sounds, soundFind => soundFind.name == soundName);
            if (sound == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }
            sound.source.Stop();
        }

        public bool IsSoundPlaying(string soundName)
        {
            Sound sound = Array.Find(sounds, soundFind => soundFind.name == soundName);
            if (sound == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return false; //not sure if correct
            }
            return sound.source.isPlaying;
        }

        public void PlayRandomSound(params string[] soundNames)
        {
            if (!muteSounds)
            {
                string soundName = soundNames[UnityEngine.Random.Range(0, soundNames.Length)];
                Sound sound = Array.Find(sounds, soundFind => soundFind.name == soundName);
                if (sound == null)
                {
                    Debug.LogWarning("Randomized sound: " + soundName + " not found!");
                    return;
                }
                sound.source.Play();
            }
        }

        public void SetMusicMixerVolume(float sliderValue)
        {
            musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        }

        public void SetEffectsMixerVolume(float sliderValue)
        {
            effectsMixerGroup.audioMixer.SetFloat("EffectsVolume", Mathf.Log10(sliderValue) * 20);
        }
    }
}
