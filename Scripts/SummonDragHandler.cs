// Assets/Scripts/SummonDragHandler.cs
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class SummonDragHandler :
    MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    RectTransform rt;
    Canvas rootCanvas;
    Transform originalParent;
    int originalIndex;
    Vector2 originalAnchored;
    CanvasGroup cg;

    [Header("Options")]
    public bool handOnly = true;
    public RectTransform dragLayer;

    public void InitForHandOnly() => handOnly = true;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();                 // RequireComponent로 존재 보장
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();

        rootCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
        if (!dragLayer && rootCanvas)
        {
            var t = rootCanvas.transform.Find("DragLayer");
            if (t) dragLayer = t as RectTransform;
        }
    }

    public void OnPointerEnter(PointerEventData e) { }
    public void OnPointerExit(PointerEventData e) { }

    public void OnBeginDrag(PointerEventData e)
    {
        if (!rt) return;
        if (!rootCanvas) rootCanvas = GetComponentInParent<Canvas>()?.rootCanvas;

        originalParent = rt.parent;
        originalIndex = rt.GetSiblingIndex();
        originalAnchored = rt.anchoredPosition;

        if (cg) cg.blocksRaycasts = false;

        var parent = (dragLayer != null) ? (Transform)dragLayer : rootCanvas?.transform;
        if (parent)
        {
            rt.SetParent(parent, false);
            rt.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData e)
    {
        if (!rt || !rootCanvas) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform, e.position, e.pressEventCamera, out var local);
        rt.anchoredPosition = local;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (cg) cg.blocksRaycasts = true;

        // handOnly인데 드롭 실패(부모 그대로)면 원위치
        if (handOnly && originalParent && rt && rt.parent == originalParent)
        {
            rt.SetParent(originalParent, false);
            rt.SetSiblingIndex(originalIndex);
            rt.anchoredPosition = originalAnchored;
        }
    }
}
