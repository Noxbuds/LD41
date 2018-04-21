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
    public bool Input1;
    public bool Input2;
    public bool Output;

    // Optional: source connections
    // The actual source will hold the wire(s)
    public Gates.SourceConnection Source1;
    public Gates.SourceConnection Source2;

    // Optional: end connection
    public Gates.SourceConnection EndConnection;

    // Source/end wires
    public LineRenderer Source1Wire;
    public LineRenderer Source2Wire;
    public LineRenderer EndWire;

    // The gates connected to inputs
    public GateBehaviour Input1Gate;
    public GateBehaviour Input2Gate;

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

	// Run when necessary to check inputs; an update function
    // on every single gate is wasteful!
    public void RunLogic()
    {
        // Just assign the output to the result of Gates.GetOutput()
        Output = Gates.GetOutput(LogicGate, Input1, Input2);

        // Colour the wires
        ColourWires();

        // Assign the next gate's output
        if (OutputGate != null)
        {
            // Check the OGCID; if it's 0, connect to the first input,
            // otherwise, connect to the second input
            if (OGCID == 0)
                OutputGate.Input1 = Output;
            else
                OutputGate.Input2 = Output;
        }

        // Assign the end's output
        if (EndConnection != null)
        {
            EndConnection.Powered = Output;
        }
        
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
        if (Input1Gate != null)
        {
            Destroy(Input1Gate.WireObject);
            Input1Gate.OutputGate = null;
            Input1Gate = null;
        }
        
        // Sever second input connection
        if (Input2Gate != null)
        {
            Destroy(Input2Gate.WireObject);
            Input2Gate.OutputGate = null;
            Input2Gate = null;
        }

        // Sever the output connection
        if (OutputGate != null)
        {
            if (OGCID == 0)
                OutputGate.Input1Gate = null;
            else
                OutputGate.Input2Gate = null;
        }
        
        // Sever connections to sources
        if (Source1 != null)
            Source1.RemoveConnections(Source1.GetGateID(this));

        if (Source2 != null)
            Source2.RemoveConnections(Source2.GetGateID(this));

        if (EndConnection != null)
            EndConnection.RemoveConnections(EndConnection.GetGateID(this));

        // Delete source/end wires
        if (Source1Wire != null)
            Destroy(Source1Wire);

        if (Source2Wire != null)
            Destroy(Source2Wire);

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
        if (Input1 && Source1Wire != null)
        {
            Source1Wire.startColor = Source1Wire.endColor = PoweredColour;
        }
        else if (Source1Wire != null)
        {
            Source1Wire.startColor = Source1Wire.endColor = UnpoweredColour;
        }

        // Change the second source wire colours
        if (Input2 && Source2Wire != null)
        {
            Source2Wire.startColor = Source2Wire.endColor = PoweredColour;
        }
        else if (Source2Wire != null)
        {
            Source2Wire.startColor = Source2Wire.endColor = UnpoweredColour;
        }
    }
}
