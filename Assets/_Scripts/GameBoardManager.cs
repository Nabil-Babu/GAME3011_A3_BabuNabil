using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : Singleton<GameBoardManager>
{
    [Header("Game Board Properties")]
    public GameObject tilePrefab; 
    public int boardXSize;
    public int boardYSize;
    public List<Sprite> tileImages = new List<Sprite>();

    private GameObject[,] _tileGrid;
    private Transform _boardTransform;
    
    void Start()
    {
        _boardTransform = transform;
        Vector2 offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
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
                possibleImages.AddRange(tileImages);

                possibleImages.Remove(previousLeft[yIndex]);
                possibleImages.Remove(previousBelow);
                
                Sprite newSprite = possibleImages[Random.Range(0, possibleImages.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
                previousLeft[yIndex] = newSprite;
                previousBelow = newSprite;
            }
        }
    }
    
    
}
