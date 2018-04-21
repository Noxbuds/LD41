using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Gates {

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
        // Probably don't need to set outputs to false, but it's good to be sure :)
        // AND Gate details
        Gate_AND.GateName = "AND";
        Gate_AND.GateDescription = "Gives an output if both inputs are on";
        Gate_AND.GateSprite = Resources.Load("Gate Sprites/and gate") as Sprite;
        Gate_AND.HasOneInput = false;

        // AND Gate outputs.
        Gate_AND.Output00 = false;
        Gate_AND.Output01 = false;
        Gate_AND.Output10 = false;
        Gate_AND.Output11 = true;

        // OR Gate details
        Gate_OR.GateName = "OR";
        Gate_OR.GateDescription = "Gives an output if any input is on";
        Gate_OR.GateSprite = Resources.Load("Gate Sprites/or gate") as Sprite;
        Gate_OR.HasOneInput = false;

        // OR Gate outputs.
        Gate_OR.Output00 = false;
        Gate_OR.Output01 = true;
        Gate_OR.Output10 = true;
        Gate_OR.Output11 = true;

        // XOR Gate details
        Gate_XOR.GateName = "XOR";
        Gate_XOR.GateDescription = "Gives an output only if one input is on";
        Gate_XOR.GateSprite = Resources.Load("Gate Sprites/xor gate") as Sprite;
        Gate_XOR.HasOneInput = false;

        // XOR Gate outputs.
        Gate_XOR.Output00 = false;
        Gate_XOR.Output01 = true;
        Gate_XOR.Output10 = true;
        Gate_XOR.Output11 = false;

        // NAND Gate details
        Gate_NAND.GateName = "NAND";
        Gate_NAND.GateDescription = "Gives an output unless both outputs are on";
        Gate_NAND.GateSprite = Resources.Load("Gate Sprites/nand gate") as Sprite;
        Gate_NAND.HasOneInput = false;

        // NAND Gate outputs.
        Gate_NAND.Output00 = true;
        Gate_NAND.Output01 = true;
        Gate_NAND.Output10 = true;
        Gate_NAND.Output11 = false;

        // NOR Gate details
        Gate_NOR.GateName = "NOR";
        Gate_NOR.GateDescription = "Gives an output only if both inputs are on";
        Gate_NOR.GateSprite = Resources.Load("Gate Sprites/nor gate") as Sprite;
        Gate_NOR.HasOneInput = false;

        // NOR Gate outputs.
        Gate_NOR.Output00 = true;
        Gate_NOR.Output01 = false;
        Gate_NOR.Output10 = false;
        Gate_NOR.Output11 = false;

        // XNOR Gate details
        Gate_XNOR.GateName = "XNOR";
        Gate_XNOR.GateDescription = "Gives an output if both inputs are on or off at the same time; 00 or 11 provides an output";
        Gate_XNOR.GateSprite = Resources.Load("Gate Sprites/xnor gate") as Sprite;
        Gate_XNOR.HasOneInput = false;
        
        // XNOR Gate outputs.
        Gate_XNOR.Output00 = false;
        Gate_XNOR.Output01 = true;
        Gate_XNOR.Output10 = true;
        Gate_XNOR.Output11 = false;

        // NOT Gate details
        Gate_NOT.GateName = "NOT";
        Gate_NOT.GateDescription = "Inverts an output; if the input is on, the output is off, and vice versa";
        Gate_NOT.GateSprite = Resources.Load("Gate Sprites/not gate") as Sprite;
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
}
