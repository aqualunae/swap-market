using TMPro;
using UnityEngine;

public class SwapCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI numberField;

    [SerializeField]
    private Board board;

    private void Update()
    {
        numberField.text = board.GetSwaps().ToString();
    }
}
