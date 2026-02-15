using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [SerializeField]
    private Tilemap background;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private GameObject objectContainer;

    private Dictionary<int, TileObject> tileDictionary;
    int gridWidth;

    private void Start()
    {
        // read object locations
        background.CompressBounds();
        TileObject[] tileObjects = objectContainer.GetComponentsInChildren<TileObject>();
        Vector3Int gridSize = background.size;
        gridWidth = gridSize.x;
        int gridHeight = gridSize.y;
        int cellCount = gridWidth * gridHeight;
        tileDictionary = new Dictionary<int, TileObject>();
        for (int i = 0; i < cellCount; i++)
        {
            tileDictionary.Add(i, null);
        }
        foreach(TileObject obj in tileObjects)
        {
            Vector3Int location = grid.WorldToCell(obj.transform.position);
            int index = location.x + (location.y * gridWidth);
            tileDictionary[index] = obj;
        }
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Market");
    }

    private Vector2Int defaultValue = new Vector2Int(-1, -1);
    private Vector2Int clickedTile = new Vector2Int(-1, -1);

    public void OnClickLocation(InputValue value)
    {
        Vector3Int coords = grid.WorldToCell(Camera.main.ScreenToWorldPoint(value.Get<Vector2>()));
        int index = coords.x + (coords.y * gridWidth);
        if (index < 0) { return; }
        TileObject clickedObject = tileDictionary[index];
        if (clickedObject != null)
        {
            if (!clickedObject.canSwap) { return; }

            if (clickedTile == defaultValue)
            {
                clickedTile = new Vector2Int(coords.x, coords.y);
            }
            else
            {
                if (Mathf.Abs(coords.x - clickedTile.x) + Mathf.Abs(coords.y - clickedTile.y) == 1)
                {
                    int lastIndex = clickedTile.x + (clickedTile.y * gridWidth);
                    TileObject lastObject = tileDictionary[lastIndex];
                    tileDictionary[index] = lastObject;
                    tileDictionary[lastIndex] = clickedObject;
                    clickedTile = defaultValue;
                    Debug.Log("Swapped!");
                    return;
                }
                else
                {
                    Debug.Log("Not adjacent");
                }
            }
        }
        Debug.Log("Not swapped");
    }

    // method to update visuals
}
