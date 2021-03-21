using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    private static readonly Color SelectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private static TileBehaviour _previousSelectedTile = null;
    [SerializeField] private SpriteRenderer render;
    private bool IsSelected { get; set; } = false;
    
    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

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

    private void OnMouseDown()
    {
        if (render.sprite == null) return;
        
        
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
                    Debug.Log("Swapping Tiles");
                    SwapImages(_previousSelectedTile.render);
                } else {
                    Debug.Log("New Select");
                    _previousSelectedTile.GetComponent<TileBehaviour>().Deselect();
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
    
    private GameObject GetAdjacent(Vector2 castDir) {
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null) {
            
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

}
