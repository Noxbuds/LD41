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

    // The 'next level' flag is for determining if the next
    // shot the player receives causes a new level
    public bool NextLevelFlag;

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

    // Camera stuff
    public float CameraShake;

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
    private Ship EnemyShip;
    private Gates _Gates;
    private LevelUI _LevelUI;
    private Transform StarBackground;

	// Use this for initialization
	void Start ()
    {
		// Setup the level
        ViewingPanel = true;
        
        // Next level flag starts as true
        NextLevelFlag = true;

        // Get the star background
        StarBackground = Camera.main.transform.GetChild(0);

        // Get the level ui manager
        _LevelUI = GetComponent<LevelUI>();

        // Set level number to -1 since it'll be incremented in NewLevel()
        LevelNumber = -1;

        // Get gates reference
        _Gates = FindObjectOfType<Gates>();

        // Get the player
        ThePlayer = GameObject.FindObjectOfType<Player>();

        // Setup player ship
        PlayerShip = Instantiate(PlayerShipPrefab, new Vector2((BoundsRight - BoundsLeft) / 2f, (BoundsUp - BoundsDown) / 2f), new Quaternion(0, 0, 0, 0)).GetComponent<Ship>();
        PlayerShip.transform.SetParent(ThePlayer.transform);
        PlayerShip.gameObject.name = "Player Ship";

        // Set bounds
        PlayerShip.BoundsLeft = BoundsLeft;
        PlayerShip.BoundsRight = BoundsRight;
        PlayerShip.BoundsUp = BoundsUp;
        PlayerShip.BoundsDown = BoundsDown;

        // Spawn the enemy
        SpawnEnemy();

        // New level
        NewLevel(0);

        // Stop viewing panel
        ViewingPanel = false;
	}
	
    /// <summary>
    /// Recalculates screen positions for terminals
    /// </summary>
    public void RecalculateUI()
    {
        // Set the camera position. This is necessary for re-calculating the positions
        Camera.main.transform.SetParent(ThePlayer.transform);
        Camera.main.transform.position = new Vector3(0, 0.06f, -10);

        // Set camera orthographic size
        if (PanelSize == EPanelSize.Large)
            Camera.main.orthographicSize = 0.3f;
        else
            Camera.main.orthographicSize = 0.24f;

        // Positions depend on panel size..
        switch (PanelSize)
        {
            case EPanelSize.Large:
                // Setup the player's base positions for inputs+outputs
                ThePlayer.SourceBasePos = Camera.main.WorldToScreenPoint(new Vector2(-0.24f, 0.19f));
                ThePlayer.OutBasePos = Camera.main.WorldToScreenPoint(new Vector2(0.22f, 0.19f));
                Camera.main.orthographicSize = 0.3f;

                // Flip the base positions' y-coordinates
                ThePlayer.SourceBasePos.y = Screen.height - ThePlayer.SourceBasePos.y;
                ThePlayer.OutBasePos.y = Screen.height - ThePlayer.OutBasePos.y;
                break;
            case EPanelSize.Medium:
                // Setup the player's base positions for inputs+outputs
                ThePlayer.SourceBasePos = Camera.main.WorldToScreenPoint(new Vector2(-0.24f, 0.1f));
                ThePlayer.OutBasePos = Camera.main.WorldToScreenPoint(new Vector2(0.22f, 0.1f));
                Camera.main.orthographicSize = 0.24f;

                // Flip the base positions' y-coordinates
                ThePlayer.SourceBasePos.y = Screen.height - ThePlayer.SourceBasePos.y;
                ThePlayer.OutBasePos.y = Screen.height - ThePlayer.OutBasePos.y;
                break;
            case EPanelSize.Small:
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

	// Update is called once per frame
	void Update ()
    {
        // If camera shake is not zero, reduce it
        if (CameraShake >= Time.deltaTime)
            CameraShake -= Time.deltaTime;
        else
            CameraShake = 0;

		// "Animate" the switch
        if (PowerFlowing)
        {
            SwitchObj.GetComponent<Animator>().Play("Panel Switch On");
        }
        else
        {
            SwitchObj.GetComponent<Animator>().Play("Panel Switch Off");
        }

        // The camera offset (for shaking)
        Vector3 ShakingOffset = Vector3.one * Random.Range(-0.01f, 0.01f) * CameraShake;

        // Move the camera as necessary
        if (ViewingPanel)
        {
            // Set the camera position. Shake it if necessary
            Camera.main.transform.SetParent(ThePlayer.transform);
            Camera.main.transform.position = new Vector3(0, 0.06f, -10) + ShakingOffset * 0.5f;

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
            Camera.main.orthographicSize = 2;

            // Make the camera follow the ship and shake
            Camera.main.transform.position = new Vector3(PlayerShip.transform.position.x, PlayerShip.transform.position.y, -10) + ShakingOffset;

            // Shake the background too..
            if (CameraShake > 0)
                StarBackground.localPosition = new Vector3(ShakingOffset.x, ShakingOffset.y, 10);
            else
                StarBackground.localPosition = new Vector3(0, 0, 10);
        }
	}

    /// <summary>
    /// Spawns an enemy
    /// </summary>
    public void SpawnEnemy()
    {
        // If there is already an enemy vessel, destroy it
        if (EnemyShip != null)
            Destroy(EnemyShip.gameObject);

        // Create a new enemy ship
        EnemyShip = Instantiate(EnemyShipPrefab, PlayerShip.transform.position * 1.01f, new Quaternion(0, 0, 0, 0)).GetComponent<Ship>();
        EnemyShip.gameObject.name = "Enemy Ship";

        // Set bounds
        EnemyShip.BoundsLeft = BoundsLeft;
        EnemyShip.BoundsRight = BoundsRight;
        EnemyShip.BoundsUp = BoundsUp;
        EnemyShip.BoundsDown = BoundsDown;
    }

    /// <summary>
    /// Sets a new level
    /// </summary>
    /// <param name="NextComponent">The ID of the next component to be targetted</param>
    public void NewLevel(int NextComponent)
    {
        // Increment level number
        LevelNumber += 1;

        // Show level ui
        if (NextComponent == 0)
            _LevelUI.ShowingLevelUI = true;

        // First delete the old stuff
        // Delete the gates
        Destroy(Gates.GateParent);

        // Delete the panels
        if (PanelsRoot != null)
            Destroy(PanelsRoot);

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

        // Create a new ship if the current one is set to null
        if (EnemyShip == null)
        {
            SpawnEnemy();
        }

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
