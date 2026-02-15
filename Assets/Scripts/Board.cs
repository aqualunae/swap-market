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

    [SerializeField]
    private int initialSwaps = 10;

    private Dictionary<int, TileObject> tileDictionary;
    private int gridWidth;
    private int swaps;

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

        swaps = initialSwaps;
    }

    // method to update visuals
    private void Refresh()
    {
        for (int i = 0; i < tileDictionary.Count; i++)
        {
            TileObject tileObject = tileDictionary[i];
            if (tileObject != null)
            {
                Vector3Int calculatedPosition = new Vector3Int(i % gridWidth, i / gridWidth, 0);
                tileObject.transform.position = grid.CellToWorld(calculatedPosition) + (grid.cellSize / 2);
            }
        }
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
                    if (lastObject is CustomerItem && clickedObject is CustomerItem)
                    {
                        CustomerItem lastCI = lastObject as CustomerItem;
                        CustomerItem clickedCI = clickedObject as CustomerItem;
                        if (lastCI.SaleColor == clickedCI.SaleColor && lastCI.IsItem != clickedCI.IsItem)
                        {
                            swaps += 5;
                            lastCI.canSwap = false;
                            clickedCI.canSwap = false;
                        }
                    }

                    tileDictionary[index] = lastObject;
                    tileDictionary[lastIndex] = clickedObject;
                    clickedTile = defaultValue;
                    swaps--;
                    if (swaps <= 0)
                    {
                        GameOver();
                    }
                    Refresh();
                    return;
                }
            }
        }
    }

    private void GameOver()
    {
        for (int i = 0; i < tileDictionary.Count; i++)
        {
            TileObject tileObject = tileDictionary[i];
            if (tileObject != null)
            {
                tileObject.canSwap = false;
            }
        }
    }

    public int GetSwaps()
    {
        return swaps;
    }
}
