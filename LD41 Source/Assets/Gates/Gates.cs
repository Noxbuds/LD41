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

        // And a burned sprite
        public Sprite BurnedSprite;

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
    public static GameObject GateParent;

    // Create a static set of logic gates
    // AND Gate first
    public static LogicGate Gate_AND;
    public static LogicGate Gate_OR;
    public static LogicGate Gate_XOR;
    public static LogicGate Gate_NAND;
    public static LogicGate Gate_NOR;
    public static LogicGate Gate_XNOR;
    public static LogicGate Gate_NOT;
    public static LogicGate Gate_Power;
    public static LogicGate[] AllLogicGates;

    // Sets up the logic gates
    public static void SetupGates()
    {
        // Initialise the gates list
        CurrentGates = new List<GateBehaviour>();

        // Initialise All Logic Gates array
        AllLogicGates = new LogicGate[7];

        // Probably don't need to set outputs to false, but it's good to be sure :)
        // AND Gate details
        Gate_AND.GateName = "AND";
        Gate_AND.GateDescription = "Gives an output if both inputs are on.";
        Gate_AND.HasOneInput = false;

        // AND Gate outputs.
        Gate_AND.Output00 = false;
        Gate_AND.Output01 = false;
        Gate_AND.Output10 = false;
        Gate_AND.Output11 = true;

        AllLogicGates[0] = Gate_AND;

        // OR Gate details
        Gate_OR.GateName = "OR";
        Gate_OR.GateDescription = "Gives an output if any input is on.";
        Gate_OR.HasOneInput = false;

        // OR Gate outputs.
        Gate_OR.Output00 = false;
        Gate_OR.Output01 = true;
        Gate_OR.Output10 = true;
        Gate_OR.Output11 = true;

        AllLogicGates[1] = Gate_OR;

        // XOR Gate details
        Gate_XOR.GateName = "XOR";
        Gate_XOR.GateDescription = "Gives an output only if one input is on.";
        Gate_XOR.HasOneInput = false;

        // XOR Gate outputs.
        Gate_XOR.Output00 = false;
        Gate_XOR.Output01 = true;
        Gate_XOR.Output10 = true;
        Gate_XOR.Output11 = false;

        AllLogicGates[2] = Gate_XOR;

        // NAND Gate details
        Gate_NAND.GateName = "NAND";
        Gate_NAND.GateDescription = "Gives an output unless both outputs are on.";
        Gate_NAND.HasOneInput = false;

        // NAND Gate outputs.
        Gate_NAND.Output00 = true;
        Gate_NAND.Output01 = true;
        Gate_NAND.Output10 = true;
        Gate_NAND.Output11 = false;

        AllLogicGates[3] = Gate_NAND;

        // NOR Gate details
        Gate_NOR.GateName = "NOR";
        Gate_NOR.GateDescription = "Gives an output only if both inputs are on.";
        Gate_NOR.HasOneInput = false;

        // NOR Gate outputs.
        Gate_NOR.Output00 = true;
        Gate_NOR.Output01 = false;
        Gate_NOR.Output10 = false;
        Gate_NOR.Output11 = false;

        AllLogicGates[4] = Gate_NOR;

        // XNOR Gate details
        Gate_XNOR.GateName = "XNOR";
        Gate_XNOR.GateDescription = "Gives an output if both inputs are on or off at the same time; 00 or 11 provides an output.";
        Gate_XNOR.HasOneInput = false;
        
        // XNOR Gate outputs.
        Gate_XNOR.Output00 = false;
        Gate_XNOR.Output01 = true;
        Gate_XNOR.Output10 = true;
        Gate_XNOR.Output11 = false;

        AllLogicGates[5] = Gate_XNOR;

        // NOT Gate details
        Gate_NOT.GateName = "NOT";
        Gate_NOT.GateDescription = "Inverts an output; if the input is on, the output is off, and vice versa.";
        Gate_NOT.HasOneInput = true;

        // NOT Gate outputs.
        Gate_NOT.Output00 = true;
        Gate_NOT.Output01 = false;
        Gate_NOT.Output10 = true;
        Gate_NOT.Output11 = false;

        AllLogicGates[6] = Gate_NOT;
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
        // Check that there are no other gates nearby
        for (int i = 0; i < CurrentGates.Count; i++)
        {
            // If the distance is below a certain threshold, do not allow a gate to be placed
            if (Mathf.Pow(Vector2.Distance(Position, CurrentGates[i].transform.position), 2f) < 0.001f)
            {
                return null;
            }
        }

        // Setup the object
        GameObject GateObject = new GameObject();
        GateObject.transform.position = Position;
        GateObject.transform.localScale = Vector3.one * 0.2f;
        GateObject.name = "Gate (" + Gate.GateName + ")"; // name is just "Gate (Type)", using gate list count is gonna be odd :)
        GateObject.transform.parent = GateParent.transform;

        // Setup gate behaviour
        GateBehaviour GateScript = GateObject.AddComponent<GateBehaviour>();
        GateScript.LogicGate = Gate;
        GateScript.Working = true;

        // Setup sprite renderer
        SpriteRenderer GateSR = GateObject.AddComponent<SpriteRenderer>();
        GateSR.sprite = Gate.GateSprite;
        GateSR.sortingOrder = 3;

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
        // Store a temporary copy
        GateBehaviour CurrentGate = CurrentGates[GateIndex];

        // Remove the gate
        CurrentGates.RemoveAt(GateIndex);

        // Then run the delete method
        CurrentGate.Delete();
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

    // Burned sprites
    public Sprite Burned_AND;
    public Sprite Burned_OR;
    public Sprite Burned_XOR;
    public Sprite Burned_NAND;
    public Sprite Burned_NOR;
    public Sprite Burned_XNOR;
    public Sprite Burned_NOT;

    // A type for a connection between a power source/output and a gate
    // Needs to be public since the lists for the connections are...
    [System.Serializable]
    public class SourceConnection
    {
        // The gate that's connected
        public List<GateBehaviour> Gates;

        // The ID of the source/output it's connected to
        public int SourceID;

        // Whether this is an input or output connection
        public bool IsInputConnection;

        // The ID of the input on the gate this is connected to (not always used)
        public List<int> GateInputIDs;

        // Whether or not it's powered
        public bool Powered;

        // The label for it
        public string Label;

        // The description for it
        public string Description;

        /// <summary>
        /// Removes connections
        /// </summary>
        public void RemoveConnections(int id)
        {
            // Only do stuff with gate sources if the ID is less than 2 (2 = output-only connection)
            if (GateInputIDs[id] < 2)
            {
                // Sever inbound connection
                Gates[id].Sources[GateInputIDs[id]] = null;

                // Remove source wire
                if (Gates[id].SourceWires[GateInputIDs[id]] != null)
                    Destroy(Gates[id].SourceWires[GateInputIDs[id]]);
            }

            if (Gates[id].EndConnection == this)
            {
                // Sever outbound connection
                Gates[id].EndConnection = null;

                // Remove end wire
                if (Gates[id].EndWire != null)
                    Destroy(Gates[id].EndWire);
            }

            // Sever outbound connection
            Gates.RemoveAt(id);
            GateInputIDs.RemoveAt(id);
        }

        /// <summary>
        /// Resets all lists
        /// </summary>
        public void RemoveAllConnections()
        {
            while (Gates.Count > 0)
            {
                RemoveConnections(0);
            }
        }

        /// <summary>
        /// Gets the ID of a speciifc gate in the connections
        /// </summary>
        public int GetGateID(GateBehaviour Gate)
        {
            return Gates.FindIndex(0, Gates.Count, x => x == Gate);
        }
    }

    // A list of input and output connections
    public int InputCount;
    public int OutputCount;
    public List<SourceConnection> InputConnections;
    public List<SourceConnection> OutputConnections;
    
    // Private reference to level manager
    LevelManager _LevelManager;

    /// <summary>
    /// Assigns sprites to the gates
    /// </summary>
    public void AssignSprites()
    {
        // Assign normal sprites
        Gate_AND.GateSprite = Sprite_AND;
        Gate_OR.GateSprite = Sprite_OR;
        Gate_XOR.GateSprite = Sprite_XOR;
        Gate_NAND.GateSprite = Sprite_NAND;
        Gate_NOR.GateSprite = Sprite_NOR;
        Gate_XNOR.GateSprite = Sprite_XNOR;
        Gate_NOT.GateSprite = Sprite_NOT;

        // Assign burned sprites
        Gate_AND.BurnedSprite = Burned_AND;
        Gate_OR.BurnedSprite = Burned_OR;
        Gate_XOR.BurnedSprite = Burned_XOR;
        Gate_NAND.BurnedSprite = Burned_NAND;
        Gate_NOR.BurnedSprite = Burned_NOR;
        Gate_XNOR.BurnedSprite = Burned_XNOR;
        Gate_NOT.BurnedSprite = Burned_NOT;
    }

    /// <summary>
    /// Returns the current outputs of the current circuit board
    /// </summary>
    /// <returns></returns>
    public string GetCurrentOutputs()
    {
        string OutputString = "";

        // Loop through and setup a binary number in a string
        for (int i = 0; i < OutputConnections.Count; i++)
        {
            if (OutputConnections[i].Powered)
                OutputString = OutputString + "1";
            else
                OutputString = OutputString + "0";
        }
        
        // Return the string
        return OutputString;
    }

    /// <summary>
    /// Set the current inputs
    /// </summary>
    /// <param name="Inputs">A binary number (in a string). Each digit is a signal</param>
    public void SetInputs(string Inputs)
    {
        // Loop through each input
        for (int i = 0; i < Inputs.Length; i++)
        {
            // Make sure we're in range still
            if (InputConnections.Count > i)
            {
                if (Inputs[i] == '1')
                    InputConnections[i].Powered = true;
                else
                    InputConnections[i].Powered = false;
            }
        }
    }

    /// <summary>
    /// Fries all circuits. Haha.
    /// </summary>
    public void FryCircuits()
    {
        // Ruin the gates
        for (int i = 0; i < CurrentGates.Count; i++)
        {
            CurrentGates[i].Working = false;
            CurrentGates[i].GetComponent<SpriteRenderer>().sprite = CurrentGates[i].LogicGate.BurnedSprite;
        }

        // Disconnect the inputs
        for (int i = 0; i < InputConnections.Count; i++)
        {
            InputConnections[i].RemoveAllConnections();
        }

        // Disconnect the outputs
        for (int i = 0; i < OutputConnections.Count; i++)
        {
            OutputConnections[i].RemoveAllConnections();
        }

        // Play the sound
        Camera.main.GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// Called when the game starts/this object is created
    /// </summary>
    public void Initialise()
    {
        // Setup the gates first
        SetupGates();
        GateParent = new GameObject();
        GateParent.transform.position = new Vector3(0, 0, 5);
        GateParent.name = "Logic Gate Parent Object";

        // Assign the sprites
        AssignSprites();

        // Get the level manager
        _LevelManager = GameObject.FindObjectOfType<LevelManager>();

        // Initialise the source connections
        InputConnections = new List<SourceConnection>();
        OutputConnections = new List<SourceConnection>();

        // Add a source connection for each input and output
        for (int i = 0; i < InputCount; i++)
        {
            SourceConnection NewConnection = new SourceConnection();
            NewConnection.IsInputConnection = true;
            NewConnection.SourceID = i;
            NewConnection.Gates = new List<GateBehaviour>();
            NewConnection.GateInputIDs = new List<int>();

            InputConnections.Add(NewConnection);
        }

        for (int i = 0; i < OutputCount; i++)
        {
            SourceConnection NewConnection = new SourceConnection();
            NewConnection.IsInputConnection = false;
            NewConnection.SourceID = i;
            NewConnection.Gates = new List<GateBehaviour>();
            NewConnection.GateInputIDs = new List<int>();

            OutputConnections.Add(NewConnection);
        }
    }

    /// <summary>
    /// Called every frame by the engine
    /// </summary>
    void Update()
    {
        // Check that we don't keep outputs powered
        for (int i = 0; i < OutputConnections.Count; i++)
        {
            // Get a shorthand reference for the current source
            SourceConnection CurrentSource = OutputConnections[i];

            // Check that we don't keep something powered unnecessarily
            if (CurrentSource.Gates.Count < 1)
                CurrentSource.Powered = false;
        }

        // Loop through and make power flower
        if (_LevelManager.PowerFlowing)
        {
            // Go through the list of current gates and run the logic if they work
            for (int i = 0; i < CurrentGates.Count; i++)
            {
                CurrentGates[i].RunLogic();
            }

            // Go through the list of power sources and power
            for (int i = 0; i < InputConnections.Count; i++)
            {
                // Get a shorthand reference for the current source
                SourceConnection CurrentSource = InputConnections[i];

                // Check if powered
                if (CurrentSource.Powered)
                {
                    // Power each gate
                    for (int j = 0; j < InputConnections[i].Gates.Count; j++)
                    {
                        // Power gate
                        CurrentSource.Gates[j].Inputs[CurrentSource.GateInputIDs[j]] = true;
                    }
                }
                else
                {
                    // Power each gate
                    for (int j = 0; j < InputConnections[i].Gates.Count; j++)
                    {
                        // Power gate
                        CurrentSource.Gates[j].Inputs[CurrentSource.GateInputIDs[j]] = false;
                    }
                }
            }
        }
        else
        {
            // Go through the list of current gates and stop power
            for (int i = 0; i < CurrentGates.Count; i++)
            {
                // Stop power
                CurrentGates[i].SetInputs(false, false);
                CurrentGates[i].Output = false;
                CurrentGates[i].ColourWires();
            }

            // Go through the list of power sources and reset light
            for (int i = 0; i < InputConnections.Count; i++)
            {
                // Get a shorthand reference for the current source
                SourceConnection CurrentSource = InputConnections[i];

                // Power each gate
                for (int j = 0; j < InputConnections[i].Gates.Count; j++)
                {
                    // Un-power gate
                    CurrentSource.Gates[j].Inputs[CurrentSource.GateInputIDs[j]] = false;
                }
            }
        }
    }
}
