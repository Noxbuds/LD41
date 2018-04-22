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
        PlaceGate, CreateWire
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
    public bool DraggingEnd;
    public GateBehaviour GateDraggingFrom; // the gate we started dragging from
    public Gates.SourceConnection SourceDraggingFrom; // source/end dragging connection
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
    public Texture2D ToolboxImage;
    public Texture2D ToolboxButtonImage;
    private GUIStyle ToolboxButtonStyle;
    private GUIStyle ToolMiscStyle;
    public Texture2D[] ToolHoverImages;
    public Texture2D WireToolIcon;
    public Texture2D ShipIcon;

	// Use this for initialization
	void Start ()
    {
        if (ToolHoverImages == null)
            Debug.LogError("GateHoverImages is null");

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

        ToolboxButtonStyle = new GUIStyle();
        ToolboxButtonStyle.normal.background = ToolboxButtonImage;

        ToolMiscStyle = new GUIStyle();

        // Create preview line. Points will be set in the Update and OnGUI methods
        PreviewLine = new GameObject().AddComponent<LineRenderer>();
        PreviewLine.transform.position = Vector2.zero;
        PreviewLine.material = WireMaterial;
        PreviewLine.startWidth = PreviewLine.endWidth = 0.005f;
        PreviewLine.startColor = PreviewLine.endColor = GateBehaviour.UnpoweredColour;
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

        // Handle viewing panel stuff
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

    /// <summary>
    /// Converts a logic gate's name into a gate selection
    /// </summary>
    /// <returns></returns>
    GateSelection GateSelectionFromGateName(string GateName)
    {
        switch (GateName)
        {
            default:
            case "AND":
                return GateSelection.AND;
            case "OR":
                return GateSelection.OR;
            case "XOR":
                return GateSelection.XOR;
            case "NAND":
                return GateSelection.NAND;
            case "NOR":
                return GateSelection.NOR;
            case "XNOR":
                return GateSelection.XNOR;
            case "NOT":
                return GateSelection.NOT;
        }
    }

    /// <summary>
    /// Handles the buttons for toolbox elements
    /// </summary>
    void HandleToolClick(bool IsGate, string ToolName, float UIScale, float ToolBaseX, int Position, Texture2D ToolImage)
    {
        // Draw the glowy pad thing
        GUI.DrawTexture(new Rect(ToolBaseX + 240f * UIScale * Position, 30f * UIScale, 240f * UIScale, 240f * UIScale), ToolboxButtonImage);

        // Assign the tool misc style's texture to the logic gate
        ToolMiscStyle.normal.background = ToolImage;
        ToolMiscStyle.hover.background = ToolHoverImages[Position];
        ToolMiscStyle.active.background = ToolImage;

        // Draw button
        if (GUI.Button(new Rect(ToolBaseX + 40f * UIScale + 240f * UIScale * Position, 70f * UIScale, 160f * UIScale, 160f * UIScale), "", ToolMiscStyle))
        {
            // If we're clicking a gate, select it
            if (IsGate)
            {
                SelectedGate = GateSelectionFromGateName(ToolName);
                CurrentTool = PlayerTool.PlaceGate;
            }
            else
            {
                // Otherwise select a tool
                if (ToolName == "Wire")
                    CurrentTool = PlayerTool.CreateWire;

                // Return to ship
                if (ToolName == "Ship")
                    _LevelManager.ViewingPanel = false;
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
            // First the tool box itself
            GUI.DrawTexture(new Rect(0, 0, 2560f * UIScale, 360f * UIScale), ToolboxImage);

            // Then the tools. Loop through the 'all logic gates' list
            float ToolImageWidth = 240f * UIScale;
            float ToolBaseX = 50f * UIScale;

            // AND Gate
            HandleToolClick(true, "AND", UIScale, ToolBaseX, 0, Gates.Gate_AND.GateSprite.texture);

            // OR Gate
            HandleToolClick(true, "OR", UIScale, ToolBaseX, 1, Gates.Gate_OR.GateSprite.texture);

            // XOR Gate
            HandleToolClick(true, "XOR", UIScale, ToolBaseX, 2, Gates.Gate_XOR.GateSprite.texture);

            // NAND Gate
            HandleToolClick(true, "NAND", UIScale, ToolBaseX, 3, Gates.Gate_NAND.GateSprite.texture);

            // NOR Gate
            HandleToolClick(true, "NOR", UIScale, ToolBaseX, 4, Gates.Gate_NOR.GateSprite.texture);

            // XNOR Gate
            HandleToolClick(true, "XNOR", UIScale, ToolBaseX, 5, Gates.Gate_XNOR.GateSprite.texture);

            // NOT Gate
            HandleToolClick(true, "NOT", UIScale, ToolBaseX, 6, Gates.Gate_NOT.GateSprite.texture);

            // Wire tool
            HandleToolClick(false, "Wire", UIScale, ToolBaseX, 7, WireToolIcon);

            // Return to ship
            HandleToolClick(false, "Ship", UIScale, ToolBaseX, 8, ShipIcon);

            // Render tooltips
            // Get this done if, and only if, you have time!

            // Render the inputs and outputs
            for (int i = 0; i < _Gates.InputCount; i++)
            {
                // Calculate positions
                float ButtonX = SourceBasePos.x;
                float ButtonY = SourceBasePos.y + 90f * i * UIScale;
                float ButtonWidth = 28 * UIScale;

                // Draw the button. Stay consistent in the colour and icons as the rest of the gates
                if (GUI.Button(new Rect(ButtonX, ButtonY, ButtonWidth, ButtonWidth), "", OutputButtonStyle) && CurrentTool == PlayerTool.CreateWire)
                {
                    // If we aren't already dragging, make it so
                    if (!IsDraggingWire)
                    {
                        // Play sound
                        PickupSound.Play();

                        // Pickup wire
                        IsDraggingWire = true;
                        DraggingOutput = false;
                        DraggingSource = true;
                        DraggingEnd = false;
                        GateDraggingFrom = null;
                        SourceDraggingFrom = _Gates.InputConnections[i];
                        DraggingOrigin = new Vector2(ButtonX, Screen.height - ButtonY);
                    }
                    else if (IsDraggingWire && !DraggingOutput && !DraggingSource && !DraggingEnd)
                    {
                        // Play sound
                        DropSound.Play();

                        // Stop dragging wire
                        IsDraggingWire = false;

                        // Connect the source
                        _Gates.InputConnections[i].Gates.Add(GateDraggingFrom);
                        _Gates.InputConnections[i].GateInputIDs.Add(GateDraggingFromInputID);

                        // Connect the gate
                        GateDraggingFrom.Sources[GateDraggingFromInputID] = _Gates.InputConnections[i];

                        // Create a wire
                        LineRenderer NewWire = CreateSourceEndWire(true, ButtonWidth, ButtonX, ButtonY);

                        // Parent it to the gate
                        NewWire.transform.parent = GateDraggingFrom.transform;

                        // Assign it to the gate
                        if (DraggingOutput)
                        {
                            // Assign it as an end wire
                            GateDraggingFrom.EndWire = NewWire;
                        }
                        else
                        {
                            // Assign it as a source wire
                            GateDraggingFrom.SourceWires[GateDraggingFromInputID] = NewWire;
                        }
                    }
                }
            }

            // Render outputs
            for (int i = 0; i < _Gates.OutputCount; i++)
            {
                // Calculate positions
                float ButtonX = OutBasePos.x;
                float ButtonY = OutBasePos.y + 90f * i * UIScale;
                float ButtonWidth = 28 * UIScale;

                // Draw the button. Stay consistent in the colour and icons as the rest of the gates
                if (GUI.Button(new Rect(ButtonX, ButtonY, ButtonWidth, ButtonWidth), "", InputButtonStyle) && CurrentTool == PlayerTool.CreateWire)
                {
                    // If we aren't already dragging, make it so
                    if (!IsDraggingWire)
                    {
                        // Play sound
                        PickupSound.Play();

                        // Pickup wire
                        IsDraggingWire = true;
                        DraggingOutput = false;
                        DraggingSource = false;
                        DraggingEnd = true;
                        GateDraggingFrom = null;
                        SourceDraggingFrom = _Gates.OutputConnections[i];
                        DraggingOrigin = new Vector2(ButtonX, Screen.height - ButtonY);
                    }
                    else if (IsDraggingWire && DraggingOutput && !DraggingSource && !DraggingEnd)
                    {
                        // Sever connections first if the output is connected already
                        if (_Gates.OutputConnections[i].Gates.Count > 0)
                        {
                            _Gates.OutputConnections[i].RemoveAllConnections();
                        }

                        // Play sound
                        DropSound.Play();

                        // Stop dragging wire
                        IsDraggingWire = false;

                        // Connect the source
                        _Gates.OutputConnections[i].Gates.Add(GateDraggingFrom);
                        _Gates.OutputConnections[i].GateInputIDs.Add(GateDraggingFromInputID);

                        // Connect the gate
                        GateDraggingFrom.EndConnection = _Gates.OutputConnections[i];

                        // Create a wire
                        LineRenderer NewWire = CreateSourceEndWire(false, ButtonWidth, ButtonX, ButtonY);

                        // Parent it to the gate
                        NewWire.transform.parent = GateDraggingFrom.transform;

                        // Assign it as an end wire
                        GateDraggingFrom.EndWire = NewWire;
                    }
                }
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
                    HandleOutputClicking(OutputX, OutputY, OutputWidth, i);
                }

                // Now draw the input buttons
                if (!GateList[i].LogicGate.HasOneInput)
                {
                    // Two inputs
                    float InputX = GatePosition.x - 68f * UIScale;
                    float InputY1 = GatePosition.y - 44f * UIScale;
                    float InputY2 = GatePosition.y + 16f * UIScale;
                    float InputWidth = 28 * UIScale;

                    // Handle clicking (button 0)
                    HandleInputClicking(InputX, InputY1, InputWidth, i, 0);

                    // Handle clicking (button 1)
                    HandleInputClicking(InputX, InputY2, InputWidth, i, 1);
                }
                else
                {
                    // One input
                    float InputX = GatePosition.x - 68f * UIScale;
                    float InputY = GatePosition.y - 14f * UIScale;
                    float InputWidth = 28 * UIScale;

                    // Handle clicking
                    HandleInputClicking(InputX, InputY, InputWidth, i, 0);
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
    /// Handles output terminals
    /// </summary>
    void HandleOutputClicking(float ButtonX, float ButtonY, float ButtonWidth, int Id)
    {
        // If we aren't already dragging, then set it so we are dragging,
        // and that we're dragging an output wire
        if (!IsDraggingWire)
        {
            // Play sound
            PickupSound.Play();

            // If connected already, delete current connection
            if (Gates.CurrentGates[Id].OutputGate != null)
            {
                // Delete current connection
                Gates.CurrentGates[Id].OutputGate.InputGates[Gates.CurrentGates[Id].OGCID] = null;

                // Remove output gate connection
                Gates.CurrentGates[Id].OutputGate = null;

                // Delete wire
                Destroy(Gates.CurrentGates[Id].WireObject.gameObject);
            }

            // If end-connected already, delete connection
            if (Gates.CurrentGates[Id].EndConnection != null)
            {
                Gates.CurrentGates[Id].EndConnection.RemoveConnections(Gates.CurrentGates[Id].EndConnection.GetGateID(Gates.CurrentGates[Id]));
            }

            // Pickup wire
            IsDraggingWire = true;
            DraggingOutput = true;
            DraggingSource = false;
            DraggingEnd = false;
            GateDraggingFrom = Gates.CurrentGates[Id];
            GateDraggingFromInputID = 2; // 2 = outputs only, not really needed I suppose...
            DraggingOrigin = new Vector2(ButtonX, Screen.height - ButtonY);
        }
        else if (IsDraggingWire && !DraggingOutput && Gates.CurrentGates[Id] != GateDraggingFrom && !DraggingSource && !DraggingEnd)
        {
            // Play sound
            DropSound.Play();

            // However if we are dragging a wire and it's not an output
            // wire, then connect them
            IsDraggingWire = false;

            // Make sure to disconnect other wires
            if (Gates.CurrentGates[Id].OutputGate != null)
            {
                // Get rid of the input connection
                Gates.CurrentGates[Id].OutputGate.InputGates[Gates.CurrentGates[Id].OGCID] = null;

                // Get rid of the output connection
                Gates.CurrentGates[Id].OutputGate = null;

                // Get rid of the wire
                Destroy(Gates.CurrentGates[Id].WireObject.gameObject);
            }

            // If end-connected already, delete connection
            if (Gates.CurrentGates[Id].EndConnection != null)
            {
                Gates.CurrentGates[Id].EndConnection.RemoveConnections(Gates.CurrentGates[Id].EndConnection.GetGateID(Gates.CurrentGates[Id]));
            }

            // Connect this gate to the one we dragged from
            Gates.CurrentGates[Id].OutputGate = GateDraggingFrom;
            Gates.CurrentGates[Id].OGCID = GateDraggingFromInputID;

            // Connect the one we dragged from to this gate
            GateDraggingFrom.InputGates[GateDraggingFromInputID] = Gates.CurrentGates[Id];

            // Create wire
            CreateWire(ButtonWidth, ButtonX, ButtonY, Id, 2);
        }
        else if (IsDraggingWire && DraggingEnd)
        {
            // Play sound
            DropSound.Play();

            // Stop dragging wire
            IsDraggingWire = false;
            DraggingEnd = false;
            DraggingSource = false;
            DraggingOutput = false;

            // Make sure to disconnect other wires
            if (Gates.CurrentGates[Id].OutputGate != null)
            {
                // Get rid of the input connection
                Gates.CurrentGates[Id].OutputGate.InputGates[Gates.CurrentGates[Id].OGCID] = null;

                // Get rid of the output connection
                Gates.CurrentGates[Id].OutputGate = null;

                // Get rid of the wire
                Destroy(Gates.CurrentGates[Id].WireObject.gameObject);
            }

            // If end-connected already, delete connection
            if (Gates.CurrentGates[Id].EndConnection != null)
            {
                Gates.CurrentGates[Id].EndConnection.RemoveConnections(Gates.CurrentGates[Id].EndConnection.GetGateID(Gates.CurrentGates[Id]));
            }

            // Connect this gate to the end
            Gates.CurrentGates[Id].EndConnection = SourceDraggingFrom;

            // Connect end to the gate
            SourceDraggingFrom.Gates.Add(Gates.CurrentGates[Id]);
            SourceDraggingFrom.GateInputIDs.Add(2);

            // Create a wire
            LineRenderer NewWire = CreateSourceEndWire(false, ButtonWidth, ButtonX, ButtonY);

            // Parent it to the gate
            NewWire.transform.parent = Gates.CurrentGates[Id].transform;

            // Assign it as an end wire
            Gates.CurrentGates[Id].EndWire = NewWire;
        }
    }

    /// <summary>
    /// Handles the input terminals
    /// </summary>
    void HandleInputClicking(float ButtonX, float ButtonY, float ButtonWidth, int Id, int ConnectionID)
    {
        // Handle wire dragging
        if (GUI.Button(new Rect(ButtonX, ButtonY, ButtonWidth, ButtonWidth), "", InputButtonStyle) && CurrentTool == PlayerTool.CreateWire)
        {
            if (!IsDraggingWire)
            {
                // Play sound
                PickupSound.Play();

                // If source connected already, sever that connection
                // If source-connected already, delete connection
                if (Gates.CurrentGates[Id].Sources[ConnectionID] != null && Gates.CurrentGates[Id].Sources[ConnectionID] != Gates.CurrentGates[Id].EndConnection)
                {
                    Gates.CurrentGates[Id].Sources[ConnectionID].RemoveConnections(Gates.CurrentGates[Id].Sources[ConnectionID].GetGateID(Gates.CurrentGates[Id]));
                }

                // Fetch all the details we need and set the player to be dragging
                IsDraggingWire = true;
                DraggingOutput = false;
                GateDraggingFrom = Gates.CurrentGates[Id];
                GateDraggingFromInputID = ConnectionID;
                DraggingOrigin = new Vector2(ButtonX, Screen.height - ButtonY);
            }
            else if (IsDraggingWire && DraggingOutput && Gates.CurrentGates[Id] != GateDraggingFrom && !DraggingSource && !DraggingEnd)
            {
                // Play sound
                DropSound.Play();

                // First stop the player from dragging anymore
                IsDraggingWire = false;
                DraggingEnd = false;
                DraggingSource = false;

                // Make sure to disconnect other wires
                if (Gates.CurrentGates[Id].InputGates[ConnectionID] != null)
                {
                    // Get rid of the output connection
                    Gates.CurrentGates[Id].InputGates[ConnectionID].OutputGate = null;

                    // Get rid of the wire
                    Destroy(Gates.CurrentGates[Id].InputGates[ConnectionID].WireObject.gameObject);

                    // Get rid of the input connection
                    Gates.CurrentGates[Id].InputGates[ConnectionID] = null;
                }

                // Then connect this gate to the one we dragged from
                Gates.CurrentGates[Id].InputGates[ConnectionID] = GateDraggingFrom;

                // And connect the one we dragged from to this gate
                GateDraggingFrom.OutputGate = Gates.CurrentGates[Id];
                GateDraggingFrom.OGCID = ConnectionID;

                // Create wire
                CreateWire(ButtonWidth, ButtonX, ButtonY, Id, ConnectionID);
            }
            else if (IsDraggingWire && DraggingSource)
            {
                // Play sound
                DropSound.Play();

                // Stop dragging wire
                IsDraggingWire = false;
                DraggingEnd = false;
                DraggingSource = false;

                // Make sure to disconnect other wires
                if (Gates.CurrentGates[Id].InputGates[ConnectionID] != null)
                {
                    // Get rid of the input connection
                    Gates.CurrentGates[Id].OutputGate.InputGates[ConnectionID] = null;

                    // Get rid of the output connection
                    Gates.CurrentGates[Id].InputGates[ConnectionID] = null;

                    // Get rid of the wire
                    Destroy(Gates.CurrentGates[Id].InputGates[ConnectionID].WireObject);
                }

                // If source-connected already, delete connection
                if (Gates.CurrentGates[Id].Sources[ConnectionID] != null)
                {
                    Gates.CurrentGates[Id].Sources[ConnectionID].RemoveConnections(Gates.CurrentGates[Id].Sources[ConnectionID].GetGateID(Gates.CurrentGates[Id]));
                }

                // Connect this gate to the source
                Gates.CurrentGates[Id].Sources[ConnectionID] = SourceDraggingFrom;

                // Connect end to the gate
                SourceDraggingFrom.Gates.Add(Gates.CurrentGates[Id]);
                SourceDraggingFrom.GateInputIDs.Add(ConnectionID);

                // Create a wire
                LineRenderer NewWire = CreateSourceEndWire(true, ButtonWidth, ButtonX, ButtonY);

                // Parent it to the gate
                NewWire.transform.parent = Gates.CurrentGates[Id].transform;

                // Assign it as an end wire
                Gates.CurrentGates[Id].SourceWires[ConnectionID] = NewWire;
            }
        }
    }

    /// <summary>
    /// Creates a line renderer between two gates
    /// </summary>
    LineRenderer CreateWire(float ImageWidth, float ButtonX, float ButtonY, int GateIndex, int ConnectionType)
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
        else
        {
            Wire.transform.parent = Gates.CurrentGates[GateIndex].InputGates[ConnectionType].transform;
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
        else
            Gates.CurrentGates[GateIndex].InputGates[ConnectionType].WireObject = WireLine;

        // Return line renderer
        return WireLine;
    }

    /// <summary>
    /// Creates a line renderer between a gate and source/end
    /// </summary>
    LineRenderer CreateSourceEndWire(bool IsSource, float ImageWidth, float ButtonX, float ButtonY)
    {
        // Create the wire object
        GameObject Wire = new GameObject();
        Wire.transform.position = Camera.main.ScreenToWorldPoint(new Vector2(ButtonX, ButtonY));
        Wire.gameObject.name = IsSource ? "source wire" : "end wire";

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

        // Return it
        return WireLine;
    }
}
