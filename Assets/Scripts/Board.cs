using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
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
    private int customersAvailable = 0;

    public AudioSource audioSwap;

    public AudioSource audioSuccess;
    public AudioSource audioSuccessPart;
    public AudioSource audioFail;

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
            if (obj is CustomerItem)
            {
                CustomerItem customerItem = obj as CustomerItem;
                if (!customerItem.IsItem)
                {
                    customersAvailable++;
                }
            }
        }
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Market");

        swaps = initialSwaps;
    }

    public void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
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

    public void ClearSelect()
    {
        for(int x = 0; x < 6; x++)
        {
            for(int y = 0; y< 3; y++)
            {
                int index = x + y*gridWidth;
                tileDictionary[index].ToggleSelect(false);
            }
        } 
        
    }

    public Vector2[] adjacentTiles(Vector2 coords)
    {
        Vector2[] list = new Vector2[4];
        if (coords.x - 1 >= 0)
        {
            list[0] = new Vector2(coords.x - 1, coords.y);        
        } 
        else list[0] = new Vector2(-1,-1);
        if (coords.x + 1 <= 6)
        {
            list[1] = new Vector2(coords.x + 1, coords.y);        
        } 
        else list[1] = new Vector2(-1,-1);
        if (coords.y - 1 >= 0)
        {
            list[2] = new Vector2(coords.x, coords.y - 1);        
        } 
        else list[2] = new Vector2(-1,-1);
        if (coords.y + 1 <= 3)
        {
            list[3] = new Vector2(coords.x, coords.y + 1);        
        } 
        else list[3] = new Vector2(-1,-1);
        return list;
    }

    public void OnClickLocation(InputValue value)
    {
        Vector3Int coords = grid.WorldToCell(Camera.main.ScreenToWorldPoint(value.Get<Vector2>()));
        int index = coords.x + (coords.y * gridWidth);
        if (index < 0) { return; }
        TileObject clickedObject = tileDictionary[index];
        if (clickedObject != null)
        {
            if (!clickedObject.canSwap) { return; }
            
            // foreach (Vector2 v in adjacentTiles(value.Get<Vector2>()))
            // {
            //     v.
            // }
            //clickedObject.ToggleSelect(true);
            
            if (clickedTile == defaultValue)
            {
                clickedTile = new Vector2Int(coords.x, coords.y);
                clickedObject.ToggleSelect(true);
            }
            else
            {
                if (Mathf.Abs(coords.x - clickedTile.x) + Mathf.Abs(coords.y - clickedTile.y) == 1)
                {
                    int lastIndex = clickedTile.x + (clickedTile.y * gridWidth);
                    TileObject lastObject = tileDictionary[lastIndex];
                    Debug.Log("SAFE");
                    audioSwap.Play();
                    ClearSelect();

                    if (lastObject is CustomerItem && clickedObject is CustomerItem)
                    {
                        CustomerItem lastCI = lastObject as CustomerItem;
                        CustomerItem clickedCI = clickedObject as CustomerItem;
                        if (lastCI.SaleColor == clickedCI.SaleColor && lastCI.IsItem != clickedCI.IsItem)
                        {
                            swaps += 5;
                            lastCI.canSwap = false;
                            clickedCI.canSwap = false;
                            customersAvailable--;
                            if (customersAvailable == 0)
                            {
                                GameOver(true);
                            }
                            else
                            {
                                audioSuccessPart.Play();
                            }
                        }
                    }

                    tileDictionary[index] = lastObject;
                    tileDictionary[lastIndex] = clickedObject;
                    clickedTile = defaultValue;
                    swaps--;
                    if (swaps <= 0)
                    {
                        GameOver(false);
                    }
                    Refresh();
                    return;
                }
            }
        }
    }

    [SerializeField]
    private GameObject resultsObject;

    [SerializeField]
    private TextMeshProUGUI resultsText;

    private void GameOver(bool victory)
    {
        for (int i = 0; i < tileDictionary.Count; i++)
        {
            TileObject tileObject = tileDictionary[i];
            if (tileObject != null)
            {
                tileObject.canSwap = false;
            }
        }
        
        resultsObject.SetActive(true);
        resultsText.text = victory ? "All your customers are satisfied! You won!" : "Your customers are tired and ready to go home. Game over.";
        if (victory)
        {
            audioSuccess.Play();
        }
        else
        {
            audioFail.Play();
        }
    }

    public int GetSwaps()
    {
        return swaps;
    }
}
