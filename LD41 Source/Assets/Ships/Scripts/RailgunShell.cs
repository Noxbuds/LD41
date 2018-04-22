using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunShell : MonoBehaviour {

    // Simple behaviour. Hits a target, damages the target, destroys itself. 

    // The timer before it arms
    public float ArmTimer;
    public float Timer;

    // The damage this deals
    public float Damage;

    // Bounds
    public float BoundsLeft;
    public float BoundsRight;
    public float BoundsUp;
    public float BoundsDown;

    // Reference to the collider
    Collider2D Coll;

    void Start()
    {
        Coll = GetComponent<Collider2D>();
        Coll.enabled = false;
    }

    void Update()
    {
        // Bounds checking, wrap around if need be
        // Checking right boundaries
        if (transform.position.x > BoundsRight)
            transform.position += new Vector3(-(BoundsRight - BoundsLeft) + 5, 0);

        // Checking left boundaries
        if (transform.position.x < BoundsLeft)
            transform.position += new Vector3(BoundsRight - BoundsLeft - 5, 0);

        // Checking top boundaries
        if (transform.position.y > BoundsUp)
            transform.position += new Vector3(0, -(BoundsUp - BoundsDown) + 5); // Wrap around

        // Checking bottom boundaries
        if (transform.position.y < BoundsDown)
            transform.position += new Vector3(0, BoundsUp - BoundsDown - 5); // Wrap around

        // Increment timer
        Timer += Time.deltaTime;

        // If it's alive for more than 4s, delete it
        if (Timer > 4)
            Destroy(this.gameObject);

        // Arm the shell
        if (ArmTimer > 0)
            ArmTimer -= Time.deltaTime;
        else if (Coll.enabled == false)
            Coll.enabled = true;
    }

    // Called on collision
	void OnTriggerEnter2D(Collider2D other)
    {
        // Try and fetch the ship
        Ship OtherShip = other.gameObject.GetComponent<Ship>();

        // If a ship was found, damage and destroy the shell
        if (OtherShip != null)
        {
            OtherShip.Health -= Damage * OtherShip.ShieldMultiplier;
            Destroy(this.gameObject);
        }
    }
}
