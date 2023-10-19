using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBehaviour : MonoBehaviour
{
    bool isDead = true;

    void OnTriggerEnter2D(Collider2D projectile)
    {
        Destroy(projectile.gameObject);
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        if (projectiles.Length <= 1)
        {
            EffectButtons effectButtons = FindObjectOfType<EffectButtons>();
            effectButtons.EnableEffectButtons();
            effectButtons.ToggleAllOff();
            KillProjectile();
            ParticleSystem destructionEffect = GetComponentInChildren<ParticleSystem>();
            destructionEffect.transform.position = projectile.transform.position;
            destructionEffect.Play();
            GetComponent<AudioSource>().Play();
        }
        FindObjectOfType<GameManager>().CheckForLevelEnd();
    }

    public void BirthProjectile()
    {
        isDead = false;
    }

    public void KillProjectile()
    {
        isDead = true;
    }

    public bool GetIsDead()
    {
        return isDead;
    }
}
