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
    
    public bool IsShifting { get; set; }
    
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
    
    public IEnumerator FindNullTiles() {
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
    
    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .5f) {
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
        possibleCharacters.AddRange(tileImages);

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
}
