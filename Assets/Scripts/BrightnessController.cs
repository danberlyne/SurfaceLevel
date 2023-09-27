using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] float baseBrightness = 1.0f;
    [SerializeField] float brightnessSensitivity = 1.0f;
    LoudnessExtractor loudnessExtractor;
    Light2D lightSource;

    void Start()
    {
        loudnessExtractor = FindObjectOfType<LoudnessExtractor>();
        lightSource = GetComponent<Light2D>();
    }

    void Update()
    {
        lightSource.intensity = baseBrightness + loudnessExtractor.GetClipLoudness() * brightnessSensitivity;
    }
}
