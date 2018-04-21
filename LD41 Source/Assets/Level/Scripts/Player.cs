using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Private reference to level manager
    private LevelManager _LevelManager;
    
    // Details about player clicking/toolbox/etc
    private enum PlayerTool
    {
        PlaceGate, CreateWire
    }
    private enum GateSelection
    {
        AND, OR, XOR, NAND, NOR, XNOR, NOT
    }
    private PlayerTool CurrentTool;
    private GateSelection SelectedGate;
    public bool MouseInBounds;

	// Use this for initialization
	void Start ()
    {
        // Necessary for detecting mouse clicks
        Physics.queriesHitTriggers = true;

        // Get the level manager
        _LevelManager = GameObject.FindObjectOfType<LevelManager>();

        // Set the current tool for now
        CurrentTool = PlayerTool.PlaceGate;
        SelectedGate = GateSelection.AND;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (_LevelManager.ViewingPanel)
        {
            // Check for player clicks. Record the position if so.
            if (Input.GetMouseButtonUp(0))
            {
                // First we need to know if we hit the switch, if we're clicking to place
                // a logic gate or if we're dragging the view around (maybe not the last one?)
                if (MouseInBounds)
                {
                    // Get the player's mouse position in world co-ordinates
                    Vector2 MouseWorldCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // Spawn a logic gate or create wiring depending on the tool
                    if (CurrentTool == PlayerTool.PlaceGate)
                    {
                        // Spawn the selected logic gate at mouse position
                        switch (SelectedGate)
                        {
                            case GateSelection.AND:
                                Gates.SpawnGate(Gates.Gate_AND, MouseWorldCoords);
                                break;
                            case GateSelection.NAND:
                                Gates.SpawnGate(Gates.Gate_NAND, MouseWorldCoords);
                                break;
                            case GateSelection.OR:
                                Gates.SpawnGate(Gates.Gate_OR, MouseWorldCoords);
                                break;
                            case GateSelection.NOR:
                                Gates.SpawnGate(Gates.Gate_NOR, MouseWorldCoords);
                                break;
                            case GateSelection.XOR:
                                Gates.SpawnGate(Gates.Gate_XOR, MouseWorldCoords);
                                break;
                            case GateSelection.XNOR:
                                Gates.SpawnGate(Gates.Gate_XNOR, MouseWorldCoords);
                                break;
                            case GateSelection.NOT:
                                Gates.SpawnGate(Gates.Gate_NOT, MouseWorldCoords);
                                break;
                        }
                    }
                }
            }
        }
	}

    // Draw the player's UI
    void OnGUI()
    {
        // Draw the panel UI if need be
        if (_LevelManager.ViewingPanel)
        {
            // Loop through each gate and draw 4 buttons; one for each input,
            // one for the output, and one to delete it
            for (int i = 0; i < Gates.CurrentGates.Count; i++)
            {
                Vector2 Pos = Camera.main.WorldToScreenPoint(Gates.CurrentGates[i].transform.position);
                Pos.y = Screen.height - Pos.y;

                GUI.Box(new Rect(Pos, new Vector2(10, 10)), "");
            }
        }
    }
}
