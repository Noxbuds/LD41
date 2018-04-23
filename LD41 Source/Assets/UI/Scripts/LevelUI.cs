using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUI : MonoBehaviour {

    // Handles GUI elements related to the level

    // A data type for a dialog box which informs you about the current level
    [System.Serializable]
    public struct DialogBox
    {
        public string Title;
        public string Description;
    }

    // The ID of the current level UI
    public int CurrentLevelUI;
    public bool ShowingLevelUI;

    // The list of level UI boxes
    public List<DialogBox> LevelUIs;

    // Images
    public Texture2D ReportBackground;
    public Texture2D ButtonImage;
    public Texture2D ButtonImageHover;
    public Font ReportFont;
    private GUIStyle ReportStyle;
    private GUIStyle EmptyStyle;

    // Local references
    private Player ThePlayer;
    private LevelManager _LevelManager;

    /// <summary>
    /// Sets the dialog boxes up
    /// </summary>
    void SetupDialogs()
    {
        // Initialise the list
        LevelUIs = new List<DialogBox>();

        // Initial report saying that a hostile was detected
        DialogBox Dialog = new DialogBox();
        Dialog.Title = "MESSAGE FROM BRIDGE";
        Dialog.Description = "All stations, go to red alert - our sensors have just detected a hostile vessel approaching!";
        LevelUIs.Add(Dialog);

        // Damage report for thrusters
        Dialog = new DialogBox();
        Dialog.Title = "DAMAGE REPORT";
        Dialog.Description = "Multiple hull impacts detected. Thrusters are non-functional. All available engineers, please report to engine bay #1.";
        LevelUIs.Add(Dialog);

        // Another hostile detection
        Dialog = new DialogBox();
        Dialog.Title = "MESSAGE FROM BRIDGE";
        Dialog.Description = "Alert: Sensors have just detected another hostile vessel approaching!";
        LevelUIs.Add(Dialog);

        // Damage report for railguns
        Dialog = new DialogBox();
        Dialog.Title = "DAMAGE REPORT";
        Dialog.Description = "Railguns have been hit, and their circuits appear to be non-functional. All available engineers, please report to Weapon Control Bay #1";
        LevelUIs.Add(Dialog);

        // No hostiles detected
        Dialog = new DialogBox();
        Dialog.Title = "MESSAGE FROM BRIDGE";
        Dialog.Description = "Threat has been eliminated, long-range sensors are picking up no further hostiles. All stations, return to normal status.";
        LevelUIs.Add(Dialog);

        // "You Win"
        Dialog = new DialogBox();
        Dialog.Title = "YOU HAVE WON";
        Dialog.Description = "Unfortunately I couldn't fit in more 'levels', so this is it. Thank you for playing my game!";
        LevelUIs.Add(Dialog);
    }

	// Use this for initialization
	void Start ()
    {
		// Setup the Dialog Boxes
        SetupDialogs();

        // Get the level manager
        _LevelManager = GetComponent<LevelManager>();

        // Get the player
        ThePlayer = GameObject.FindObjectOfType<Player>();

        // Setup GUI styles
        ReportStyle = new GUIStyle();
        ReportStyle.normal.background = ButtonImage;
        ReportStyle.hover.background = ButtonImageHover;
        ReportStyle.active.background = ButtonImage;
        ReportStyle.font = ReportFont;
        ReportStyle.normal.textColor = Color.white;
        ReportStyle.hover.textColor = Color.white;
        ReportStyle.active.textColor = Color.white;
        ReportStyle.alignment = TextAnchor.MiddleCenter;

        EmptyStyle = new GUIStyle();
        EmptyStyle.font = ReportFont;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // This is mainly a GUI script...
    void OnGUI()
    {
        // UI Scale
        float UIScale = Screen.width / 2560f;

        // Only show the dialog box when switching levels
        if (ShowingLevelUI)
        {
            // Stop time from passing while the level UI is showing
            Time.timeScale = 0;

            // Calculate position and widths
            float ReportWidth = ReportBackground.width * 10f * UIScale;
            float ReportHeight = ReportBackground.height * 10f * UIScale;
            float ReportX = Screen.width / 2f - ReportWidth / 2f;
            float ReportY = Screen.height / 2f - ReportHeight / 2f;

            // Scale multiplier for font height
            float FontScale = 18f / LevelUIs[CurrentLevelUI].Title.Length;

            // Position and widths of button
            float ButtonWidth = ButtonImage.width * 10f * UIScale;
            float ButtonHeight = ButtonImage.height * 10f * UIScale;
            float ButtonX = ReportX + ReportWidth - ButtonWidth - 30f * UIScale;
            float ButtonY = ReportY + ReportHeight - ButtonHeight - 30f * UIScale;

            // Setup EmptyStyle correctly
            EmptyStyle.fontSize = (int)(80f * UIScale * FontScale);
            EmptyStyle.normal.textColor = Color.white;

            // Setup ReportStyle font size
            ReportStyle.fontSize = (int)(80f * UIScale);

            // Draw the background
            GUI.DrawTexture(new Rect(ReportX, ReportY, ReportWidth, ReportHeight), ReportBackground);

            // Write the title
            GUI.Box(new Rect(ReportX + 40f * UIScale, ReportY + 40f * UIScale, ReportWidth, ReportHeight), LevelUIs[CurrentLevelUI].Title, EmptyStyle);

            // Set the font size for the description
            EmptyStyle.fontSize = (int)(40f * UIScale * FontScale);
            EmptyStyle.wordWrap = true;
            
            // Write the description
            GUI.Box(new Rect(ReportX + 40f * UIScale, ReportY + 200f * UIScale, ReportWidth - 80f * UIScale, ReportHeight - 40f * UIScale), LevelUIs[CurrentLevelUI].Description, EmptyStyle);

            // Button to close
            if (GUI.Button(new Rect(ButtonX, ButtonY, ButtonWidth, ButtonHeight), "OK", ReportStyle))
            {
                // Play a sound
                ThePlayer.PickupSound.Play();

                // Increment level UI ID by 1, and stop showing level UI
                CurrentLevelUI++;
                ShowingLevelUI = false;

                // Handle the button
                HandleButtonClick(CurrentLevelUI - 1); // - 1 because we just increased it but are still referencing the same one
            }
        }
        else
            // Make time flow again
            Time.timeScale = 1;
    }

    // Handles the function of a button based on its ID
    void HandleButtonClick(int id)
    {
        if (id == 1)
        {
            // This one is when the thrusters are damaged
            _LevelManager.NewLevel(1);
        }
        else if (id == 2)
        {
            // Spawn a new enemy vessel
            _LevelManager.SpawnEnemy();

            // Set next level flag
            _LevelManager.NextLevelFlag = true;
        }
        else if (id == 3)
        {
            // This is the one where railguns are damaged
            _LevelManager.NewLevel(2);
        }
        else if (id == 4)
        {
            // Enable UI again
            ShowingLevelUI = true;
        }
        else if (id == 5)
            // Close game
            Application.Quit();
    }
}
