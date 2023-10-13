using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField] float destructionDelay = 0.5f;
    int destroyedThisShot = 0;
    Rigidbody2D projectileRB;
    float gunAngle;
    float projectileSpeed;
    ScoreKeeper scoreKeeper;
    GameManager gameManager;
    [SerializeField] float slowMotionMultiplier = 0.1f;

    void OnEnable()
    {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        projectileRB = GetComponent<Rigidbody2D>();
        destroyedThisShot = 0;
        // Angle that gun is pointing relative to straight downwards.
        gunAngle = Mathf.Sign(transform.rotation.z) * 2 * Mathf.Acos(transform.rotation.w);
        projectileSpeed = FindObjectOfType<GunController>().GetProjectileSpeed();
        projectileRB.velocity = new Vector2(projectileSpeed * Mathf.Sin(gunAngle), -projectileSpeed * Mathf.Cos(gunAngle));
    }

    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Fragment")
        {
            FragmentBehaviour fragBehaviour = other.gameObject.GetComponent<FragmentBehaviour>();
            bool isDestroyed = fragBehaviour.GetIsDestroyed();

            if (!isDestroyed)
            {
                fragBehaviour.SetIsDestroyed();
                FindObjectOfType<GunController>().RemoveCollider(other.collider);
                Destroy(other.gameObject, destructionDelay);
                scoreKeeper.UpdateScore();
                destroyedThisShot += 1;
                gameManager.UpdateRemainingFragments();

                if (gameManager.GetRemainingFragments() == 0)
                {
                    Time.timeScale = slowMotionMultiplier;
                }
            }
        }
    }

    public int GetDestroyedThisShot()
    { 
        return destroyedThisShot; 
    }

    public float GetDestructionDelay()
    {
        return destructionDelay;
    }

    public float GetSlowMotionMultipler()
    {
        return slowMotionMultiplier;
    }
}
