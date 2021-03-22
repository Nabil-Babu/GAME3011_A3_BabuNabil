using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : Singleton<GUIManager>
{
    public TextMeshProUGUI Score;
    [SerializeField] private int _playerScore;
    public int PlayerScore
    {
        get
        {
            return _playerScore; 
        }
        set
        {
            _playerScore = value;
            Score.text = _playerScore.ToString() +" / "+ GameBoardManager.instance.scoreLimit.ToString(); 
        }
    }

    public TextMeshProUGUI Time;
    [SerializeField] private int _currentTime;
    public int CurrentTime
    {
        get
        {
            return _currentTime;
        }
        set
        {
            _currentTime = value;
            Time.text = _currentTime.ToString() + "s";
        }
    }


    public GameObject VictoryText;
    public GameObject LossText;
    public GameObject MessageBackground; 
    public void EnableVictoryText()
    {
        MessageBackground.SetActive(true);
        VictoryText.SetActive(true);
        LossText.SetActive(false);
    }
    
    public void EnableLossText()
    {
        MessageBackground.SetActive(true);
        VictoryText.SetActive(false);
        LossText.SetActive(true);
    }

    public void ResetMessages()
    {
        MessageBackground.SetActive(false);
        VictoryText.SetActive(false);
        LossText.SetActive(false);
    }
    
}
