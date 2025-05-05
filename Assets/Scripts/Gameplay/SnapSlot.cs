using UnityEngine;
using UnityEngine.UI;

public class SnapSlot : MonoBehaviour
{
    public string Value;
    public string CorrectValue;
    public bool IsOccupied = false;

    public void SetColor(Color color)
    {
        GetComponent<Image>().color = color;
    }
}
