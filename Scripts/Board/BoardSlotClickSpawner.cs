using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoardSlotDropZone))]
public class BoardSlotClickSpawner : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("타일을 클릭했을 때 생성해 장착할 UI 프리팹 (RectTransform 루트)")]
    public GameObject prefab;

    [Tooltip("없으면 자동으로 루트 Canvas를 잡습니다.")]
    public RectTransform parentForSpawn;

    BoardSlotDropZone slot;
    Canvas rootCanvas;

    void Awake()
    {
        slot = GetComponent<BoardSlotDropZone>();
        rootCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
        if (!parentForSpawn && rootCanvas)
            parentForSpawn = rootCanvas.transform as RectTransform;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (!prefab || !parentForSpawn) return;

        var go = Instantiate(prefab, parentForSpawn);
        go.name = prefab.name + "_FromTile";

        // 생성 시점에 드래그용 레이어로 올라가 있을 수 있으므로 우선 parent는 Canvas
        var rt = go.GetComponent<RectTransform>();
        if (rt) rt.anchoredPosition = Vector2.zero;

        // 슬롯에 '직접 배치'
        slot.Place(go);
    }
}
