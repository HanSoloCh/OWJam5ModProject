using UnityEngine;

namespace OWJam5ModProject;

[RequireComponent(typeof(ShipBody))]
public class ShipContactSensor : MonoBehaviour
{
    public static OWRigidbody lastContact;

    private void OnCollisionEnter(Collision other)
    {
        lastContact = other.rigidbody.GetAttachedOWRigidbody();
    }
}