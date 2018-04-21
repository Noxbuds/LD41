using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour {

	// The meat of the logic gate system. This class handles pretty much
    // everything about them, the behaviour is just to get some interaction
    // in the game world.

    // Struct for a logic gate. Contains some details about the logic
    // gate, and the outputs it provides
    public struct LogicGate
    {
        // The name of the logic gate
        public string GateName;

        // We also need a sprite
        public Sprite GateSprite;

        // Tooltip/description for each logic gate
        public string GateDescription;

        // Check if NOT gate; this makes it only have one input
        public bool HasOneInput;

        // A set of outputs. The binary number after 'Output' corresponds
        // to the input which gives this output.
        public bool Output00;
        public bool Output01;
        public bool Output10;
        public bool Output11;
    }

    // A list of currently spawned gates
    public static List<GateBehaviour> CurrentGates;

    // Create a static set of logic gates
    // AND Gate first
    public static LogicGate Gate_AND;
    public static LogicGate Gate_OR;
    public static LogicGate Gate_XOR;
    public static LogicGate Gate_NAND;
    public static LogicGate Gate_NOR;
    public static LogicGate Gate_XNOR;
    public static LogicGate Gate_NOT;

    // Sets up the logic gates
    public static void SetupGates()
    {
        // Initialise the gates list
        CurrentGates = new List<GateBehaviour>();

        // Probably don't need to set outputs to false, but it's good to be sure :)
        // AND Gate details
        Gate_AND.GateName = "AND";
        Gate_AND.GateDescription = "Gives an output if both inputs are on";
        Gate_AND.HasOneInput = false;

        // AND Gate outputs.
        Gate_AND.Output00 = false;
        Gate_AND.Output01 = false;
        Gate_AND.Output10 = false;
        Gate_AND.Output11 = true;

        // OR Gate details
        Gate_OR.GateName = "OR";
        Gate_OR.GateDescription = "Gives an output if any input is on";
        Gate_OR.HasOneInput = false;

        // OR Gate outputs.
        Gate_OR.Output00 = false;
        Gate_OR.Output01 = true;
        Gate_OR.Output10 = true;
        Gate_OR.Output11 = true;

        // XOR Gate details
        Gate_XOR.GateName = "XOR";
        Gate_XOR.GateDescription = "Gives an output only if one input is on";
        Gate_XOR.HasOneInput = false;

        // XOR Gate outputs.
        Gate_XOR.Output00 = false;
        Gate_XOR.Output01 = true;
        Gate_XOR.Output10 = true;
        Gate_XOR.Output11 = false;

        // NAND Gate details
        Gate_NAND.GateName = "NAND";
        Gate_NAND.GateDescription = "Gives an output unless both outputs are on";
        Gate_NAND.HasOneInput = false;

        // NAND Gate outputs.
        Gate_NAND.Output00 = true;
        Gate_NAND.Output01 = true;
        Gate_NAND.Output10 = true;
        Gate_NAND.Output11 = false;

        // NOR Gate details
        Gate_NOR.GateName = "NOR";
        Gate_NOR.GateDescription = "Gives an output only if both inputs are on";
        Gate_NOR.HasOneInput = false;

        // NOR Gate outputs.
        Gate_NOR.Output00 = true;
        Gate_NOR.Output01 = false;
        Gate_NOR.Output10 = false;
        Gate_NOR.Output11 = false;

        // XNOR Gate details
        Gate_XNOR.GateName = "XNOR";
        Gate_XNOR.GateDescription = "Gives an output if both inputs are on or off at the same time; 00 or 11 provides an output";
        Gate_XNOR.HasOneInput = false;
        
        // XNOR Gate outputs.
        Gate_XNOR.Output00 = false;
        Gate_XNOR.Output01 = true;
        Gate_XNOR.Output10 = true;
        Gate_XNOR.Output11 = false;

        // NOT Gate details
        Gate_NOT.GateName = "NOT";
        Gate_NOT.GateDescription = "Inverts an output; if the input is on, the output is off, and vice versa";
        Gate_NOT.HasOneInput = true;

        // NOT Gate outputs.
        Gate_NOT.Output00 = false;
        Gate_NOT.Output01 = true;
        Gate_NOT.Output10 = true;
        Gate_NOT.Output11 = false;
    }
    
    /// <summary>
    /// Returns the output of the logic gate with the given inputs
    /// </summary>
    /// <param name="Gate"></param>
    /// <returns></returns>
    public static bool GetOutput(LogicGate Gate, bool Input1, bool Input2)
    {
        // Quick if-statement
        // Supposedly a few 'if' statements are faster than switch statement? o.0
        if (!Input1 && !Input2)
            return Gate.Output00;
        else if (Input1 && !Input2)
            return Gate.Output01;
        else if (!Input1 && Input2)
            return Gate.Output10;
        else if (Input1 && Input2)
            return Gate.Output11;
        
        // If somehow the inputs aren't true or false (??????) then return an error
        Debug.LogError("Somehow the inputs aren't true or false?!");
        return false;
    }

    /// <summary>
    /// Spawns a logic gate in the world
    /// </summary>
    /// <param name="Gate">The gate type to spawn</param>
    /// <param name="Location">The world position to spawn it</param>
    public static GateBehaviour SpawnGate(LogicGate Gate, Vector2 Position)
    {
        // Setup the object
        GameObject GateObject = new GameObject();
        GateObject.transform.position = Position;
        GateObject.name = "Gate (" + Gate.GateName + ")"; // name is just "Gate (Type)", using gate list count is gonna be odd :)

        // Setup gate behaviour
        GateBehaviour GateScript = GateObject.AddComponent<GateBehaviour>();
        GateScript.LogicGate = Gate;

        // Setup sprite renderer
        SpriteRenderer GateSR = GateObject.AddComponent<SpriteRenderer>();
        GateSR.sprite = Gate.GateSprite;

        // Add the gate to the list, bearing in mind we need the GateBehaviour
        CurrentGates.Add(GateScript);

        // Return the gate behaviour
        return GateScript;
    }

    /// <summary>
    /// Destroys a specific gate in the gate list
    /// </summary>
    /// <param name="GateIndex">The index in the CurrentGates list</param>
    public static void DestroyGate(int GateIndex)
    {
        // Just destroy the game object
        GameObject.Destroy(CurrentGates[GateIndex].gameObject);
    }

    /// <summary>
    /// Fetches the index in the CurrentGates list for a specific object with a gate behaviour
    /// </summary>
    /// <param name="Gate">The gate behaviour component of the game object</param>
    /// <returns></returns>
    public static int GetGateID(GateBehaviour Gate)
    {
        // Return the index of the gate in the list
        return CurrentGates.FindIndex(0, CurrentGates.Count, x => x == Gate);
    }

    // Non-static code, for world interaction
    // Replacing an earlier temporary 'GateManager' script

    // Gate sprites, since apparently creating a sprite 'manually' is difficult
    // and I know this will be easier
    public Sprite Sprite_AND;
    public Sprite Sprite_OR;
    public Sprite Sprite_XOR;
    public Sprite Sprite_NAND;
    public Sprite Sprite_NOR;
    public Sprite Sprite_XNOR;
    public Sprite Sprite_NOT;

    // A type for a connection between a power source/output and a gate
    // Needs to be public since the lists for the connections are...
    public struct SourceConnection
    {
        // The gate that's connected
        public GateBehaviour Gate;

        // The ID of the source/output it's connected to
        public int SourceID;

        // Whether this is an input or output connection
        public bool IsInputConnection;

        // The ID of the input on the gate this is connected to (not always used)
        public int GateInputID;
    }

    // A type for holding data about an input or ouput
    [System.Serializable]
    public struct InputOutputData
    {
        // Whether this is powered or not
        public bool Powered;

        // The position of this connection
        public Vector2 Position;
    }

    // A list of inputs and outputs, and their positions
    // Note: Inputs and outputs must be setup on a per-level basis
    public List<InputOutputData> Inputs;
    public List<InputOutputData> Outputs;

    // And a list of input and output connections
    public List<SourceConnection> InputConnections;
    public List<SourceConnection> OutputConnections;
    
    /// <summary>
    /// Assigns sprites to the gates
    /// </summary>
    void AssignSprites()
    {
        Gate_AND.GateSprite = Sprite_AND;
        Gate_OR.GateSprite = Sprite_OR;
        Gate_XOR.GateSprite = Sprite_XOR;
        Gate_NAND.GateSprite = Sprite_NAND;
        Gate_NOR.GateSprite = Sprite_NOR;
        Gate_XNOR.GateSprite = Sprite_XNOR;
        Gate_NOT.GateSprite = Sprite_NOT;
    }

    GateBehaviour TestGate1;
    GateBehaviour TestGate2;

    /// <summary>
    /// Called when the game starts/this object is created
    /// </summary>
    void Start()
    {
        // Setup the gates first
        SetupGates();

        // Assign the sprites
        AssignSprites();
    }

    /// <summary>
    /// Called every frame by the engine
    /// </summary>
    void Update()
    {
        // Go through the list of current gates and run the logic if they work
        for (int i = 0; i < CurrentGates.Count; i++)
        {
            if (CurrentGates[i].Working)
                CurrentGates[i].RunLogic();
        }
    }
}
