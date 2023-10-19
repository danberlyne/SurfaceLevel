using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField] float destructionDelay = 0.5f;
    int destroyedThisShot = 0;
    Rigidbody2D projectileRB;
    float gunAngle;
    float projectileSpeed;
    GameManager gameManager;
    [SerializeField] float slowMotionMultiplier = 0.1f;

    [Header("Scoring")]
    ScoreKeeper scoreKeeper;
    int multihitBonus = 0;
    [SerializeField] GameObject scoreAmountThisHit;
    [SerializeField] float scoreFlashDuration = 1f;
    [SerializeField] Vector2 scoreFlashOffset = new Vector3 (0, 0.1f);

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
                multihitBonus = 1 << destroyedThisShot;
                StartCoroutine(FlashScore(multihitBonus));

                destroyedThisShot += 1;
                gameManager.UpdateRemainingFragments();

                if (gameManager.GetRemainingFragments() == 0)
                {
                    Time.timeScale = slowMotionMultiplier;
                }
            }
        }
    }

    IEnumerator FlashScore(int scoreThisHit)
    {
        RectTransform canvasRect = scoreAmountThisHit.transform.parent.GetComponent<RectTransform>();
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 projectileScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        scoreAmountThisHit.GetComponent<RectTransform>().anchoredPosition = projectileScreenPosition + scoreFlashOffset;

        scoreAmountThisHit.SetActive(true);
        TextMeshProUGUI scoreTextThisHit = scoreAmountThisHit.GetComponent<TextMeshProUGUI>();
        scoreTextThisHit.text = scoreThisHit.ToString();
        yield return new WaitForSeconds(scoreFlashDuration);
        scoreAmountThisHit.SetActive(false);
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
