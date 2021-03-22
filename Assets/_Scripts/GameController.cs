using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject gameView;
    
    public void ToggleGame()
    {
        gameView.SetActive(!gameView.activeInHierarchy);
    }

    public void ResetGame()
    {
        GameBoardManager.instance.ResetGame();
    }
}
