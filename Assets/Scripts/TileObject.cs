using UnityEngine;

public class TileObject : MonoBehaviour
{
    public bool canSwap;

    public GameObject Select;

    public bool isM;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        spriteRenderer.color = canSwap ? Color.white : Color.grey;
    }

    public void ToggleSelect(bool setTrue)
    {
        if(Select != null)
        {
            Select.SetActive(setTrue);
        }
    }
}
