using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/* 
Note, transform.rotation is a quaternion (x,y,z,w) where (x,y,z) determines the axis of rotation and w is the rotation amount.
x = v1 * sin (theta / 2)
y = v2 * sin (theta / 2)
z = v3 * sin (theta / 2)
w = cos (theta / 2) 
where (v1, v2, v3) is the rotation axis as a vector and theta is the rotation angle.
*/

public class GunController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] float startDelay = 4.0f;
    bool disabled = false;
    Vector2 turnInput;
    [SerializeField] float turnSensitivity = -200f;
    // Quaternion w-value for a right angle. Note, the w-value does not encode the direction of rotation, but the x, y, z values do.
    static readonly float rightAngle = Mathf.Sqrt(2) / 2;
    // Determines how much the rotation range of the gun is restricted.
    [SerializeField] float angleRestriction = 0.1f;
    // Keeps track of whether sound effect is playing.
    bool isPlaying = false;
    SliderController sliderController;
    bool hasSufficientEnergy = true;
    [SerializeField] AudioSource firingSFX;

    [Header("Projectiles")]
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] int energyCost = 10;
    DeathBehaviour death;
    [SerializeField] AudioClip selfDestruction;
    GameObject[] activeProjectiles;

    [Header("Trajectory")]
    [SerializeField] LineRenderer[] lineRenderers = new LineRenderer[10];
    IEnumerable<List<Vector2>> path;
    [SerializeField] float trajectoryLength = 1f;
    List<Collider2D> colliders;
    bool hasCollided;
    GradientColorKey[] colours = new GradientColorKey[2];
    GradientAlphaKey[] alphas = new GradientAlphaKey[2];

    void Awake()
    {
        death = FindObjectOfType<DeathBehaviour>();
    }

    void Start()
    {
        StartCoroutine(StartUp());
    }

    void OnEnable()
    {
        colliders = FindObjectsOfType<Collider2D>().ToList();
    }

    IEnumerator StartUp()
    {
        disabled = true;
        yield return new WaitForSeconds(startDelay);
        disabled = false;
    }

    void Update()
    {
        if (disabled)
        {
            return;
        }
        else
        {
            float turnAmount = turnInput.x * turnSensitivity * Time.deltaTime;
            TurnTurret(turnAmount);
            UpdateTurnSFX(turnAmount);

            if (!death.GetIsDead())
            {
                HideTrajectory();
            }
            else
            {
                DisplayTrajectory();
            }
        }
    }

    void TurnTurret(float turnAmount)
    {
        // 'true' if turnAmount is decreasing the absolute value of the current angle of rotation.
        bool isOppositeDirection = Mathf.Sign(transform.rotation.z) != Mathf.Sign(turnAmount);
        if (transform.rotation.w > rightAngle + angleRestriction || isOppositeDirection)
        {
            transform.Rotate(0, 0, turnAmount);
        }
    }

    void UpdateTurnSFX(float turnAmount)
    {
        if (turnAmount != 0 && !isPlaying)
        {
            GetComponent<AudioSource>().Play();
            isPlaying = true;
        }
        else if (turnAmount == 0 && isPlaying)
        {
            GetComponent<AudioSource>().Stop();
            isPlaying = false;
        }
    }

    void HideTrajectory()
    {
        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            lineRenderer.enabled = false;
        }
    }

    void DisplayTrajectory()
    {
        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            lineRenderer.enabled = true;
        }

        path = SimulateTrajectory();
        RenderTrajectory(path);
    }

    IEnumerable<List<Vector2>> SimulateTrajectory()
    {
        float gunAngle = Mathf.Sign(transform.rotation.z) * 2 * Mathf.Acos(transform.rotation.w);
        Vector2 position = projectileSpawn.position;
        Vector2 nextPosition;
        Vector2 velocity = new Vector2(projectileSpeed * Mathf.Sin(gunAngle), -projectileSpeed * Mathf.Cos(gunAngle));
        Vector2 nextVelocity;
        List<Vector2> pathSegment = new List<Vector2>();
        int numSegments = 1;

        float duration = trajectoryLength;
        float timestep = Time.fixedDeltaTime;
        for (float t = 0f; t < duration; t += timestep)
        {
            hasCollided = false;

            foreach (Collider2D collider in colliders)
            {
                nextVelocity = velocity + (Vector2)Physics.gravity * timestep;
                nextVelocity = nextVelocity * (1 - timestep * projectile.GetComponent<Rigidbody2D>().drag);
                nextPosition = position + nextVelocity * timestep;

                if (collider.ClosestPoint(nextPosition) == nextPosition // If position is inside a collider
                    && collider.isTrigger                               // and collider is a trigger
                    && collider.gameObject.GetComponent<Teleporter>())  // and collider is a teleporter, teleport the trajectory.
                {
                    Teleporter teleporter = collider.gameObject.GetComponent<Teleporter>();
                    Teleporter otherTeleporter = teleporter.GetOtherTeleporter();
                    Vector2 collisionPoint = collider.ClosestPoint(nextPosition);
                    Vector2 boundaryAnchor = teleporter.GetBoundaryAnchor();
                    Vector2 pointOfContactRelBoundary = collisionPoint - boundaryAnchor;
                    Vector2 normalVector = teleporter.GetNormalVector();

                    if (Vector2.Angle(normalVector, velocity) > 90)
                    {
                        Vector2 newContactRelBoundary = Teleporter.Rotate(pointOfContactRelBoundary,
                                                                          Vector2.SignedAngle(-normalVector, otherTeleporter.GetNormalVector()));
                        position = otherTeleporter.GetBoundaryAnchor() + newContactRelBoundary + otherTeleporter.GetNormalVector() * teleporter.GetTeleportationOffset();
                        velocity = Teleporter.Rotate(velocity, Vector2.SignedAngle(-normalVector, otherTeleporter.GetNormalVector()));
                        hasCollided = true;

                        yield return pathSegment; // End this path segment and start a new one at the teleported location.

                        if (numSegments >= lineRenderers.Length) // If we have reached the maximum number of segments, terminate the path entirely.
                        {
                            yield break;
                        }
                        else
                        {
                            numSegments++;
                            pathSegment = new List<Vector2>();
                            break;
                        }
                    }
                }
                else if (collider.ClosestPoint(position) == position) // If position is inside a non-teleporter collider, end the path.
                {
                    yield return pathSegment;
                    yield break;
                }
            }

            if (!hasCollided)
            {
                velocity += (Vector2)Physics.gravity * timestep;
                velocity = velocity * (1 - timestep * projectile.GetComponent<Rigidbody2D>().drag);
                position += velocity * timestep;
            }

            pathSegment.Add(position);
        }

        yield return pathSegment;
    }

    void RenderTrajectory(IEnumerable<List<Vector2>> path)
    {
        foreach (LineRenderer lr in lineRenderers)
        {
            lr.positionCount = 0;
        }

        List<Vector3> path3;
        LineRenderer lineRenderer;
        Gradient lineColourGradient;
        int segmentIndex = 0;
        float pathLength = trajectoryLength / Time.fixedDeltaTime;

        foreach (List<Vector2> pathSegment in path)
        {
            lineRenderer = lineRenderers[segmentIndex];
            path3 = new List<Vector3>();

            foreach (Vector3 v in pathSegment)
            {
                path3.Add(v);
            }

            lineRenderer.positionCount = path3.Count;
            lineRenderer.SetPositions(path3.ToArray());

            lineColourGradient = lineRenderer.colorGradient;
            colours[0].time = 0;
            colours[0].color = (segmentIndex == 0) ? new Color(196f / 255f, 0f, 196f / 255f) : colours[1].color;
            colours[1].time = 1;
            colours[1].color = new Color(colours[0].color.r - (path3.Count / pathLength) * 132f / 255f,
                                          0f,
                                          colours[0].color.b - (path3.Count / pathLength) * 132f / 255f);
            alphas[0].time = 0;
            alphas[0].alpha = (segmentIndex == 0) ? 1 : alphas[1].alpha;
            alphas[1].time = 1;
            alphas[1].alpha = alphas[0].alpha - path3.Count / pathLength;
            lineColourGradient.SetKeys(colours, alphas);
            lineRenderer.colorGradient = lineColourGradient;

            segmentIndex++;
        }
    }

    void OnTurn(InputValue value)
    {
        turnInput = value.Get<Vector2>();
    }

    void OnFire(InputValue value)
    {
        sliderController = FindObjectOfType<SliderController>();
        CheckEnergy();

        if (!hasSufficientEnergy || !death.GetIsDead() || disabled)
        {
            return;
        }

        if (value.isPressed)
        {
            death.BirthProjectile();
            Instantiate(projectile, projectileSpawn.position, transform.rotation);
            firingSFX.Play();
            sliderController.UpdateEnergy(energyCost);
            FindObjectOfType<EffectButtons>().DisableEffectButtons();
        }
    }

    void CheckEnergy()
    {
        int energyAmount = sliderController.GetRemainingEnergy();
        hasSufficientEnergy = energyAmount >= energyCost;
    }

    void OnKill(InputValue value)
    {
        if (death.GetIsDead() || disabled)
        {
            return;
        }

        if (value.isPressed)
        {
            SelfDestruct();
            EffectButtons effectButtons = FindObjectOfType<EffectButtons>();
            effectButtons.EnableEffectButtons();
            effectButtons.ToggleAllOff();
            death.KillProjectile();
            FindObjectOfType<GameManager>().CheckForLevelEnd();
        }
    }

    void SelfDestruct()
    {
        activeProjectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject proj in activeProjectiles)
        {
            ParticleSystem destructionEffect = GetComponentInChildren<ParticleSystem>();
            destructionEffect.transform.position = proj.transform.position;
            destructionEffect.Play();
            GetComponent<AudioSource>().PlayOneShot(selfDestruction, 0.5f);
            Destroy(proj);
        }
    }

    public int GetEnergyCost()
    {
        return energyCost;
    }

    public void DisableControls()
    {
        disabled = true;
    }

    public void EnableControls()
    {
        disabled = false;
    }

    public void SetTurnSensitivity(float newSensitivity)
    {
        turnSensitivity = newSensitivity;
    }

    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }

    public void RemoveCollider(Collider2D collider)
    {
        colliders.Remove(collider);
    }
}
