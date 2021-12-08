using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;                   //Drag a reference to the audio source which will play the sound effects.

    //POINTS
    public AudioClip point1;
    public AudioClip spit;

    public AudioClip vaccum;
    public AudioClip shootObject;


    //MAIN THEME
    public AudioClip MainTheme1;



    //Miscellaneous

    public AudioClip getObject;

    //clock
    public AudioClip clock;

    public AudioSource musicSource;                 //Drag a reference to the audio source which will play the music.
    public static SoundManager instance = null;     //Allows other scripts to call functions from SoundManager.             
    public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.



    void Start()
    {
        PlayMusic(MainTheme1);
    }

    private void Update()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.clip = MainTheme1;
            musicSource.Play();
        }
    }

    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }


    /// <summary>
    /// Plays the single clip
    /// </summary>
    /// <param name="clip">Clip.</param>
    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip, AudioSource src)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        src.clip = clip;

        //Play the clip.
        src.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        musicSource.clip = clip;

        //Play the clip.
        musicSource.Play();

    }


    /// <summary>
    /// Randomizes the sfx.
    /// </summary>
    /// <param name="clips">Clips.</param>
    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx(params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        efxSource.clip = clips[randomIndex];

        //Play the clip.
        efxSource.Play();
    }


    public void Clock(AudioSource audioSrc)
    {
        instance.PlaySingle(clock, audioSrc);
    }

    public void Vacuum(AudioSource audioSrc)
    {
        instance.PlaySingle(vaccum, audioSrc);
    }

    public void ShootObject(AudioSource audioSrc)
    {
        instance.PlaySingle(shootObject, audioSrc);
    }

    public void Spit(AudioSource audioSrc)
    {
        instance.PlaySingle(spit, audioSrc);
    }

    public void GetObject(AudioSource audioSrc)
    {
        instance.PlaySingle(getObject, audioSrc);
    }



}