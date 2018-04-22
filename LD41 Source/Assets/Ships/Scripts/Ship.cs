using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour
{

    // General ship script.
    // Controls movement, health, and damage.
    // Simple 2D movement, like in Star Control 2 combat

    // Important local references
    private Player ThePlayer;
    private LevelManager _LevelManager;
    private Rigidbody2D _Rigidbody;
    private Gates _Gates;

    // Firing cooldown stuff
    public float ShootTimer;
    public float RailgunTemp; // Temperature of the railguns' internals, in degrees C
    public float CooldownMargin;

    // Ship component
    [System.Serializable]
    public struct ShipComponent
    {
        // The ship components will work fairly simply. Each one
        // has a certain amount of health. It will be made so that
        // after each panel is fixed, a new level is started. The
        // player's current progress is saved, and any new level
        // code will be executed. The health bars are part of this,
        // with the enemy targetting and destroying certain components.
        public string Name;
        public float Health;

        // Size of the circuit board needed
        public LevelManager.EPanelSize BoardSize;

        // Input and output count, for level management
        public int InputCount;
        public int OutputCount;
    }
    
    // The id of the component currently being targetted
    public int CurrentComponentId;

    // The list of all components
    public ShipComponent[] AllComponents;

    // Here's a list of the components:
    /*
     * 0: None
     * 1: Thrusters
     * 2: Railguns
     */

    // Arena boundaries
    // These are positions relative to the x/y-axis
    public float BoundsLeft;
    public float BoundsUp;
    public float BoundsRight;
    public float BoundsDown;

    // Prefabs
    public GameObject RailgunShell;
    
    // Ship stats
    private float _Health;
    public float MaxHealth;
    public float Health
    {
        get { return _Health; }
        set
        {
            // Trigger OnDamage()
            // Bearing in mind we set the health to a value, so the actual
            // damage dealt is (old health) - (new health)
            OnDamage(_Health - value);
            
            // Assign health
            _Health = value;
        }
    }
    public float TurnSpeed; // How fast the ship turns. No actual units assigned, just a multiplier
    public float FireSpeed; // How fast the ship fires. Higher values = faster shooting
    public float FireForce; // How fast shells travel
    public float RailOverheatPoint; // Temperature the railguns overheat at
    public float ShieldMultiplier; // The damage multiplier of the shield. Lower values are better
    public bool IsPlayer; // Whether or not this is the player
    public float MaxSpeed;

    // Use this for initialization
    void Start()
    {
        // Set health to a default value
        _Health = MaxHealth;

        // Get a reference to our rigidbody
        _Rigidbody = GetComponent<Rigidbody2D>();

        // Get a reference to the gates manager
        _Gates = FindObjectOfType<Gates>();
    }

    // Update is called once per frame
    void Update()
    {
        // Normalize the velocity
        _Rigidbody.velocity.Normalize();

        // Set torque to zero
        _Rigidbody.angularVelocity = 0;

        // Decrement shoot timer. If it's greater than Time.deltaTime, then
        // subtract the delta time, otherwise set the shoot timer to 0 to
        // stop values like 0.00000000000001
        if (ShootTimer > Time.deltaTime)
            ShootTimer -= Time.deltaTime;
        else if (ShootTimer > 0)
            ShootTimer = 0;

        // Cool down the railguns
        if (RailgunTemp > 0)
            RailgunTemp -= Time.deltaTime * 15f;

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

        // If the thrusters are not being fixed, then run them normally
        if (CurrentComponentId != 1)
        {
            // Fly forward
            if (Input.GetKey(KeyCode.W))
            {
                // Get the current speed in that direction
                float FlightSpeed = Vector3.Dot(_Rigidbody.velocity, transform.forward);

                if (FlightSpeed < MaxSpeed)
                    // Add a force and enable the thrusters
                    _Rigidbody.AddRelativeForce(new Vector2(10f, 0));

                transform.GetChild(0).gameObject.SetActive(true);
            }
            else
                transform.GetChild(0).gameObject.SetActive(false);

            // Rotate
            if (Input.GetKey(KeyCode.A))
                transform.Rotate(new Vector3(0, 0, TurnSpeed * Time.deltaTime));

            if (Input.GetKey(KeyCode.D))
                transform.Rotate(new Vector3(0, 0, -TurnSpeed * Time.deltaTime));
        }
        else
        {
            // Setup a signal
            string InputSignal = "";

            // Set forward signal
            if (Input.GetKey(KeyCode.W))
                InputSignal += "1";
            else
                InputSignal += "0";

            // Set left rotation signal
            if (Input.GetKey(KeyCode.A))
                InputSignal += "1";
            else
                InputSignal += "0";

            // Set right rotation signal
            if (Input.GetKey(KeyCode.D))
                InputSignal += "1";
            else
                InputSignal += "0";

            // Set inputs
            _Gates.SetInputs(InputSignal);
        }

        // If the railguns aren't broken, shoot
        if (CurrentComponentId != 2)
        {
            // Don't shoot if overheated
            if (RailgunTemp < RailOverheatPoint)
            {
                // Shoot
                if (Input.GetKey(KeyCode.UpArrow) && ShootTimer == 0)
                {
                    FireRailgun(0);
                    ShootTimer = 2f / FireSpeed;
                }
                if (Input.GetKey(KeyCode.LeftArrow) && ShootTimer == 0)
                {
                    FireRailgun(1);
                    ShootTimer = 2f / FireSpeed;
                }
                if (Input.GetKey(KeyCode.RightArrow) && ShootTimer == 0)
                {
                    FireRailgun(2);
                    ShootTimer = 2f / FireSpeed;
                }
            }
        }
        else
        {
            // Setup a signal
            string InputSignal = "";

            // Set forward fire signal
            if (Input.GetKey(KeyCode.UpArrow) && ShootTimer == 0)
                InputSignal += "1";
            else
                InputSignal += "0";

            // Set left fire signal
            if (Input.GetKey(KeyCode.LeftArrow))
                InputSignal += "1";
            else
                InputSignal += "0";

            // Set right fire signal
            if (Input.GetKey(KeyCode.RightArrow))
                InputSignal += "1";
            else
                InputSignal += "0";

            // Set overheating signal
            if (RailgunTemp > RailOverheatPoint * CooldownMargin)
                InputSignal += "1";
            else
                InputSignal += "0";

            // Send signal
            _Gates.SetInputs(InputSignal);
        }

        // Get the output of the current circuit board
        string Output = _Gates.GetCurrentOutputs();

        // Handle the output for the current circuit board
        if (CurrentComponentId == 1)
        {
            // Two thruster 'clusters'
            // Left and right - if you power both, you go forward, power only one
            // and you turn

            // Check we have all 2 outputs
            if (Output.Length < 2)
                Debug.LogError("Too few outputs");

            // First sort out the rotation, that's pretty easy
            // Anti-clockwise rotation
            if (Output[0] == '1')
                transform.Rotate(new Vector3(0, 0, -TurnSpeed * Time.deltaTime));
            
            // Clockwise rotation
            if (Output[1] == '1')
                transform.Rotate(new Vector3(0, 0, TurnSpeed * Time.deltaTime));

            // Now for the forward thrust
            if (Output[0] == '1' && Output[1] == '1')
                _Rigidbody.AddRelativeForce(new Vector2(10, 0));
        }

        // Railguns
        if (CurrentComponentId == 2)
        {
            // As in the guide on components

            // First check we have all outputs
            if (Output.Length < 3)
                Debug.LogError("Too few outputs");
            
            // Shoot every few milliseconds
            if (ShootTimer == 0)
            {
                // Forward railgun
                if (Output[0] == '1')
                    // Fire forward railgun
                    FireRailgun(0);

                // Left railgun
                if (Output[1] == '1')
                    FireRailgun(1);

                // Right railgun
                if (Output[2] == '1')
                    FireRailgun(2);
            }

            // Check temperature. If it's too high, fry all the circuits
            if (RailgunTemp > RailOverheatPoint)
            {
                _Gates.FryCircuits();
            }
        }
    }

    // Fire railgun
    // Directions: 0 = forward, 1 = left, 2 = right
    void FireRailgun(int Direction)
    {
        // Create a railgun shell
        GameObject Shell = Instantiate(RailgunShell, this.transform);
        Shell.transform.localPosition = Vector3.zero;

        // Set parameters on the shell
        RailgunShell ShellScript = Shell.GetComponent<RailgunShell>();
        ShellScript.ArmTimer = 1; // this should be in seconds
        ShellScript.Damage = 2;

        // Set rotation
        if (Direction == 0)
            Shell.transform.rotation = transform.rotation;
        else if (Direction == 1)
        {
            Shell.transform.rotation = transform.rotation;
            Shell.transform.Rotate(new Vector3(0, 0, 45));
        }
        else if (Direction == 2)
        {
            Shell.transform.rotation = transform.rotation;
            Shell.transform.Rotate(new Vector3(0, 0, -45));
        }

        // Give it a base velocity
        Shell.GetComponent<Rigidbody2D>().velocity = _Rigidbody.velocity;

        // And give it some force
        Shell.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(FireForce, 0));

        // Un-parent it
        Shell.transform.SetParent(null);

        // Set bounds
        ShellScript.BoundsLeft = BoundsLeft;
        ShellScript.BoundsUp = BoundsUp;
        ShellScript.BoundsRight = BoundsRight;
        ShellScript.BoundsDown = BoundsDown;

        // Set shoot timer
        ShootTimer = 1f / FireSpeed;

        // Increase heat
        if (CurrentComponentId == 2)
            RailgunTemp += 20;
    }

    // Triggered when damage is received
    void OnDamage(float amount)
    {
        // Play a sound and stuff
    }
}
