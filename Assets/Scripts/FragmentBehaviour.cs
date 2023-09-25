using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentBehaviour : MonoBehaviour
{
    [SerializeField] ParticleSystem destructionEffect;
    [SerializeField] AudioClip collide;
    [SerializeField] AudioClip disappear;
    bool isDestroyed = false;

    void OnCollisionEnter2D(Collision2D other)
    {
        GetComponent<AudioSource>().PlayOneShot(collide);
        destructionEffect.Play();
        GetComponent<AudioSource>().PlayOneShot(disappear);
    }

    public void SetIsDestroyed()
    {
        isDestroyed = true;
    }

    public bool GetIsDestroyed()
    {
        return isDestroyed;
    }
}
