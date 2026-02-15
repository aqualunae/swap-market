using UnityEngine;
using static Scripts;

public class CustomerItem : TileObject
{
    [SerializeField]
    private SaleColor color;

    [SerializeField]
    private bool isItem;

    public SaleColor SaleColor
    {
        get => color;
    }

    public bool IsItem
    {
        get => isItem;
    }
}
