using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Teleporter : MonoBehaviour
{
    Vector2 boundaryAnchor;
    Vector2 normalVector;
    GameObject[] opposites;
    GameObject otherBoundary;
    Teleporter otherTeleporter;
    [SerializeField] float teleportationOffset = 0.1f;

    // Returns the Vector2 `v` rotated `theta` degrees anticlockwise.
    public static Vector2 Rotate(Vector2 v, float theta)
    {
        return new Vector2(
            v.x * Mathf.Cos(theta * Mathf.PI / 180) - v.y * Mathf.Sin(theta * Mathf.PI / 180),
            v.x * Mathf.Sin(theta * Mathf.PI / 180) + v.y * Mathf.Cos(theta * Mathf.PI / 180)
        );
    }

    void Start()
    {
        boundaryAnchor = transform.parent.position;
        Vector2 boundaryStart = transform.parent.GetChild(1).position;
        Vector2 boundaryEnd = transform.parent.GetChild(2).position;
        normalVector = Rotate(boundaryEnd - boundaryStart, 90) / Vector2.Distance(boundaryStart, boundaryEnd);

        if (tag == "PositiveBoundary")
        {
            opposites = GameObject.FindGameObjectsWithTag("NegativeBoundary");
        }
        else
        {
            opposites = GameObject.FindGameObjectsWithTag("PositiveBoundary");
        }

        foreach (GameObject opposite in opposites)
        {
            if (transform.parent.parent.gameObject == opposite.transform.parent.parent.gameObject)
            {
                otherBoundary = opposite;
                otherTeleporter = otherBoundary.GetComponent<Teleporter>();
            }
        }
    }
    void OnTriggerEnter2D(Collider2D projectile)
    {
        Collider2D boundaryCollider = GetComponent<Collider2D>();
        Vector2 projectilePosition = projectile.gameObject.transform.position;
        Vector2 projectilePosition2D = projectile.gameObject.transform.position;
        Vector2 collisionPoint = boundaryCollider.ClosestPoint(projectilePosition);
        Vector2 pointOfContactRelBoundary = new Vector2 (collisionPoint.x, collisionPoint.y) - boundaryAnchor;
        if (Vector2.Angle(normalVector, projectile.attachedRigidbody.velocity) > 90)
        {
            projectile.gameObject.transform.position = otherTeleporter.GetBoundaryAnchor() + pointOfContactRelBoundary + otherTeleporter.GetNormalVector() * teleportationOffset;
            projectile.attachedRigidbody.velocity = Rotate(projectile.attachedRigidbody.velocity, Vector2.Angle(-normalVector, otherTeleporter.GetNormalVector()));
        }
    }

    public Vector2 GetBoundaryAnchor()
    {
        return boundaryAnchor;
    }

    public Vector2 GetNormalVector()
    {
        return normalVector;
    }

}
