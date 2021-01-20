using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class AudioController : MonoBehaviour
{
    private static AudioController Instance;

    public List<SingleAudio> bgms = new List<SingleAudio>();
    public List<SingleAudio> sfxs = new List<SingleAudio>();

    private Dictionary<string, SingleAudio> bgmsDict = new Dictionary<string, SingleAudio>();
    private Dictionary<string, SingleAudio> sfxsDict = new Dictionary<string, SingleAudio>();

    private AudioSource bgmAS, sfxAS;

    private void Awake()
    {
        Instance = this;

        bgmAS = gameObject.AddComponent<AudioSource>();
        sfxAS = gameObject.AddComponent<AudioSource>();

        bgmAS.playOnAwake = false;
        sfxAS.playOnAwake = false;

        for (int i = 0; i < bgms.Count; i++)
        {
            bgmsDict.Add(bgms[i].name, bgms[i]);
        }

        for (int i = 0; i < sfxs.Count; i++)
        {
            sfxsDict.Add(sfxs[i].name, sfxs[i]);
        }

        if(bgms.Count > 0)
        {
            PlayBGM(bgms[0]);
        }
    }

    private void M_PlayBGM(string name)
    {
        if(bgmsDict.TryGetValue(name, out SingleAudio value))
        {
            PlayBGM(value);
        }
        else
        {
            Debug.LogError("No BGM with this name: " + name);
        }
    }

    public static void PlayBGM(string name)
    {
        Instance?.M_PlayBGM(name);
    }

    private void M_PlaySfx(string name)
    {
        if (sfxsDict.TryGetValue(name, out SingleAudio value))
        {
            PlaySfx(value);
        }
        else
        {
            Debug.LogError("No sfx with this name: " + name);
        }
    }
    
    public static void PlaySfx(string name)
    {
        Instance?.M_PlaySfx(name);
    }

    private void PlaySfx(SingleAudio singleAudio)
    {
        sfxAS.pitch = singleAudio.pitch;
        sfxAS.loop = singleAudio.loop;

        sfxAS.PlayOneShot(singleAudio.clip, singleAudio.volume);
    }

    private void PlayBGM(SingleAudio singleAudio)
    {
        StopAllCoroutines();
        StartCoroutine(BGMTransition(singleAudio));
    }

    IEnumerator BGMTransition(SingleAudio singleAudio)
    {
        if(bgmAS.clip != null)
        {
            while(bgmAS.volume > 0.01f)
            {
                bgmAS.volume -= Time.deltaTime;
                yield return null;
            }
        }

        bgmAS.volume = 0f;
        bgmAS.clip = singleAudio.clip;
        bgmAS.Play();

        while (bgmAS.volume < singleAudio.volume)
        {
            bgmAS.volume += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }
}

[System.Serializable]
public class SingleAudio
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
}