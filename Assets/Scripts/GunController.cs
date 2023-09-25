using System.Collections;
using System.Collections.Generic;
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
    Vector2 turnInput;
    [SerializeField] float turnSensitivity = -200;
    // Quaternion w-value for a right angle. Note, the w-value does not encode the direction of rotation, but the x, y, z values do.
    static readonly float rightAngle = Mathf.Sqrt(2) / 2;
    // Determines how much the rotation range of the gun is restricted.
    [SerializeField] float angleRestriction = 0.1f;
    // Keeps track of whether sound effect is playing.
    bool isPlaying = false;
    SliderController sliderController;
    bool hasSufficientEnergy = true;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] int energyCost = 10;
    DeathBehaviour death;

    void Start()
    {
        death = FindObjectOfType<DeathBehaviour>();
    }

    void Update()
    {
        float turnAmount = turnInput.x * turnSensitivity * Time.deltaTime;
        TurnTurret(turnAmount);
        UpdateTurnSFX(turnAmount);
    }

    void OnTurn(InputValue value)
    {
        turnInput = value.Get<Vector2>();
    }

    void OnFire(InputValue value)
    {
        sliderController = FindObjectOfType<SliderController>();
        CheckEnergy();

        if (!hasSufficientEnergy || !death.GetIsDead())
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
        gameObject.SetActive(false);
    }
}
