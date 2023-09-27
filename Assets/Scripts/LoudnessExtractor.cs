using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoudnessExtractor : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float updateStep = 0.1f;
    [SerializeField] int sampleDataLength = 1024;
    float currentUpdateTime = 0f;
    float clipLoudness;
    float[] clipSampleData;

    void Awake()
    {
        if (!audioSource)
        {
            Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
        }
        clipSampleData = new float[sampleDataLength];
    }

    void Update()
    {
        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData, audioSource.timeSamples);
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength;
        }

    }

    public float GetClipLoudness()
    {
        return clipLoudness;
    }
}
