using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateBehaviour : MonoBehaviour {

    // A reference to what kind of logic gate this is
    public Gates.LogicGate LogicGate;

    // Features specific to an actual instance of a gate
    public float Strength; // Determines the maximum current that can flow before incurring damage
    public bool Working; // Whether the gate is working or not (could be damaged)

    // The input and output booleans
    public bool Input1;
    public bool Input2;
    public bool Output;

    // The gates connected to inputs
    public GateBehaviour Input1Gate;
    public GateBehaviour Input2Gate;

    /// The gate connected to the output
    public GateBehaviour OutputGate;

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

        /*
        // Log stuff
        Debug.Log("Running logic for gate '" + gameObject.name + "'...");
        Debug.Log("Input gate 1 is '" + ((Input1Gate != null) ? Input1Gate.gameObject.name + "'" : "null'"));
        Debug.Log("Input gate 2 is '" + ((Input2Gate != null) ? Input2Gate.gameObject.name + "'" : "null'"));
        Debug.Log("Connected to gate '" + ((OutputGate == null) ? "null'" : OutputGate.gameObject.name + "'") + ", setting that gate's input #" + (OGCID + 1) + " to " + (Output ? "true" : "false"));*/
    }
}
