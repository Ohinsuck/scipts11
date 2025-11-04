using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BoardSlotDropZone : MonoBehaviour, IDropHandler
{
    [Header("Accept")]
    public bool acceptCards = true;
    public bool acceptTokens = true;

    [Header("Visual")]
    public Material defaultUIMaterial;

    RectTransform slotRT;

    void Awake()
    {
        slotRT = transform as RectTransform;
        // 드롭을 받으려면 RaycastTarget이 켜져 있어야 함
        var img = GetComponent<Image>();
        if (img) img.raycastTarget = true;
    }

    // === 외부에서 '직접' 장착할 때 사용 (클릭-스폰 등) ===
    public void Place(GameObject go)
    {
        if (!go) return;

        // 카드/토큰 타입 필터링 (CardDisplay 유무로 판단)
        var disp = go.GetComponent<CardDisplay>();
        bool isCard = false, isToken = false;
        if (disp)
        {
            isToken = disp.treatAsTokenOrUnit;
            isCard = !isToken;
        }
        else
        {
            // CardDisplay가 없으면 토큰으로 간주(자유 오브젝트)
            isToken = true;
        }

        if ((isCard && !acceptCards) || (isToken && !acceptTokens))
        {
            Debug.Log($"[BoardSlotDropZone] Reject (type mismatch) on {name}");
            return;
        }

        // 드래그 중이었다면 Raycast 차단 복구
        var cg = go.GetComponent<CanvasGroup>();
        if (cg) cg.blocksRaycasts = true;

        // 슬롯으로 편입
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(slotRT, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;

        // 손패에서 왔다면 HandItem가 파괴/이동 통보를 못 남기지 않게 owner 측 정리
        var hi = go.GetComponent<HandItem>();
        if (hi && hi.owner) hi.owner.NotifyItemDestroyed(go); // 리스트에서 제거 + 재정렬

        // (선택) 드롭 완료 후 슬롯을 잠그고 싶으면 여기서 상태 관리
        // ex) this.enabled = false;
    }

    // === 드래그-드롭으로 들어온 케이스 ===
    public void OnDrop(PointerEventData eventData)
    {
        var go = eventData.pointerDrag;
        if (!go) return;
        Place(go);
    }
}
