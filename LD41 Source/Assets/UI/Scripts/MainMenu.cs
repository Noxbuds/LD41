using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    
    // Textures
    public Texture2D TitleArt;
    public Texture2D ButtonImage;
    public Texture2D ButtonImageHover;
    public Font ButtonFont;
    private GUIStyle ButtonStyle;

    void Start()
    {
        // Setup GUI Style
        ButtonStyle = new GUIStyle();
        ButtonStyle.normal.background = ButtonImage;
        ButtonStyle.hover.background = ButtonImageHover;
        ButtonStyle.active.background = ButtonImage;
        ButtonStyle.alignment = TextAnchor.MiddleCenter;
        ButtonStyle.font = ButtonFont;
    }

    // Draw stuff
    void OnGUI()
    {
        // UI Scale
        float UIScale = Screen.width / 2560f;

        // Draw title art
        Rect TitleRect = new Rect(Screen.width / 2f - (TitleArt.width * 20f * UIScale) / 2f, Screen.height / 3f - (TitleArt.height * 20f * UIScale) / 2f, TitleArt.width * 20f * UIScale, TitleArt.height * 20f * UIScale);
        GUI.DrawTexture(TitleRect, TitleArt);

        // Button position
        float ButtonWidth = 360f * UIScale;
        float ButtonHeight = 150f * UIScale;
        ButtonStyle.fontSize = (int)(ButtonHeight * 0.6f);
        float ButtonX = Screen.width / 2f - ButtonWidth / 2f;
        float ButtonY = TitleRect.y + 3f * ButtonHeight;

        // Draw and handle button
        if (GUI.Button(new Rect(ButtonX, ButtonY, ButtonWidth, ButtonHeight), "BEGIN", ButtonStyle))
            SceneManager.LoadScene("MainGame");

        // Button position (quit button)
        ButtonX = Screen.width / 2f - ButtonWidth / 2f;
        ButtonY = TitleRect.y + 4.2f * ButtonHeight;

        // Draw and handle button
        if (GUI.Button(new Rect(ButtonX, ButtonY, ButtonWidth, ButtonHeight), "QUIT", ButtonStyle))
            Application.Quit();
    }
}
