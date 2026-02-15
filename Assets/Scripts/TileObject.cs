using UnityEngine;

public class TileObject : MonoBehaviour
{
    public bool canSwap;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        spriteRenderer.color = canSwap ? Color.white : Color.grey;
    }
}
