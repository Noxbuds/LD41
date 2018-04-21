using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Private reference to level manager
    private LevelManager _LevelManager;
    private Gates _Gates; // local reference to the behaviour version of Gates

    // Details about player clicking/toolbox/etc
    public enum PlayerTool
    {
        PlaceGate, CreateWire, DeleteGate
    }
    public enum GateSelection
    {
        AND, OR, XOR, NAND, NOR, XNOR, NOT
    }
    public PlayerTool CurrentTool;
    public GateSelection SelectedGate;
    public bool MouseInBounds;

    // Wire dragging stuff
    public bool IsDraggingWire;
    public bool DraggingOutput;
    public bool DraggingSource;
    public GateBehaviour GateDraggingFrom; // the gate we started dragging from
    public int GateDraggingFromInputID; // the input ID of the gate we're dragging from
    public Vector2 DraggingOrigin; // the screen co-ordinates that the user started dragging from
    public Material WireMaterial;
    public LineRenderer PreviewLine;

    // Sound
    public AudioSource PickupSound;
    public AudioSource DropSound;

    // UI stuff
    public Texture2D InputButtonTex;
    public Texture2D OutputButtonTex;
    public Texture2D CrossButtonTex;
    private GUIStyle InputButtonStyle;
    private GUIStyle OutputButtonStyle;
    private GUIStyle CrossButtonStyle;
    public Vector2 SourceBasePos;
    public Vector2 OutBasePos;

	// Use this for initialization
	void Start ()
    {
        // Necessary for detecting mouse clicks
        Physics.queriesHitTriggers = true;

        // Get the level manager and gates
        _LevelManager = GameObject.FindObjectOfType<LevelManager>();
        _Gates = GameObject.FindObjectOfType<Gates>();

        // Set the current tool for now
        CurrentTool = PlayerTool.PlaceGate;
        SelectedGate = GateSelection.AND;

        // Setup UI styles
        InputButtonStyle = new GUIStyle();
        InputButtonStyle.normal.background = InputButtonTex;

        OutputButtonStyle = new GUIStyle();
        OutputButtonStyle.normal.background = OutputButtonTex;

        CrossButtonStyle = new GUIStyle();
        CrossButtonStyle.normal.background = CrossButtonTex;

        // Create preview line. Points will be set in the Update and OnGUI methods
        PreviewLine = new GameObject().AddComponent<LineRenderer>();
        PreviewLine.transform.position = Vector2.zero;
        PreviewLine.material = WireMaterial;
        PreviewLine.startWidth = PreviewLine.endWidth = 0.005f;
        PreviewLine.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Preview line stuff
        if (IsDraggingWire)
        {
            // Set active
            PreviewLine.gameObject.SetActive(true);

            // Get some shorter-named variables
            float MouseX = Input.mousePosition.x;
            float MouseY = Screen.height - Input.mousePosition.y;
            float ImageWidth = 14 * (Screen.width / 2560f);

            // Set the points on the line renderer
            Vector2 LRPoint1 = Camera.main.ScreenToWorldPoint(new Vector2(DraggingOrigin.x + ImageWidth / 2f, DraggingOrigin.y - ImageWidth / 2f));
            Vector2 LRPoint2 = Camera.main.ScreenToWorldPoint(new Vector2(MouseX + ImageWidth / 2f, Screen.height - MouseY - ImageWidth / 2f));

            PreviewLine.SetPositions(new Vector3[] { new Vector3(LRPoint1.x, LRPoint1.y, -7), new Vector3(LRPoint2.x, LRPoint2.y, -7) });
            PreviewLine.sortingOrder = 4;
        }
        else
            PreviewLine.gameObject.SetActive(false);

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
            
            // Right-clicking stops dragging a wire
            if (Input.GetMouseButtonUp(1))
            {
                IsDraggingWire = false;
                GateDraggingFrom = null;
                DropSound.Play();
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

            // Render the inputs and outputs
            for (int i = 0; i < _Gates.Inputs.Count; i++)
            {
                
            }

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
                float OutputY = GatePosition.y + 16f * UIScale;
                float CrossY = GatePosition.y - 44f * UIScale;
                float OutputWidth = 28 * UIScale;

                // Toggle the player dragging status if clicked on
                if (GUI.Button(new Rect(OutputX, OutputY, OutputWidth, OutputWidth), "", OutputButtonStyle) && CurrentTool == PlayerTool.CreateWire)
                {
                    // If we aren't already dragging, then set it so we are dragging,
                    // and that we're dragging an output wire
                    if (!IsDraggingWire)
                    {
                        // Play sound
                        PickupSound.Play();

                        // If connected already, delete current connection
                        if (Gates.CurrentGates[i].OutputGate != null)
                        {
                            // Delete current connection
                            if (Gates.CurrentGates[i].OGCID == 0)
                                Gates.CurrentGates[i].OutputGate.Input1Gate = null;
                            else
                                Gates.CurrentGates[i].OutputGate.Input2Gate = null;

                            // Delete wire
                            Destroy(Gates.CurrentGates[i].WireObject.gameObject);
                        }

                        // Pickup wire
                        IsDraggingWire = true;
                        DraggingOutput = true;
                        GateDraggingFrom = Gates.CurrentGates[i];
                        DraggingOrigin = new Vector2(OutputX, Screen.height - OutputY);
                    }
                    else if (IsDraggingWire && !DraggingOutput)
                    {
                        // Play sound
                        DropSound.Play();

                        // However if we are dragging a wire and it's not an output
                        // wire, then connect them
                        IsDraggingWire = false;

                        // Make sure to disconnect other wires
                        if (Gates.CurrentGates[i].OutputGate != null)
                        {
                            // Get rid of the input connection
                            if (Gates.CurrentGates[i].OGCID == 0)
                                Gates.CurrentGates[i].OutputGate.Input1Gate = null;
                            else
                                Gates.CurrentGates[i].OutputGate.Input2Gate = null;

                            // Get rid of the output connection
                            Gates.CurrentGates[i].OutputGate = null;

                            // Get rid of the wire
                            Destroy(Gates.CurrentGates[i].WireObject.gameObject);
                        }

                        // Connect this gate to the one we dragged from
                        Gates.CurrentGates[i].OutputGate = GateDraggingFrom;
                        Gates.CurrentGates[i].OGCID = GateDraggingFromInputID;

                        // Connect the one we dragged from to this gate
                        if (GateDraggingFromInputID == 0)
                            GateDraggingFrom.Input1Gate = Gates.CurrentGates[i];
                        else
                            GateDraggingFrom.Input2Gate = Gates.CurrentGates[i];

                        // Create wire
                        CreateWire(OutputWidth, OutputX, OutputY, i, 2);
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
                            DraggingOrigin = new Vector2(InputX, Screen.height - InputY1);
                        }
                        else if (IsDraggingWire && DraggingOutput && Gates.CurrentGates[i] != GateDraggingFrom)
                        {
                            // Play sound
                            DropSound.Play();

                            // First stop the player from dragging anymore
                            IsDraggingWire = false;

                            // Make sure to disconnect other wires
                            if (Gates.CurrentGates[i].Input1Gate != null)
                            {
                                // Get rid of the output connection
                                Gates.CurrentGates[i].Input1Gate.OutputGate = null;

                                // Get rid of the wire
                                Destroy(Gates.CurrentGates[i].Input1Gate.WireObject.gameObject);

                                // Get rid of the input connection
                                Gates.CurrentGates[i].Input1Gate = null;
                            }

                            // Then connect this gate to the one we dragged from
                            Gates.CurrentGates[i].Input1Gate = GateDraggingFrom;

                            // And connect the one we dragged from to this gate
                            GateDraggingFrom.OutputGate = Gates.CurrentGates[i];
                            GateDraggingFrom.OGCID = 0;

                            // Create wire
                            CreateWire(InputWidth, InputX, InputY1, i, 0);
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
                            DraggingOrigin = new Vector2(InputX, Screen.height - InputY2);
                        }
                        else if (IsDraggingWire && DraggingOutput && Gates.CurrentGates[i] != GateDraggingFrom)
                        {
                            // Play sound
                            DropSound.Play();

                            // First stop the player from dragging anymore
                            IsDraggingWire = false;

                            // Make sure to disconnect other wires
                            if (Gates.CurrentGates[i].Input2Gate != null)
                            {
                                // Get rid of the output connection
                                Gates.CurrentGates[i].Input2Gate.OutputGate = null;

                                // Get rid of the wire
                                Destroy(Gates.CurrentGates[i].Input2Gate.WireObject.gameObject);

                                // Get rid of the input connection
                                Gates.CurrentGates[i].Input2Gate = null;
                            }

                            // Then connect this gate to the one we dragged from
                            Gates.CurrentGates[i].Input2Gate = GateDraggingFrom;

                            // And connect the one we dragged from to this gate
                            GateDraggingFrom.OutputGate = Gates.CurrentGates[i];
                            GateDraggingFrom.OGCID = 1;

                            // Create wire
                            CreateWire(InputWidth, InputX, InputY2, i, 1);
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
                        // If the user right clicks on a terminal, delete the connection
                        if (Input.GetMouseButton(1))
                        {
                            // Sever inbound connection
                            Gates.CurrentGates[i].Input1Gate.OutputGate = null;

                            // Sever outbound connection
                            Gates.CurrentGates[i].Input1Gate = null;

                            // Break in case
                            break;
                        }

                        if (!IsDraggingWire)
                        {
                            // Play sound
                            PickupSound.Play();

                            // Fetch all the details we need and set the player to be dragging
                            IsDraggingWire = true;
                            DraggingOutput = false;
                            GateDraggingFrom = Gates.CurrentGates[i];
                            GateDraggingFromInputID = 1;
                            DraggingOrigin = new Vector2(InputX, Screen.height - InputY);
                        }
                        else if (IsDraggingWire && DraggingOutput && Gates.CurrentGates[i] != GateDraggingFrom)
                        {
                            // Play sound
                            DropSound.Play();

                            // First stop the player from dragging anymore
                            IsDraggingWire = false;

                            // Make sure to disconnect other wires
                            if (Gates.CurrentGates[i].Input1Gate != null)
                            {
                                // Get rid of the output connection
                                Gates.CurrentGates[i].Input1Gate.OutputGate = null;

                                // Get rid of the wire
                                Destroy(Gates.CurrentGates[i].OutputGate.WireObject);

                                // Get rid of the input connection
                                Gates.CurrentGates[i].Input1Gate = null;
                            }

                            // Then connect this gate to the one we dragged from
                            Gates.CurrentGates[i].Input1Gate = GateDraggingFrom;

                            // And connect the one we dragged from to this gate
                            GateDraggingFrom.OutputGate = Gates.CurrentGates[i];
                            GateDraggingFrom.OGCID = 0;

                            // Create wire
                            CreateWire(InputWidth, InputX, InputY, i, 0);
                        }
                    }
                }

                // Gate delete button
                if (GUI.Button(new Rect(OutputX, CrossY, OutputWidth, OutputWidth), "", CrossButtonStyle))
                {
                    Gates.DestroyGate(Gates.GetGateID(Gates.CurrentGates[i]));
                }
            }
        }
    }
    
    /// <summary>
    /// Creates a line renderer between two gates
    /// </summary>
    void CreateWire(float ImageWidth, float ButtonX, float ButtonY, int GateIndex, int ConnectionType)
    {
        // Create the wire object
        GameObject Wire = new GameObject();
        Wire.transform.position = Gates.CurrentGates[GateIndex].transform.position;
        Wire.gameObject.name = "wire";

        // Assign the wire object in the behaviour script
        if (ConnectionType == 2)
        {
            Wire.transform.parent = Gates.CurrentGates[GateIndex].transform;
        }
        else if (ConnectionType == 0)
        {
            Wire.transform.parent = Gates.CurrentGates[GateIndex].Input1Gate.transform;
        }
        else if (ConnectionType == 1)
        {
            Wire.transform.parent = Gates.CurrentGates[GateIndex].Input2Gate.transform;
        }

        // Line renderer
        LineRenderer WireLine = Wire.AddComponent<LineRenderer>();
        WireLine.sortingOrder += 10;
        WireLine.startWidth = WireLine.endWidth = 0.005f;
        WireLine.material = WireMaterial;

        // Fix the dragging origin
        DraggingOrigin.x += ImageWidth / 2f;
        DraggingOrigin.y -= ImageWidth / 2f;

        // Set the points on the line renderer
        Vector2 LRPoint1 = Camera.main.ScreenToWorldPoint(DraggingOrigin);
        Vector2 LRPoint2 = Camera.main.ScreenToWorldPoint(new Vector2(ButtonX + ImageWidth / 2f, Screen.height - ButtonY - ImageWidth / 2f));

        WireLine.SetPositions(new Vector3[] { new Vector3(LRPoint1.x, LRPoint1.y, -5), new Vector3(LRPoint2.x, LRPoint2.y, -5) });

        // Assign the wire object in the behaviour script
        if (ConnectionType == 2)
            Gates.CurrentGates[GateIndex].WireObject = WireLine;
        else if (ConnectionType == 0)
            Gates.CurrentGates[GateIndex].Input1Gate.WireObject = WireLine;
        else if (ConnectionType == 1)
            Gates.CurrentGates[GateIndex].Input2Gate.WireObject = WireLine;
    }
}
