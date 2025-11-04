// Assets/Scripts/Deck/GraveDropZone.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class GraveDropZone : MonoBehaviour, IDropHandler
{
    public enum Mode { Auto, Delete, DiscardToDeck }
    public Mode mode = Mode.Auto;
    public DeckManager targetDeck;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null) return;

        // ▼ 1) 포인터가 찍은 오브젝트
        var picked = eventData.pointerDrag;

        // ▼ 2) 실제로 "운반하는 루트(캐리어)" 찾기: HandItem > SummonDragHandler > fallback
        GameObject carrier =
            picked.GetComponentInParent<HandItem>()?.gameObject
            ?? picked.GetComponentInParent<SummonDragHandler>()?.gameObject
            ?? picked;

        // ▼ 3) 카드 데이터/표시는 자식에서 읽는다
        var disp = carrier.GetComponentInChildren<CardDisplay>(true);
        bool isCard = disp && !disp.treatAsTokenOrUnit;
        bool isToken = disp && disp.treatAsTokenOrUnit;

        // ▼ 4) 손패에 파괴 알림은 "캐리어" 기준으로
        void NotifyHand(GameObject g)
        {
            var hi = g.GetComponent<HandItem>();
            if (hi && hi.owner) hi.owner.NotifyItemDestroyed(g);
        }

        // ▼ 5) 파괴 직전 안전 가드
        var cg = carrier.GetComponent<CanvasGroup>();
        if (cg) cg.blocksRaycasts = true;

        switch (mode)
        {
            case Mode.Auto:
                if (isToken)
                {
                    NotifyHand(carrier);
                    Destroy(carrier); // 토큰/유닛은 삭제
                }
                else if (isCard && targetDeck)
                {
                    if (disp.cardData) targetDeck.Discard(disp.cardData);
                    NotifyHand(carrier);
                    Destroy(carrier);
                }
                else
                {
                    NotifyHand(carrier);
                    Destroy(carrier);
                }
                break;

            case Mode.Delete:
                NotifyHand(carrier);
                Destroy(carrier);
                break;

            case Mode.DiscardToDeck:
                if (isCard && targetDeck && disp.cardData) targetDeck.Discard(disp.cardData);
                NotifyHand(carrier);
                Destroy(carrier);
                break;
        }
    }
}
