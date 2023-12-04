using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public enum GameState
{
    InProgress,
    InDialog,
    InReview
}

public class GameManager : MonoBehaviour
{
    #region SINGLETON

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public event Action<GameState> OnSwitchGameState; 
    public event Action<bool> OnScoreChange; 

    [HideInInspector] public DiagnoseCard currentMouseDiagnose;
    [HideInInspector] public UrineProbe currentMouseProbe;
    public UrineType[] urinTypes;
    public UrineProbe[] allProbes;
    public ReviewGrade[] reviewGrades;
    public GameState gameState;

    public int playerScore;
    public int pointsPerProbe;
    public int correctDiagnoses = 0;
    public int wrongDiagnoses = 0;
    public int totalDiagnoses = 0;
    public float timeNeeded = 0f;
    public int diagnosesPerRound;
    
    [SerializeField] private TMP_Text pointDisplay;

    public static Professor CurrentProfessor => _currentProfessor ? _currentProfessor : _currentProfessor = FindObjectOfType<Professor>();
    private static Professor _currentProfessor;
    
    public static PlayerInputHandler CurrentInputHandler => _currentInputHandler ? _currentInputHandler : _currentInputHandler = FindObjectOfType<PlayerInputHandler>();
    private static PlayerInputHandler _currentInputHandler;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (UrineProbe urinProbe in allProbes)
            {
                urinProbe.SetupProbe();
            }
        }
        
        CountGameTime();
    }

    private void CountGameTime()
    {
        if (gameState != GameState.InProgress) return;
        timeNeeded += Time.deltaTime;
        print(timeNeeded);
    }
    
    public UrineType GetRandomType()
    {
        return urinTypes[Random.Range(0, urinTypes.Length)];
    }

    public string GetDiagnoseLabel(DiagnoseType diagnoseType)
    {
        return diagnoseType switch
        {
            DiagnoseType.Dehydrated => "Dehydration",
            DiagnoseType.Overhydrated => "Overhydration",
            DiagnoseType.Infection => "Urinary Tract Infection",
            DiagnoseType.Hematuria => "Hematuria",
            DiagnoseType.VitaminOverdose => "Vitamin B2",
            DiagnoseType.Healthy => "Healthy",
            DiagnoseType.RedBeetJuice => "Red Beets",
            DiagnoseType.LiverIssue => "Liver Issue",
            DiagnoseType.MethyleneDye => "Methyline Blue Dye",
            DiagnoseType.Rhubarb => "Rhubarb, Fava Beans",
            DiagnoseType.Rhabdomyolosis => "Rhabdomyolosis",
            DiagnoseType.PhysicalActivity => "Intense Physical Activity",
            DiagnoseType.KidneyDisease => "Kidney Disease",
            DiagnoseType.Diabetes => "Diabetes",
            _ => throw new ArgumentOutOfRangeException(nameof(diagnoseType), diagnoseType, null)
        };
    }

    public void SwitchGameState(GameState newState)
    {
        gameState = newState;
        OnSwitchGameState?.Invoke(gameState);
    }

    public void IncreaseScore()
    {
        playerScore += pointsPerProbe;
        
        OnScoreChange?.Invoke(true);
        
        correctDiagnoses++;
        ApplyDiagnose();
    }
    
    public void DecreaseScore()
    {
        playerScore -= pointsPerProbe/2;
        if (playerScore < 0)
        {
            playerScore = 0;
        }
        
        OnScoreChange?.Invoke(false);
        
        wrongDiagnoses++;
        ApplyDiagnose();
    }

    public void ApplyDiagnose()
    {
        UpdateScoreDisplay();
        totalDiagnoses++;
        if (totalDiagnoses >= diagnosesPerRound)
        {
           EndGame();
        }
    }

    public void EndGame()
    {
        SwitchGameState(GameState.InReview);
    }

    public void UpdateScoreDisplay()
    {
        pointDisplay.text = "Score: " + playerScore;
    }

    public ReviewGrade GetGrade()
    {
        foreach (ReviewGrade grade in reviewGrades)
        {
            float diagnosePercentage = 100f * ((float)correctDiagnoses / totalDiagnoses);
            print(diagnosePercentage);
            
            if (diagnosePercentage >= grade.percentageThreshold && timeNeeded <= grade.timeThreshold)
            {
                return grade;
            }
        }

        return reviewGrades[^1];
    }

    public void RestartGame()
    {
        timeNeeded = 0f;
        totalDiagnoses = wrongDiagnoses = correctDiagnoses = 0;
        SwitchGameState(GameState.InProgress);
        foreach (UrineProbe urinProbe in allProbes)
        {
            urinProbe.SetupProbe();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

[Serializable]
public class ReviewGrade
{
    public string displayText;
    public Color color;
    [FormerlySerializedAs("wrongDiagnosesThreshold")] public int percentageThreshold;
    public float timeThreshold;
}
