using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : SingletonMonoBehaviour<AudioManager> {

    private const string BGM_PATH = "Audio/BGM";
    private const string SE_PATH  = "Audio/SE";
    private const string AUDIO_MIXER_PATH = "Audio/AudioMixer";

    private AudioSource nowPlayingBGM;
    private Dictionary<string, AudioSource> seDictionary  = new Dictionary<string, AudioSource>();
    private Dictionary<string, AudioSource> bgmDictionary = new Dictionary<string, AudioSource>();
    private Dictionary<string, AudioMixerGroup> mixerDictionary = new Dictionary<string, AudioMixerGroup>();
    private AudioMixer audioMixer;

    public void Awake() {
        if(this != Instance) {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        Init();
    }

    private void Init () {
        object[] bgmList = Resources.LoadAll(BGM_PATH);
        object[] seList  = Resources.LoadAll(SE_PATH);
        audioMixer = (AudioMixer) Resources.Load(AUDIO_MIXER_PATH);

        AudioMixerGroup[] audioMasterGroups = audioMixer.FindMatchingGroups("Master");
        foreach (AudioMixerGroup mixer in audioMasterGroups) {
            mixerDictionary.Add(mixer.name, mixer);
        }

        // XXX 繰り返しているのメソッド化してまとめる
        foreach (AudioClip bgm in bgmList) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip        = bgm;
            bgmDictionary.Add(bgm.name, audioSource);

            if (mixerDictionary.ContainsKey(bgm.name)) {
                audioSource.outputAudioMixerGroup = mixerDictionary[bgm.name];
            }
            else if (mixerDictionary.ContainsKey("BGM")) {
                audioSource.outputAudioMixerGroup = mixerDictionary["BGM"];
            }
            else {
                audioSource.outputAudioMixerGroup = mixerDictionary["Master"];
            }
        }

        foreach (AudioClip se in seList) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip        = se;
            seDictionary.Add(se.name, audioSource);

             if (mixerDictionary.ContainsKey(se.name)) {
                audioSource.outputAudioMixerGroup = mixerDictionary[se.name];
            }
            else if (mixerDictionary.ContainsKey("SE")) {
                audioSource.outputAudioMixerGroup = mixerDictionary["SE"];
            }
            else {
                audioSource.outputAudioMixerGroup = mixerDictionary["Master"];
            }
        }
    }

    public void PlayOneShotSE (string name) {
        if (seDictionary.ContainsKey(name)) {
            AudioSource audioSource = seDictionary[name];
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    public void PlayBGM (string name) {
        StopBGM();
        AudioSource audioSource = bgmDictionary[name];
        audioSource.Play();
        nowPlayingBGM = audioSource;
    }

    public void StopBGM () {
        if (nowPlayingBGM) {
            nowPlayingBGM.Stop();
        }
    }
}
