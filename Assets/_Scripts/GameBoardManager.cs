using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Difficulty
{
    EASY,
    MEDIUM,
    HARD,
    TOTAL
}


public class GameBoardManager : Singleton<GameBoardManager>
{
    [Header("Game Board Properties")]
    public GameObject tilePrefab; 
    public int boardXSize;
    public int boardYSize;
    private int _currentScore = 0;
    public List<Sprite> tileImages = new List<Sprite>();
    public List<Sprite> EasyModeImages = new List<Sprite>();
    public List<Sprite> AvailableImages = new List<Sprite>();
    
    public Difficulty Difficulty = Difficulty.EASY;
    
    private float timer = 60.0f;
    public int scoreLimit = 1500;
    // private int movesLeft = 50;
    private GameObject[,] _tileGrid;

    public bool IsShifting { get; set; }
    public bool IsPlaying { get; set; }

    public int CurrentScore
    {
        get
        {
            return _currentScore;
        }
        set
        {
            _currentScore = value;
            if (_currentScore >= scoreLimit)
            {
                IsPlaying = false;
                GUIManager.instance.EnableVictoryText();   
            }
            GUIManager.instance.PlayerScore = _currentScore;
        }
    } 
    
    void Start()
    {
        IsPlaying = true;
        Vector2 offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        SetBoardDifficulty(Difficulty);
        CreateBoard(offset.x, offset.y);
        CurrentScore = 0;
    }

    private void Update()
    {
        Countdown();
    }

    void CreateBoard(float xOffset, float yOffset)
    {
        _tileGrid = new GameObject[boardXSize, boardYSize];
        
        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[boardYSize];
        Sprite previousBelow = null; 
        
        for (int xIndex = 0; xIndex < boardXSize; xIndex++)
        {
            for (int yIndex = 0; yIndex < boardYSize; yIndex++)
            {
                GameObject newTile = Instantiate(tilePrefab,
                    new Vector3(startX + (xOffset * xIndex), startY + (yOffset * yIndex), 0),
                    tilePrefab.transform.rotation);
                newTile.transform.parent = transform;
                _tileGrid[xIndex, yIndex] = newTile;

                List<Sprite> possibleImages = new List<Sprite>();
                possibleImages.AddRange(AvailableImages);

                possibleImages.Remove(previousLeft[yIndex]);
                possibleImages.Remove(previousBelow);
                
                Sprite newSprite = possibleImages[Random.Range(0, possibleImages.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
                previousLeft[yIndex] = newSprite;
                previousBelow = newSprite;
            }
        }
    }

    void ResetBoard()
    {
        Sprite[] previousLeft = new Sprite[boardYSize];
        Sprite previousBelow = null; 
        for (int xIndex = 0; xIndex < boardXSize; xIndex++)
        {
            for (int yIndex = 0; yIndex < boardYSize; yIndex++)
            {
                List<Sprite> possibleImages = new List<Sprite>();
                possibleImages.AddRange(AvailableImages);

                possibleImages.Remove(previousLeft[yIndex]);
                possibleImages.Remove(previousBelow);
                Sprite newSprite = possibleImages[Random.Range(0, possibleImages.Count)];
                _tileGrid[xIndex, yIndex].GetComponent<SpriteRenderer>().sprite = newSprite;
                previousLeft[yIndex] = newSprite;
                previousBelow = newSprite;
            }
        }
    }
    
    public IEnumerator FindNullTiles()
    {
        if (!IsPlaying) yield break;
        
        for (int x = 0; x < boardXSize; x++) {
            for (int y = 0; y < boardYSize; y++) {
                if (_tileGrid[x, y].GetComponent<SpriteRenderer>().sprite == null) {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }

        for (int x = 0; x < boardXSize; x++) {
            for (int y = 0; y < boardYSize; y++) {
                _tileGrid[x, y].GetComponent<TileBehaviour>().ClearAllMatches();
            }
        }
    }
    
    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .2f) {
        if (!IsPlaying) yield break;
        IsShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < boardYSize; y++) {
            SpriteRenderer render = _tileGrid[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null) {
                nullCount++;
            }
            renders.Add(render);
        }

        for (int i = 0; i < nullCount; i++) {
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < renders.Count - 1; k++) {
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, boardYSize - 1);
            }
        }
        IsShifting = false;
    }
    
    private Sprite GetNewSprite(int x, int y) {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(AvailableImages);

        if (x > 0) {
            possibleCharacters.Remove(_tileGrid[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < boardXSize - 1) {
            possibleCharacters.Remove(_tileGrid[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0) {
            possibleCharacters.Remove(_tileGrid[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }


    private void SetBoardDifficulty(Difficulty difficulty)
    {
        AvailableImages.Clear();
        switch (difficulty)
        {
            case Difficulty.EASY:
                AvailableImages.AddRange(EasyModeImages);
                timer = 120;
                scoreLimit = 1500;
                break;
            case Difficulty.MEDIUM:
                AvailableImages.AddRange(tileImages);
                timer = 90;
                scoreLimit = 1750;
                break;
            case Difficulty.HARD:
                AvailableImages.AddRange(tileImages);
                timer = 60;
                scoreLimit = 2000;
                break;
            default:
                break;
        }
    }
    
    public void Countdown()
    {
        if (!IsPlaying) return;
        
        timer -= Time.deltaTime;
        int roundedTime = (int)timer;
        GUIManager.instance.CurrentTime = roundedTime;
        if (roundedTime <= 0)
        {
            IsPlaying = false; 
            GUIManager.instance.EnableLossText();
        }
    }

    public void ResetGame()
    {
        IsPlaying = true;
        Vector2 offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        SetBoardDifficulty(Difficulty);
        ResetBoard();
        CurrentScore = 0;
        GUIManager.instance.ResetMessages();
    }

    public void IncreaseScore(int amount)
    {
        CurrentScore += amount;
    }
}
