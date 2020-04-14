using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class FocusScrollRectOnEnable : MonoBehaviour
{
    [SerializeField] private RectTransform FocusTarget = null;
    [SerializeField] private RectTransform maskTransform = null;

    private ScrollRect scrollRect;
    private RectTransform scrollTransform;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollTransform = transform as RectTransform;
    }

    private void OnEnable()
    {
        Vector2 itemCenterPositionInScroll = scrollTransform.InverseTransformPoint(RelativeToParent(FocusTarget));
        Vector2 targetPositionInScroll = scrollTransform.InverseTransformPoint(RelativeToParent(maskTransform));
        Vector2 difference = targetPositionInScroll - itemCenterPositionInScroll;

        if (!scrollRect.horizontal) difference.x = 0f;
        if (!scrollRect.vertical) difference.y = 0f;

        Vector2 normalizedDifference = difference / (scrollRect.content.rect.size - scrollTransform.rect.size);
        Vector2 newNormalizedPosition = scrollRect.normalizedPosition - normalizedDifference;

        if (scrollRect.movementType != ScrollRect.MovementType.Unrestricted)
            newNormalizedPosition = Vector2.Min(Vector2.Max(newNormalizedPosition, Vector2.zero), Vector2.one);

        scrollRect.normalizedPosition = newNormalizedPosition;
    }

    private Vector2 RelativeToParent(RectTransform target)
    {
        Vector2 offset = Vector2.one * .5f - target.pivot;
        Vector2 localPosition = (Vector2)target.localPosition + offset * target.rect.size;
        return target.parent.TransformPoint(localPosition);
    }
}