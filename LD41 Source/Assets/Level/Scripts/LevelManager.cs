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
    private GameObject BGPanelObj;
    private GameObject FGPanelObj;
    private GameObject SwitchObj;

	// Use this for initialization
	void Start () {
		// Setup the level
        ViewingPanel = true;

        // First spawn in the background panel
        BGPanelObj = Instantiate(PanelBackground);
        
        // Then the panel, switch, various other things that depend on panel size
        // Use a switch statement and kill many stones with one bird ;)
        switch (PanelSize)
        {
            case EPanelSize.Large:
                FGPanelObj = Instantiate(LargePanel);
                FGPanelObj.name = "large panel";

                SwitchObj = Instantiate(SwitchPrefab, LargeSwitchPos, new Quaternion(0, 0, 0, 0));
                SwitchObj.name = "panel switch";

                break;
            case EPanelSize.Medium:
                FGPanelObj = Instantiate(MediumPanel);
                FGPanelObj.name = "medium panel";

                SwitchObj = Instantiate(SwitchPrefab, MediumSwitchPos, new Quaternion(0, 0, 0, 0));
                SwitchObj.name = "panel switch";

                break;
            case EPanelSize.Small:
                FGPanelObj = Instantiate(SmallPanel);
                FGPanelObj.name = "small panel";

                SwitchObj = Instantiate(SwitchPrefab, SmallSwitchPos, new Quaternion(0, 0, 0, 0));
                SwitchObj.name = "panel switch";

                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
