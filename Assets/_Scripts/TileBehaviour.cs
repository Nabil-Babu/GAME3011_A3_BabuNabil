using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class TileBehaviour : MonoBehaviour
{
    private static readonly Color SelectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private static readonly Color MatchedColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    private static TileBehaviour _previousSelectedTile = null;
    [SerializeField] private SpriteRenderer render;
    private bool IsSelected { get; set; } = false;
   
    private Vector3[] adjacentDirections = new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
    private bool matchFound = false;
    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    private void Select()
    {
        IsSelected = true;
        render.color = SelectedColor;
        _previousSelectedTile = gameObject.GetComponent<TileBehaviour>();
    }

    private void Deselect()
    {
        IsSelected = false; 
        render.color = Color.white;
        _previousSelectedTile = null;
    }
    
    /// <summary>
    /// Debug Function
    /// </summary>
    public void Matched()
    {
        render.color = MatchedColor;
    }

    private void OnMouseDown()
    {
        if (render.sprite == null) return;
        if (!GameBoardManager.instance.IsPlaying) return;
        
        if (IsSelected)
        {
            Deselect();
        } else {
            if (_previousSelectedTile == null)
            {
                Select();
            } else {
                if (GetAllAdjacentTiles().Contains(_previousSelectedTile.gameObject))
                {
                    //Debug.Log("Swapping Tiles");
                    SwapImages(_previousSelectedTile.render);
                    _previousSelectedTile.ClearAllMatches();
                    _previousSelectedTile.Deselect();
                    ClearAllMatches();
                } else {
                    //Debug.Log("New Select");
                    _previousSelectedTile.Deselect();
                    Select();
                }
            }
        }
    }

    public void SwapImages(SpriteRenderer otherImage)
    {
        if (render.sprite == otherImage.sprite) return;
        Sprite temp = otherImage.sprite;
        otherImage.sprite = render.sprite;
        render.sprite = temp;
        // Play Effect
    }
    
    private GameObject GetAdjacent(Vector3 castDir) 
    {
        if (Physics.Raycast(transform.position, castDir, out var hit)) {
            
            return hit.collider.gameObject;
        }
        return null;
    }
    
    private List<GameObject> GetAllAdjacentTiles() {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++) {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector3 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit hit;
        Physics.Raycast(transform.position, castDir, out hit);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        {
            //Debug.Log("Found Match!");
            matchingTiles.Add(hit.collider.gameObject);
            Physics.Raycast(hit.collider.transform.position, castDir, out hit);
        }
        //Debug.Log("No More Matches");
        return matchingTiles;
    }
    
    private void ClearMatch(Vector3[] paths) {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++) { matchingTiles.AddRange(FindMatch(paths[i])); }
        if (matchingTiles.Count >= 2) 
        {
            for (int i = 0; i < matchingTiles.Count; i++) 
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
            
        }
    }
    
    public void ClearAllMatches() {
        if (render.sprite == null)
            return;

        ClearMatch(new Vector3[2] { Vector3.left, Vector3.right });
        ClearMatch(new Vector3[2] { Vector3.up, Vector3.down });
        if (matchFound) {
            render.sprite = null;
            matchFound = false;
            StopCoroutine(GameBoardManager.instance.FindNullTiles()); 
            StartCoroutine(GameBoardManager .instance.FindNullTiles());
            Debug.Log("Increasing Score");
            GameBoardManager.instance.IncreaseScore(50);
        }
    }

    

}
