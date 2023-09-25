using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBehaviour : MonoBehaviour
{
    bool isDead = true;

    void OnTriggerEnter2D(Collider2D projectile)
    {
        Destroy(projectile.gameObject);
        KillProjectile();
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        if (projectiles.Length <= 1)
        {
            FindObjectOfType<EffectButtons>().EnableEffectButtons();
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
