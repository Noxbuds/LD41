using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunShell : MonoBehaviour {

    // Simple behaviour. Hits a target, damages the target, destroys itself. 

    // The timer before it arms
    public float ArmTimer;

    // The damage this deals
    public float Damage;

    // Reference to the collider
    Collider2D Coll;

    void Start()
    {
        Coll = GetComponent<Collider2D>();
        Coll.enabled = false;
    }

    void Update()
    {
        if (ArmTimer > 0)
            ArmTimer -= Time.deltaTime;
        else if (Coll.enabled == false)
            Coll.enabled = true;
    }

    // Called on collision
	void OnCollisionEnter2D(Collision2D other)
    {
        // Try and fetch the ship
        Ship OtherShip = other.gameObject.GetComponent<Ship>();

        // If a ship was found, damage and destroy the shell
        if (OtherShip != null)
        {
            Debug.Log("Hit ship!");
            OtherShip.Health -= Damage * OtherShip.ShieldMultiplier;
        }
    }
}
