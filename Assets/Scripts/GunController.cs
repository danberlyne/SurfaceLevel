using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    [Header("Projectiles")]
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] int energyCost = 10;
    DeathBehaviour death;
    [SerializeField] AudioClip selfDestruction;
    GameObject[] activeProjectiles;
    [Header("Trajectory")]
    [SerializeField] LineRenderer lineRenderer;
    List<Vector3> path;
    [SerializeField] float trajectoryLength = 1f;

    void Awake()
    {
        death = FindObjectOfType<DeathBehaviour>();
    }

    void Start()
    {
        StartCoroutine(StartUp());
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
            sliderController.UpdateEnergy(energyCost);
            FindObjectOfType<EffectButtons>().DisableEffectButtons();
        }
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

    void TurnTurret(float turnAmount)
    {
        // 'true' if turnAmount is decreasing the absolute value of the current angle of rotation.
        bool isOppositeDirection = Mathf.Sign(transform.rotation.z) != Mathf.Sign(turnAmount);
        if (transform.rotation.w > rightAngle + angleRestriction || isOppositeDirection)
        {
            transform.Rotate(0, 0, turnAmount);
        }
    }

    void DisplayTrajectory()
    {
        lineRenderer.enabled = true;
        path = SimulateTrajectory();
        RenderTrajectory(path);
    }

    List<Vector3> SimulateTrajectory()
    {
        float gunAngle = Mathf.Sign(transform.rotation.z) * 2 * Mathf.Acos(transform.rotation.w);
        Vector3 position = projectileSpawn.position;
        Vector3 velocity = new Vector3(projectileSpeed * Mathf.Sin(gunAngle), -projectileSpeed * Mathf.Cos(gunAngle), 0);
        List<Vector3> path = new List<Vector3>();

        float duration = trajectoryLength; 
        float timestep = Time.fixedDeltaTime;
        for (float t = 0f; t < duration; t += timestep)
        {
            velocity += Physics.gravity * timestep;
            velocity = velocity * (1 - timestep * projectile.GetComponent<Rigidbody2D>().drag);
            position += velocity * timestep;
            path.Add(position);
        }

        return path;
    }

    void RenderTrajectory(List<Vector3> path)
    {
        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.ToArray());
    }

    void HideTrajectory()
    {
        lineRenderer.enabled = false;
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

    void CheckEnergy()
    {
        int energyAmount = sliderController.GetRemainingEnergy();
        hasSufficientEnergy = energyAmount >= energyCost;
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

    void SelfDestruct()
    {
        activeProjectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject proj in activeProjectiles)
        {
            ParticleSystem destructionEffect = GetComponentInChildren<ParticleSystem>();
            destructionEffect.transform.position = proj.transform.position;
            destructionEffect.Play();
            GetComponent<AudioSource>().PlayOneShot(selfDestruction);
            Destroy(proj);
        }
    }

    public void SetTurnSensitivity(float newSensitivity)
    {
        turnSensitivity = newSensitivity;
    }

    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }
}
