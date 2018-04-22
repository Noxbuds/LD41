using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    // Level manager. Handles various things, spawning prefabs, moving
    // the view between ship and panel view, probably more.

    // An enumerator for panel sizes to make it easier
    public enum EPanelSize
    {
        Large, Medium, Small
    }

    // The level number
    public int LevelNumber;
    public bool ViewingPanel;

    // Arena boundaries
    // These are positions relative to the x/y-axis
    public float BoundsLeft;
    public float BoundsUp;
    public float BoundsRight;
    public float BoundsDown;

    // Properties of the panel
    public GameObject LargePanel;
    public GameObject MediumPanel;
    public GameObject SmallPanel;
    public EPanelSize PanelSize;
    public GameObject SwitchPrefab;
    public bool PowerFlowing; // whether the switch is on or off

    // Other sprites
    public GameObject PanelBackground;

    // Ship prefabs
    public GameObject PlayerShipPrefab;
    public GameObject EnemyShipPrefab;
    public int EnemyCount;

    // Positions to spawn the switch
    public Vector2 SmallSwitchPos;
    public Vector2 MediumSwitchPos;
    public Vector2 LargeSwitchPos;

    // Local references
    private List<GameObject> BGPanelObjs;
    private GameObject FGPanelObj;
    private GameObject SwitchObj;
    private GameObject PanelsRoot;
    private Player ThePlayer;
    private Ship PlayerShip;
    private List<Ship> EnemyShips;
    private Gates _Gates;

	// Use this for initialization
	void Start ()
    {
		// Setup the level
        ViewingPanel = true;

        // Get gates reference
        _Gates = FindObjectOfType<Gates>();

        // Get the player
        ThePlayer = GameObject.FindObjectOfType<Player>();

        // Setup ships
        PlayerShip = Instantiate(PlayerShipPrefab, new Vector2((BoundsRight - BoundsLeft) / 2f, (BoundsUp - BoundsDown) / 2f), new Quaternion(0, 0, 0, 0)).GetComponent<Ship>();
        PlayerShip.transform.SetParent(ThePlayer.transform);

        // New level
        NewLevel(2);

        // Stop viewing panel
        ViewingPanel = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
		// "Animate" the switch
        if (PowerFlowing)
        {
            SwitchObj.GetComponent<Animator>().Play("Panel Switch On");
        }
        else
        {
            SwitchObj.GetComponent<Animator>().Play("Panel Switch Off");
        }

        // Move the camera as necessary
        if (ViewingPanel)
        {
            // Set the camera position
            Camera.main.transform.position = new Vector3(0, 0.06f, -10);

            // Set camera orthographic size
            if (PanelSize == EPanelSize.Large)
                Camera.main.orthographicSize = 0.3f;
            else
                Camera.main.orthographicSize = 0.24f;
        }
        else
        {
            // Then we're viewing the ship. Make the camera follow it, and set ortho size lower
            // Orthographic size first. 1 is good, but a zooming thing would be better. Think SC2...
            Camera.main.orthographicSize = 1;

            // Make the camera follow the ship
            Camera.main.transform.position = new Vector3(PlayerShip.transform.position.x, PlayerShip.transform.position.y, -10);
        }
	}

    /// <summary>
    /// Sets a new level
    /// </summary>
    /// <param name="NextComponent">The ID of the next component to be targetted</param>
    public void NewLevel(int NextComponent)
    {
        // Save code

        // Setup things
        // Get the component details
        Ship.ShipComponent Component = PlayerShip.AllComponents[NextComponent];

        // Setup inputs and outputs
        _Gates.InputCount = Component.InputCount;
        _Gates.OutputCount = Component.OutputCount;
        _Gates.Initialise();
        PanelSize = Component.BoardSize;

        // Tell the ship
        PlayerShip.CurrentComponentId = NextComponent;

        // Now setup the panels
        // Tidy up a bit
        PanelsRoot = new GameObject();
        PanelsRoot.transform.position = new Vector3(0, 0, 5);
        PanelsRoot.name = "Panel Root Object";

        // Initialise the BG Panel Objects list
        BGPanelObjs = new List<GameObject>();

        // First spawn in the background panels
        BGPanelObjs.Add(Instantiate(PanelBackground));
        BGPanelObjs.Add(Instantiate(PanelBackground, new Vector2(-0.64f, 0), new Quaternion(0, 0, 0, 0)));
        BGPanelObjs.Add(Instantiate(PanelBackground, new Vector2(0.64f, 0), new Quaternion(0, 0, 0, 0)));

        for (int i = 0; i < BGPanelObjs.Count; i++)
        {
            BGPanelObjs[i].transform.parent = PanelsRoot.transform;
        }

        // Then the panel, switch, various other things that depend on panel size
        // Use a switch statement and kill many stones with one bird ;)
        switch (PanelSize)
        {
            case EPanelSize.Large:
                // Create the foreground panel
                FGPanelObj = Instantiate(LargePanel);
                FGPanelObj.name = "large panel";
                FGPanelObj.transform.parent = PanelsRoot.transform;

                // Create the switch
                SwitchObj = Instantiate(SwitchPrefab, LargeSwitchPos, new Quaternion(0, 0, 0, 0));
                SwitchObj.name = "panel switch";
                SwitchObj.transform.parent = PanelsRoot.transform;

                // Setup the player's base positions for inputs+outputs
                ThePlayer.SourceBasePos = Camera.main.WorldToScreenPoint(new Vector2(-0.24f, 0.19f));
                ThePlayer.OutBasePos = Camera.main.WorldToScreenPoint(new Vector2(0.22f, 0.19f));
                Camera.main.orthographicSize = 0.3f;

                // Flip the base positions' y-coordinates
                ThePlayer.SourceBasePos.y = Screen.height - ThePlayer.SourceBasePos.y;
                ThePlayer.OutBasePos.y = Screen.height - ThePlayer.OutBasePos.y;
                break;
            case EPanelSize.Medium:
                // Create the foreground panel
                FGPanelObj = Instantiate(MediumPanel);
                FGPanelObj.name = "medium panel";
                FGPanelObj.transform.parent = PanelsRoot.transform;

                // Create the switch
                SwitchObj = Instantiate(SwitchPrefab, MediumSwitchPos, new Quaternion(0, 0, 0, 0));
                SwitchObj.name = "panel switch";
                SwitchObj.transform.parent = PanelsRoot.transform;

                ThePlayer.SourceBasePos = Camera.main.WorldToScreenPoint(new Vector2(-0.24f, 0.1f));
                ThePlayer.OutBasePos = Camera.main.WorldToScreenPoint(new Vector2(0.22f, 0.1f));
                Camera.main.orthographicSize = 0.24f;

                ThePlayer.SourceBasePos.y = Screen.height - ThePlayer.SourceBasePos.y;
                ThePlayer.OutBasePos.y = Screen.height - ThePlayer.OutBasePos.y;
                break;
            case EPanelSize.Small:
                // Create the foreground panel
                FGPanelObj = Instantiate(SmallPanel);
                FGPanelObj.name = "small panel";
                FGPanelObj.transform.parent = PanelsRoot.transform;

                // Create the switch
                SwitchObj = Instantiate(SwitchPrefab, SmallSwitchPos, new Quaternion(0, 0, 0, 0));
                SwitchObj.name = "panel switch";
                SwitchObj.transform.parent = PanelsRoot.transform;

                // Setup the player's base positions for inputs+outputs
                ThePlayer.SourceBasePos = Camera.main.WorldToScreenPoint(new Vector2(-0.15f, 0.1f));
                ThePlayer.OutBasePos = Camera.main.WorldToScreenPoint(new Vector2(0.15f, 0.1f));
                Camera.main.orthographicSize = 0.24f;

                // Flip the base positions' y-coordinates
                ThePlayer.SourceBasePos.y = Screen.height - ThePlayer.SourceBasePos.y;
                ThePlayer.OutBasePos.y = Screen.height - ThePlayer.OutBasePos.y;
                break;
        }
    }
}
