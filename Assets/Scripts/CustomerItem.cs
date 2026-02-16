using UnityEngine;
using static Scripts;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

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
    public bool IsCharacter = true;

    public void Start()
    {
        if (IsCharacter)
        {
            StartCoroutine(iRotate());        
        }
    }

    public IEnumerator iRotate()
    {
        transform.DORotate(new Vector3(0,0,-5), 0.5f, RotateMode.Fast);
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            transform.DORotate(new Vector3(0,0,5), 1.0f, RotateMode.Fast);
            yield return new WaitForSeconds(1.0f);     
            transform.DORotate(new Vector3(0,0,-5), 1.0f, RotateMode.Fast);
            yield return new WaitForSeconds(1.0f);      
        }
        yield return null;
    }
}
