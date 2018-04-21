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

    // Properties of the panel
    public GameObject LargePanel;
    public GameObject MediumPanel;
    public GameObject SmallPanel;
    public EPanelSize PanelSize;
    public GameObject SwitchPrefab;
    public bool PowerFlowing; // whether the switch is on or off

    // Other sprites
    public GameObject PanelBackground;

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

	// Use this for initialization
	void Start () {
		// Setup the level
        ViewingPanel = true;

        // Get the player
        ThePlayer = GameObject.FindObjectOfType<Player>();

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
                FGPanelObj = Instantiate(LargePanel);
                FGPanelObj.name = "large panel";
                FGPanelObj.transform.parent = PanelsRoot.transform;

                SwitchObj = Instantiate(SwitchPrefab, LargeSwitchPos, new Quaternion(0, 0, 0, 0));
                SwitchObj.name = "panel switch";
                SwitchObj.transform.parent = PanelsRoot.transform;

                ThePlayer.SourceBasePos = new Vector2(-0.15f, 0.1f);
                ThePlayer.OutBasePos = new Vector2(0.15f, 0.1f);
                break;
            case EPanelSize.Medium:
                FGPanelObj = Instantiate(MediumPanel);
                FGPanelObj.name = "medium panel";
                FGPanelObj.transform.parent = PanelsRoot.transform;

                SwitchObj = Instantiate(SwitchPrefab, MediumSwitchPos, new Quaternion(0, 0, 0, 0));
                SwitchObj.name = "panel switch";
                SwitchObj.transform.parent = PanelsRoot.transform;

                ThePlayer.SourceBasePos = new Vector2(-0.24f, 0.1f);
                ThePlayer.OutBasePos = new Vector2(0.22f, 0.1f);
                break;
            case EPanelSize.Small:
                FGPanelObj = Instantiate(SmallPanel);
                FGPanelObj.name = "small panel";
                FGPanelObj.transform.parent = PanelsRoot.transform;

                SwitchObj = Instantiate(SwitchPrefab, SmallSwitchPos, new Quaternion(0, 0, 0, 0));
                SwitchObj.name = "panel switch";
                SwitchObj.transform.parent = PanelsRoot.transform;

                ThePlayer.SourceBasePos = new Vector2(-0.24f, 0.19f);
                ThePlayer.OutBasePos = new Vector2(0.22f, 0.19f);
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
		// "Animate" the switch
        if (PowerFlowing)
        {
            SwitchObj.GetComponent<Animator>().Play("Panel Switch On");
        }
        else
        {
            SwitchObj.GetComponent<Animator>().Play("Panel Switch Off");
        }
	}
}
