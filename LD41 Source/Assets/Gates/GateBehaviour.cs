using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateBehaviour : MonoBehaviour {

    // The two colours used
    public static Color UnpoweredColour = new Color(204f / 255f, 106f / 255f, 40f / 255f);
    public static Color PoweredColour = new Color(109f / 255f, 1, 1);

    // A reference to what kind of logic gate this is
    public Gates.LogicGate LogicGate;

    // Features specific to an actual instance of a gate
    public float Strength; // Determines the maximum current that can flow before incurring damage
    public bool Working; // Whether the gate is working or not (could be damaged)

    // The input and output booleans
    public bool[] Inputs;
    public bool Output;

    // Optional: source connections
    // The actual source will hold the wire(s)
    public Gates.SourceConnection[] Sources;

    // Optional: end connection
    public Gates.SourceConnection EndConnection;

    // Source/end wires
    public LineRenderer[] SourceWires;
    public LineRenderer EndWire;

    // The gates connected to inputs
    public GateBehaviour[] InputGates;

    /// The gate connected to the output
    public GateBehaviour OutputGate;

    // The line renderer attached to this
    // Side note on how they work:
    // Each gate will have one line renderer, their output. This will just draw from the
    // output terminal to the input terminal it's connected to. :)
    public LineRenderer WireObject;

    /// <summary>
    /// Output Gate Connection ID; the ID of the input in OutputGate that Output is connected to
    /// </summary>
    public int OGCID;

    // Set the inputs of the gate
    public void SetInputs(bool Input1, bool Input2)
    {
        // Make sure things are initialised
        if (Inputs == null)
        {
            Inputs = new bool[2];
            Sources = new Gates.SourceConnection[2];
            SourceWires = new LineRenderer[2];
            InputGates = new GateBehaviour[2];
        }

        // Assign things
        Inputs[0] = Input1;
        Inputs[1] = Input2;
    }

	// Run when necessary to check inputs; an update function
    // on every single gate is wasteful!
    public void RunLogic()
    {
        // Make sure things are initialised
        if (Inputs == null)
        {
            Inputs = new bool[2];
            Sources = new Gates.SourceConnection[2];
            SourceWires = new LineRenderer[2];
            InputGates = new GateBehaviour[2];
        }

        // Only do logic if it's working
        if (Working)
            // Just assign the output to the result of Gates.GetOutput()
            Output = Gates.GetOutput(LogicGate, Inputs[0], Inputs[1]);
        else
            Output = false;

        // Assign the next gate's output
        if (OutputGate != null)
        {
            // Check the OGCID; if it's 0, connect to the first input,
            // otherwise, connect to the second input
            if (OGCID == 0)
                OutputGate.Inputs[0] = Output;
            else
                OutputGate.Inputs[1] = Output;
        }

        // Assign the end's output
        if (EndConnection != null)
        {
            EndConnection.Powered = Output;
        }

        // Colour the wires
        ColourWires();
        
        // Log stuff
        /*Debug.Log("Running logic for gate '" + gameObject.name + "'...");
        Debug.Log("Input gate 1 is '" + ((Input1Gate != null) ? Input1Gate.gameObject.name + "'" : "null'"));
        Debug.Log("Input gate 2 is '" + ((Input2Gate != null) ? Input2Gate.gameObject.name + "'" : "null'"));
        Debug.Log("Connected to gate '" + ((OutputGate == null) ? "null'" : OutputGate.gameObject.name + "'") + ", setting that gate's input #" + (OGCID + 1) + " to " + (Output ? "true" : "false"));*/
    }

    /// <summary>
    /// Custom delete function. Necessary for deleting wires and removing connections
    /// </summary>
    public void Delete()
    {
        // Make sure all connections are removed
        // Sever first input connection
        if (InputGates[0] != null)
        {
            Destroy(InputGates[0].WireObject);
            InputGates[0].OutputGate = null;
            InputGates[0] = null;
        }
        
        // Sever second input connection
        if (InputGates[1] != null)
        {
            Destroy(InputGates[1].WireObject);
            InputGates[1].OutputGate = null;
            InputGates[1] = null;
        }

        // Sever the output connection
        if (OutputGate != null)
        {
            if (OGCID == 0)
                OutputGate.InputGates[0] = null;
            else
                OutputGate.InputGates[1] = null;
        }
        
        // Sever connections to sources
        if (Sources[0] != null)
            Sources[0].RemoveConnections(Sources[0].GetGateID(this));

        if (Sources[1] != null)
            Sources[1].RemoveConnections(Sources[1].GetGateID(this));

        if (EndConnection != null)
            EndConnection.RemoveConnections(EndConnection.GetGateID(this));

        // Delete source/end wires
        if (SourceWires[0] != null)
            Destroy(SourceWires[0]);

        if (SourceWires[1] != null)
            Destroy(SourceWires[1]);

        if (EndWire != null)
            Destroy(EndWire);

        // Destroy the gate. Any line renderers that are children to this should be destroyed too
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Colours the wires attached to this
    /// </summary>
    public void ColourWires()
    {
        // Colour the wire in if it's active
        if (Output && WireObject != null)
        {
            WireObject.startColor = WireObject.endColor = PoweredColour;
        }
        else if (WireObject != null)
        {
            WireObject.startColor = WireObject.endColor = UnpoweredColour;
        }

        // Change the end wire colours
        if (Output && EndWire != null)
        {
            EndWire.startColor = EndWire.endColor = PoweredColour;
        }
        else if (EndWire != null)
        {
            EndWire.startColor = EndWire.endColor = UnpoweredColour;
        }

        // Change the first source wire colours
        if (Inputs[0] && SourceWires[0] != null)
        {
            SourceWires[0].startColor = SourceWires[0].endColor = PoweredColour;
        }
        else if (SourceWires[0] != null)
        {
            SourceWires[0].startColor = SourceWires[0].endColor = UnpoweredColour;
        }

        // Change the second source wire colours
        if (Inputs[1] && SourceWires[1] != null)
        {
            SourceWires[1].startColor = SourceWires[1].endColor = PoweredColour;
        }
        else if (SourceWires[1] != null)
        {
            SourceWires[1].startColor = SourceWires[1].endColor = UnpoweredColour;
        }
    }
}
