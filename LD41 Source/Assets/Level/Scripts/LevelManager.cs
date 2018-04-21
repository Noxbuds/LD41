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

    // Properties of the panel
    public GameObject LargePanel;
    public GameObject MediumPanel;
    public GameObject SmallPanel;
    public EPanelSize PanelSize;
    public GameObject SwitchPrefab;

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

        // First spawn in the background panel
        BGPanelObj = Instantiate(PanelBackground);
        
        // Then the panel and switch
        // Use a switch statement and kill two stones with one bird ;)
        switch (PanelSize)
        {
            case EPanelSize.Large:
                FGPanelObj = Instantiate(LargePanel);
                SwitchObj = Instantiate(SwitchPrefab, LargeSwitchPos, new Quaternion(0, 0, 0, 0));
                break;
            case EPanelSize.Medium:
                FGPanelObj = Instantiate(MediumPanel);
                SwitchObj = Instantiate(SwitchPrefab, MediumSwitchPos, new Quaternion(0, 0, 0, 0));
                break;
            case EPanelSize.Small:
                FGPanelObj = Instantiate(SmallPanel);
                SwitchObj = Instantiate(SwitchPrefab, SmallSwitchPos, new Quaternion(0, 0, 0, 0));
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
