using UnityEngine;

public class RandomVoiceLinePlayer : MonoBehaviour
{
    [Header("Voice Line Settings")]
    public AudioClip[] voiceLines; // Array of audio clips to randomly choose from
    public float minInterval = 3f; // Minimum time between voice lines
    public float maxInterval = 10f; // Maximum time between voice lines

    private AudioSource audioSource; // Reference to the AudioSource component
    private float nextPlayTime; // Time when the next voice line should play

    private void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on this GameObject.");
            return;
        }

        // Schedule the first voice line
        ScheduleNextVoiceLine();
    }

    private void Update()
    {
        // Check if it's time to play the next voice line
        if (Time.time >= nextPlayTime && voiceLines.Length > 0)
        {
            PlayRandomVoiceLine();
            ScheduleNextVoiceLine();
        }
    }

    private void PlayRandomVoiceLine()
    {
        // Select a random clip from the array
        AudioClip clip = voiceLines[Random.Range(0, voiceLines.Length)];

        // Play the clip
        audioSource.PlayOneShot(clip);
    }

    private void ScheduleNextVoiceLine()
    {
        // Schedule the next voice line with a random interval
        nextPlayTime = Time.time + Random.Range(minInterval, maxInterval);
    }
}