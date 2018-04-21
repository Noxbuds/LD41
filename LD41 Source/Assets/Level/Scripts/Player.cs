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

    // Wire dragging stuff
    public bool IsDraggingWire;
    public bool DraggingOutput;
    public GateBehaviour GateDraggingFrom; // the gate we started dragging from
    public int GateDraggingFromInputID; // the input ID of the gate we're dragging from

    // Sound
    public AudioSource PickupSound;
    public AudioSource DropSound;

    // UI stuff
    public Texture2D InputButtonTex;
    public Texture2D OutputButtonTex;
    private GUIStyle InputButtonStyle;
    private GUIStyle OutputButtonStyle;

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

        // Setup UI styles
        InputButtonStyle = new GUIStyle();
        InputButtonStyle.normal.background = InputButtonTex;

        OutputButtonStyle = new GUIStyle();
        OutputButtonStyle.normal.background = OutputButtonTex;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
            CurrentTool = PlayerTool.PlaceGate;
        if (Input.GetKeyUp(KeyCode.Alpha2))
            CurrentTool = PlayerTool.CreateWire;

        if (Input.GetKeyUp(KeyCode.A))
            SelectedGate = GateSelection.AND;
        else if (Input.GetKeyUp(KeyCode.B))
            SelectedGate = GateSelection.NOT;


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
                        // Play a sound
                        DropSound.Play();

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
        // Get a UI scaling variable
        float UIScale = Screen.width / 2560f;

        // Draw the panel UI if need be
        if (_LevelManager.ViewingPanel)
        {
            // Draw a 'toolbox' at the top of the screen


            // Fetch a local copy of the gates list
            List<GateBehaviour> GateList = Gates.CurrentGates;

            // Loop through each gate and draw 4 buttons; one for each input,
            // one for the output, and one to delete it
            for (int i = 0; i < GateList.Count; i++)
            {
                // Get the screen position of the logic gate
                Vector2 GatePosition = Camera.main.WorldToScreenPoint(GateList[i].transform.position);
                GatePosition.y = Screen.height - GatePosition.y;

                // Draw the output button
                float OutputX = GatePosition.x + 46f * UIScale;
                float OutputY = GatePosition.y - 14f * UIScale;
                float OutputWidth = 28 * UIScale;

                // Toggle the player dragging status if clicked on
                if (GUI.Button(new Rect(OutputX, OutputY, OutputWidth, OutputWidth), "", OutputButtonStyle) && CurrentTool == PlayerTool.CreateWire)
                {
                    // If we aren't already dragging, then set it so we are dragging,
                    // and that we're dragging an output wire
                    if (!IsDraggingWire)
                    {
                        PickupSound.Play();
                        IsDraggingWire = true;
                        DraggingOutput = true;
                        GateDraggingFrom = Gates.CurrentGates[i];
                    }
                    else if (IsDraggingWire && !DraggingOutput)
                    {
                        // Play sound
                        DropSound.Play();

                        // However if we are dragging a wire and it's not an output
                        // wire, then connect them
                        IsDraggingWire = false;

                        // Connect this gate to the one we dragged from
                        Gates.CurrentGates[i].OutputGate = GateDraggingFrom;
                        Gates.CurrentGates[i].OGCID = GateDraggingFromInputID;

                        // Connect the one we dragged from to this gate
                        if (GateDraggingFromInputID == 0)
                            GateDraggingFrom.Input1Gate = Gates.CurrentGates[i];
                        else
                            GateDraggingFrom.Input2Gate = Gates.CurrentGates[i];
                    }
                }

                // Now draw the input buttons
                if (!GateList[i].LogicGate.HasOneInput)
                {
                    // Two inputs
                    float InputX = GatePosition.x - 68f * UIScale;
                    float InputY1 = GatePosition.y - 44f * UIScale;
                    float InputY2 = GatePosition.y + 16f * UIScale;
                    float InputWidth = 28 * UIScale;

                    // Handle wire dragging
                    if (GUI.Button(new Rect(InputX, InputY1, InputWidth, InputWidth), "", InputButtonStyle) && CurrentTool == PlayerTool.CreateWire)
                    {
                        if (!IsDraggingWire)
                        {
                            // Play sound
                            PickupSound.Play();

                            // Fetch all the details we need and set the player to be dragging
                            IsDraggingWire = true;
                            DraggingOutput = false;
                            GateDraggingFrom = Gates.CurrentGates[i];
                            GateDraggingFromInputID = 0;
                        }
                        else if (IsDraggingWire && DraggingOutput)
                        {
                            // Play sound
                            DropSound.Play();

                            // First stop the player from dragging anymore
                            IsDraggingWire = false;

                            // Then connect this gate to the one we dragged from
                            Gates.CurrentGates[i].Input1Gate = GateDraggingFrom;

                            // And connect the one we dragged from to this gate
                            GateDraggingFrom.OutputGate = Gates.CurrentGates[i];
                            GateDraggingFrom.OGCID = 0;
                        }
                    }

                    // Handle wire dragging
                    if (GUI.Button(new Rect(InputX, InputY2, InputWidth, InputWidth), "", InputButtonStyle) && CurrentTool == PlayerTool.CreateWire)
                    {
                        if (!IsDraggingWire)
                        {
                            // Play sound
                            PickupSound.Play();

                            // Fetch all the details we need and set the player to be dragging
                            IsDraggingWire = true;
                            DraggingOutput = false;
                            GateDraggingFrom = Gates.CurrentGates[i];
                            GateDraggingFromInputID = 1;
                        }
                        else if (IsDraggingWire && DraggingOutput)
                        {
                            // Play sound
                            DropSound.Play();

                            // First stop the player from dragging anymore
                            IsDraggingWire = false;

                            // Then connect this gate to the one we dragged from
                            Gates.CurrentGates[i].Input2Gate = GateDraggingFrom;

                            // And connect the one we dragged from to this gate
                            GateDraggingFrom.OutputGate = Gates.CurrentGates[i];
                            GateDraggingFrom.OGCID = 0;
                        }
                    }
                }
                else
                {
                    // One input
                    float InputX = GatePosition.x - 68f * UIScale;
                    float InputY = GatePosition.y - 14f * UIScale;
                    float InputWidth = 28 * UIScale;

                    if (GUI.Button(new Rect(InputX, InputY, InputWidth, InputWidth), "", InputButtonStyle) && CurrentTool == PlayerTool.CreateWire)
                    {
                        if (!IsDraggingWire)
                        {
                            // Play sound
                            PickupSound.Play();

                            // Fetch all the details we need and set the player to be dragging
                            IsDraggingWire = true;
                            DraggingOutput = false;
                            GateDraggingFrom = Gates.CurrentGates[i];
                            GateDraggingFromInputID = 1;
                        }
                        else if (IsDraggingWire && DraggingOutput)
                        {
                            // Play sound
                            DropSound.Play();

                            // First stop the player from dragging anymore
                            IsDraggingWire = false;

                            // Then connect this gate to the one we dragged from
                            Gates.CurrentGates[i].Input1Gate = GateDraggingFrom;

                            // And connect the one we dragged from to this gate
                            GateDraggingFrom.OutputGate = Gates.CurrentGates[i];
                            GateDraggingFrom.OGCID = 0;
                        }
                    }
                }
            }
        }
    }
}
