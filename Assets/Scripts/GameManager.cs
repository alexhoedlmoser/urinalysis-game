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
    
    [HideInInspector] public DiagnoseCard currentMouseDiagnose;
    [HideInInspector] public UrinProbe currentMouseProbe;
    public UrinType[] urinTypes;
    public UrinProbe[] allProbes;

    public int playerScore;
    public int pointsPerProbe;
    [SerializeField] private TMP_Text pointDisplay;

   

    public UrinType GetRandomType()
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

    public void IncreaseScore()
    {
        playerScore += pointsPerProbe;
        UpdateScoreDisplay();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (UrinProbe urinProbe in allProbes)
            {
                urinProbe.SetupProbe();
            }
        }
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
