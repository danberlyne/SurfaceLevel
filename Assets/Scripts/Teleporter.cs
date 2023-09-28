using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    Vector3 boundaryAnchor;
    GameObject[] opposites;
    GameObject otherBoundary;
    Teleporter otherTeleporter;

    void Start()
    {
        boundaryAnchor = transform.parent.position;

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
        Vector3 projectilePosition = projectile.gameObject.transform.position;
        Vector2 projectilePosition2D = projectile.gameObject.transform.position;
        Vector2 collisionPoint = boundaryCollider.ClosestPoint(projectilePosition);
        Vector3 pointOfContactRelBoundary = new Vector3 (collisionPoint.x, collisionPoint.y, projectilePosition.z) - boundaryAnchor;
        if (Vector2.Distance(collisionPoint, projectilePosition2D + projectile.attachedRigidbody.velocity) < Vector2.Distance(collisionPoint, projectilePosition2D - projectile.attachedRigidbody.velocity))
        {
            projectile.gameObject.transform.position = otherTeleporter.GetBoundaryAnchor() + pointOfContactRelBoundary;
            // projectile.gameObject.transform.rotation = transform.rotation; // Do we need localRotation instead? Or perhaps velocity of the rigidbody needs to be changed?
        }
    }

    public Vector3 GetBoundaryAnchor()
    {
        return boundaryAnchor;
    }
}
