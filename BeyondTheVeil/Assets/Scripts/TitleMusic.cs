using UnityEngine;
using System.Collections;

//ATTENTION!! THIS ENTIRE SCRIPT WAS WRITTEN BY ME. MORGAN HARRY BENNETT. I AM NOT 
//A GAME DEVELOPER SO THIS IS PROBABLY SHIT. SORRY IN ADVANCE.

public class SceneMusic : MonoBehaviour
{
    // Make them editable in the object thingy
    [SerializeField] private float fadeInTime = 1.5f;
    [SerializeField] private float fadeOutTime = 1.0f;

    // Variables
    private AudioSource source;
    private Coroutine fadeRoutine;

    // This is basically a constant 
    public float FadeOutTime => fadeOutTime;


    // Attach the music file to the 'source' variable 
    private void Awake()
    {
        source = GetComponent<AudioSource>();

            // Set the track to loop
            source.loop = true;
    }

    /// <summary>
    /// For fading into a track
    /// </summary>
    void Start()
    {
        // Start the volume at 0
        source.volume = 0f;
        // Start the track
        source.Play();
        // Beigin the fade
        FadeTo(1f, fadeInTime);
    }

    // Fade Out
    public void FadeOut()
    {
        FadeTo(0f, FadeOutTime, stopAfter: true);
    }

    // Fade In
    public void FadeTo(float targetVolume, float time, bool stopAfter = false)
    {
        // Only run if it is active
        if (fadeRoutine != null)
        {
            // Stop it 
            StopCoroutine(fadeRoutine);
        }
        // Start the fade 
        fadeRoutine = StartCoroutine(FadeRoutine(targetVolume,time, stopAfter));
    }


    /// <summary>
    /// Controls the logic of fading
    /// </summary>
    /// <param name="target"></param>
    /// <param name="time"></param>
    /// <param name="stopAfter"></param>
    /// <returns></returns>
    private IEnumerator FadeRoutine(float target, float time, bool stopAfter)
    {
        // Variables 
        float start = source.volume;
        float trackTime = 0f;

        // While the beginning time is less than the time passed in 
        while (trackTime < time)
        {
            // Increment the time 
            trackTime += Time.deltaTime;
            // Change the volume
            // Start - The volume at the beginning of the fade
            // Target - The end target volume ( 1 for FadeIn, 0 for FadeOut )
            // ( trackTime / Time ) progress of the fade from beginning to end
            source.volume = Mathf.Lerp(start, target, trackTime / time);
            yield return null;
        }
        // After the fade has completed, set the final volume
        source.volume = target;

        // Fading to silence?
        if (stopAfter && target <= 0.001f)
        {
            // Stop the music 
            source.Stop();
        }
        // No more fade to run
        fadeRoutine = null;
    }
}