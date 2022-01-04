using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singleton
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (!instance) Debug.LogError("UIManager is null");
            return instance;
        }
    }
    #endregion

    public enum GameState { Main, Game, Results }
    private GameState gameState = GameState.Main;

    [SerializeField, Tooltip("The main menu canvas")]
    private Transform mainCanvas;
    [SerializeField, Tooltip("The game canvas")]
    private Transform gameCanvas;
    [SerializeField, Tooltip("The results canvas")]
    private Transform resultsCanvas;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetActiveCanvas();
    }

    public void Play()
    {
        gameState = GameState.Game;
        SetActiveCanvas();
        GameManager.Instance.onPlay.Invoke();
    }

    public void Back()
    {
        gameState = GameState.Main;
        SetActiveCanvas();
        GameInfo.Reset();
    }

    public void Results()
    {
        gameState = GameState.Results;
        SetActiveCanvas();
        GameManager.Instance.onFinish.Invoke();
    }

    public void Quit()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    /// <summary>
    /// Sets all but the current canvas to inactive
    /// </summary>
    private void SetActiveCanvas()
    {
        switch (gameState)
        {
            case GameState.Main:
                if (mainCanvas) mainCanvas.gameObject.SetActive(true);
                if (gameCanvas) gameCanvas.gameObject.SetActive(false);
                if (resultsCanvas) resultsCanvas.gameObject.SetActive(false);
                break;
            case GameState.Game:
                if (mainCanvas) mainCanvas.gameObject.SetActive(false);
                if (gameCanvas) gameCanvas.gameObject.SetActive(true);
                if (resultsCanvas) resultsCanvas.gameObject.SetActive(false);
                break;
            case GameState.Results:
                if (mainCanvas) mainCanvas.gameObject.SetActive(false);
                if (gameCanvas) gameCanvas.gameObject.SetActive(false);
                if (resultsCanvas) resultsCanvas.gameObject.SetActive(true);
                break;
        }
    }
}
