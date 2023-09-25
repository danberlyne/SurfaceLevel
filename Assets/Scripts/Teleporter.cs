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
        Vector3 pointOfContactRelBoundary = projectile.gameObject.transform.position - boundaryAnchor;
        projectile.gameObject.transform.position = otherTeleporter.GetBoundaryAnchor() + pointOfContactRelBoundary;
        projectile.gameObject.transform.rotation = transform.rotation; // Do we need localRotation instead? Or perhaps velocity of the rigidbody needs to be changed?
    }

    public Vector3 GetBoundaryAnchor()
    {
        return boundaryAnchor;
    }
}
