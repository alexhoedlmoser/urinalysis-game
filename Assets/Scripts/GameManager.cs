using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    
    public DiagnoseCard currentMouseDiagnose;
    public UrinProbe currentMouseProbe;
    public UrinType[] urinTypes;

    public int playerScore;
    public int pointsPerProbe;
    [SerializeField] private TMP_Text pointDisplay;

    public UrinType GetRandomType()
    {
        return urinTypes[Random.Range(0, urinTypes.Length)];
    }

    public string GetDiagnoseLabel(DiagnoseType diagnoseType)
    {
        switch (diagnoseType)
        {
            case DiagnoseType.Dehydrated:
                return "Dehydrated";
                break;
            case DiagnoseType.Overhydrated:
                return "Overhydrated";
                break;
            case DiagnoseType.Infection:
                return "Infection";
                break;
            case DiagnoseType.Blood:
                return "Blood";
                break;
            case DiagnoseType.VitaminOverdose:
                return "Vitamin Overdose";
                break;
            case DiagnoseType.Healthy:
                return "Healthy";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(diagnoseType), diagnoseType, null);
        }
    }

    public void IncreaseScore()
    {
        playerScore += pointsPerProbe;
        UpdateScoreDisplay();
    }

    public void DecreaseScore()
    {
        playerScore -= pointsPerProbe;
        if (playerScore < 0)
        {
            playerScore = 0;
        }
        UpdateScoreDisplay();
    }

    public void UpdateScoreDisplay()
    {
        pointDisplay.text = "Score: " + playerScore;
    }
}
