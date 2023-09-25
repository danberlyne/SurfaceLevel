using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthBehaviour : MonoBehaviour
{
    DeathBehaviour death;
    GunController gunController;

    void Awake()
    {
        death = FindObjectOfType<DeathBehaviour>();
        gunController = FindObjectOfType<GunController>();
    }

    void OnTriggerEnter2D(Collider2D projectile)
    {
        Destroy(projectile.gameObject);
        death.KillProjectile();
        FindObjectOfType<SliderController>().UpdateEnergy(-gunController.GetEnergyCost());
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        if (projectiles.Length <= 1)
        {
            FindObjectOfType<EffectButtons>().EnableEffectButtons();
        }
    }
}
