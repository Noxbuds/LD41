using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateBehaviour : MonoBehaviour {

    // A reference to what kind of logic gate this is
    public Gates.LogicGate LogicGate;

	// Run when necessary to check inputs; an update function
    // on every single gate is wasteful!
    public void CheckInputs()
    {

    }
}
