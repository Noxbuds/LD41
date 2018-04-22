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
    private Ship PlayerShip;
    private float HealthShowTimer;
    public float TargetDistance;

    // Whether the player is in sights
    private bool PlayerInSights = false;

    // Firing cooldown stuff
    public float ShootTimer;
    public float RailgunTemp; // Temperature of the railguns' internals, in degrees C
    public float CooldownMargin;

    // Sounds
    public AudioSource EngineSound;

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

    // UI Stuff
    public Texture HealthFG;
    public Texture HealthBG;

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

            // Set health show timer
            HealthShowTimer = 1f;
            
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

        // If the ship is the enemy, find the player
        if (!IsPlayer)
            PlayerShip = GameObject.Find("Player Ship").GetComponent<Ship>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the health show timer isn't zero, decrement it
        if (HealthShowTimer > 0)
            HealthShowTimer -= Time.deltaTime;

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
            RailgunTemp -= Time.deltaTime * 30f;

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

        // Player code
        if (IsPlayer)
        {
            // If the thrusters are not being fixed, then run them normally
            if (CurrentComponentId != 1)
            {
                // Fly forward
                if (Input.GetKey(KeyCode.W))
                {
                    // Move forward
                    MoveForward();

                    // Activate thrusters
                    transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    // Deactivate thrusters
                    transform.GetChild(0).gameObject.SetActive(false);
                }

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
                if (RailgunTemp < RailOverheatPoint * CooldownMargin)
                {
                    if (ShootTimer == 0)
                    {
                        // Shoot
                        if (Input.GetKey(KeyCode.UpArrow))
                        {
                            FireRailgun(0);
                        }
                        if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            FireRailgun(1);
                        }
                        if (Input.GetKey(KeyCode.RightArrow))
                        {
                            FireRailgun(2);
                        }
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
                {
                    MoveForward();
                }
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
        else
        {
            // Enemy code
            // How hard do we really want to make the AI? :|

            // Calculate where to shoot
            // Estimate travel time to player
            float ShellTravelTime = Vector2.Distance(PlayerShip.transform.position, this.transform.position) / MaxSpeed;

            // Estimate position
            Vector2 EstimatedPosition = PlayerShip.transform.position + Vector3.Project(PlayerShip._Rigidbody.velocity, PlayerShip.transform.forward) * ShellTravelTime;

            // Aim towards player
            Vector3 Target = PlayerShip.transform.position - transform.position;
            Target.Normalize();
            float TargetAngle = Mathf.Atan2(Target.y, Target.x) * Mathf.Rad2Deg;

            // Aiming to ship
            // Rotate towards player ship, and shoot
            if (PlayerShip.transform.position.y > transform.position.y && !PlayerInSights)
            {
                // Rotate
                if (TargetAngle < transform.eulerAngles.z)
                    transform.Rotate(new Vector3(0, 0, -TurnSpeed * Time.deltaTime));
                else
                    transform.Rotate(new Vector3(0, 0, TurnSpeed * Time.deltaTime));
            }
            else if (!PlayerInSights)
            {
                // Try to get below the player
                if (transform.eulerAngles.z - 180 > TargetAngle + 180)
                    transform.Rotate(new Vector3(0, 0, -TurnSpeed * Time.deltaTime));
                else
                    transform.Rotate(new Vector3(0, 0, TurnSpeed * Time.deltaTime));
            }

            // Checking for player being in sights
            if (PlayerShip.transform.position.y > transform.position.y)
                // Check for player being in sights
                PlayerInSights = Mathf.Abs(TargetAngle - transform.eulerAngles.z) < 5;
            else
                // Check for player being in sights
                PlayerInSights = Mathf.Abs((TargetAngle + 180) - (transform.eulerAngles.z - 180)) < 5;

            // Maintaining target distance
            // Get distance
            float Distance = Vector2.Distance(this.transform.position, PlayerShip.transform.position);

            // Move away from player if the distance is too far
            if (Distance < TargetDistance - 0.1f)
            {
                // If the player is above the ship, shoot (ai not working properly?)
                if (PlayerShip.transform.position.y > transform.position.y)
                {
                    // Rotate
                    if (TargetAngle + 180 < transform.eulerAngles.z - 180)
                        transform.Rotate(new Vector3(0, 0, -TurnSpeed * Time.deltaTime));
                    else
                        transform.Rotate(new Vector3(0, 0, TurnSpeed * Time.deltaTime));
                }
                else
                {
                    // Try to get below the player
                    if (transform.eulerAngles.z > TargetAngle)
                        transform.Rotate(new Vector3(0, 0, -TurnSpeed * Time.deltaTime));
                    else
                        transform.Rotate(new Vector3(0, 0, TurnSpeed * Time.deltaTime));
                }

                // Move forwards (away from player)
                MoveForward();
            }
            else if (Distance > TargetDistance + 0.1f)
            {
                // We're already facing the player, so move towards them
                MoveForward();
            }

            // Shoot
            if (ShootTimer == 0 && PlayerInSights)
                FireRailgun(0);
        }
    }

    // UI stuff
    void OnGUI()
    { 
        // UI Scale
        float UIScale = Screen.width / 2560f;

        // Only draw health bar if recently damaged
        if (HealthShowTimer > 0)
        {
            // Really just need to draw a small health bar

            // Get the screen base position
            Vector2 ScreenBasePos = Camera.main.WorldToScreenPoint(transform.position);

            // Calculate positions
            float HealthWidth = 250f * UIScale;
            float HealthHeight = 15f * UIScale;
            float ScreenX = ScreenBasePos.x - HealthWidth / 2f;
            float ScreenY = (Screen.height - ScreenBasePos.y) - 100f * UIScale;

            // Draw the background
            GUI.DrawTexture(new Rect(ScreenX, ScreenY, HealthWidth, HealthHeight), HealthBG);

            // Calculate the health percentage
            float Percentage = Health / MaxHealth;
            
            // Draw the foreground
            GUI.DrawTexture(new Rect(ScreenX, ScreenY, HealthWidth * Percentage, HealthHeight), HealthFG);
        }
    }

    // Moves the ship forward
    void MoveForward()
    {
        // Reduce speed
        _Rigidbody.velocity *= 0.9f;

        // Get the current speed in that direction
        float FlightSpeed = Vector3.Project(_Rigidbody.velocity, transform.right).magnitude;

        // Add a force and enable the thrusters
        _Rigidbody.AddRelativeForce(new Vector2(MaxSpeed - FlightSpeed, 0));
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
        ShellScript.ArmTimer = 0.05f; // this should be in seconds
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
        RailgunTemp += 20;
    }

    // Triggered when damage is received
    void OnDamage(float amount)
    {
        // Play a sound and stuff
    }
}
