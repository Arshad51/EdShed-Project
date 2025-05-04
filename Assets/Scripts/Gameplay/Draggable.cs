using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private SnapSlot currentSlot;

    public float snapThreshold = 80f;
    public float unsnapThreshold = 100f;

    private TextMeshProUGUI characterText;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        characterText = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(string value)
    {
        characterText.text = value;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Start drag, keep currentSlot reference
        // We will only clear it if dragged far enough in OnDrag
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Check for unsnapping distance during drag
        if (currentSlot != null)
        {
            RectTransform slotRect = currentSlot.GetComponent<RectTransform>();
            float distance = Vector2.Distance(rectTransform.anchoredPosition, slotRect.anchoredPosition);

            if (distance > unsnapThreshold)
            {
                currentSlot.IsOccupied = false;
                currentSlot = null;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Try to find the nearest available snap slot
        var slots = GameObject.FindObjectsOfType<SnapSlot>()
            .Where(slot => !slot.IsOccupied)
            .OrderBy(slot => Vector2.Distance(rectTransform.anchoredPosition, ((RectTransform)slot.transform).anchoredPosition));

        foreach (var slot in slots)
        {
            RectTransform slotRect = (RectTransform)slot.transform;
            float distance = Vector2.Distance(rectTransform.anchoredPosition, slotRect.anchoredPosition);

            if (distance < snapThreshold)
            {
                rectTransform.anchoredPosition = slotRect.anchoredPosition;
                slot.IsOccupied = true;
                currentSlot = slot;
                return;
            }
        }
    }
}
