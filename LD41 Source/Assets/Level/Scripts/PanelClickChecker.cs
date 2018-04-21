using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelClickChecker : MonoBehaviour {

    // Very lightweight script for detecting if the mouse is in bounds
    // of the working area, and checking if the switch is clicked;

    // Local reference to player and level manager
    private Player ThePlayer;
    private LevelManager _LevelManager;

	// Use this for initialization
	void Start () {
        ThePlayer = GameObject.FindObjectOfType<Player>();
        _LevelManager = GameObject.FindObjectOfType<LevelManager>();
	}
	
	// Called when the user hovers over this object's collider
    void OnMouseEnter()
    {
        if (gameObject.name == "large panel" || gameObject.name == "medium panel" || gameObject.name == "small panel")
            ThePlayer.MouseInBounds = true;
    }

    // Called when the user stops hovering over this object's collider
    void OnMouseExit()
    {
        if (gameObject.name == "large panel" || gameObject.name == "medium panel" || gameObject.name == "small panel")
            ThePlayer.MouseInBounds = false;
    }

    // Called when the user clicks on this object's collider
    void OnMouseDown()
    {
        if (gameObject.name == "panel switch")
            _LevelManager.PowerFlowing = !_LevelManager.PowerFlowing;
    }
}
