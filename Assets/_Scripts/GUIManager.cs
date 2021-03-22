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
            Score.text = _playerScore.ToString(); 
        }
    }
}
