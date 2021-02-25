using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Public Variables
    public TextMeshProUGUI scoreCounterText;
    #endregion

    #region Private Variables
    [SerializeField] private float _currScore = 0f;
    #endregion

    #region Unity Callbacks
    void OnEnable() // Subcribed to event on Enable;
    {
        PlayerPickup.OnItemPicked += OnItemPickedEventReceived;
    }

    void OnDisable() // Unsubcribed to event on Disable;
    {
        PlayerPickup.OnItemPicked -= OnItemPickedEventReceived;
    }

    void OnDestroy() // Unsubcribed to event on Destroy;
    {
        PlayerPickup.OnItemPicked -= OnItemPickedEventReceived;
    }

    void Update() // Counter check on score each frame
    {
        if (scoreCounterText != null)
            OnWinning();
    }
    #endregion

    #region My Functions
    void OnWinning()
    {
        if (_currScore >= 10)
            SceneManager.LoadScene("WinSceneV2");
    }

    #region Buttons
    public void OnClickStartGame() // Tied to Start Button in MainMenu
    {
        SceneManager.LoadScene("MainScene");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnClickQuitGame() // Tied to Quit Button in MainMenu
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }

    public void OnClickMainMenu() // Tied to Back Button in Lost/Win Screens
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion

    #endregion

    #region Events
    void OnItemPickedEventReceived() // Received event from PlayerPickup Script and runs the funtion;
    {
        _currScore++;
        scoreCounterText.text = "Toys Collected: " + _currScore.ToString() + "/10";
    }
    #endregion
}