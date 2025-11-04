// Assets/Scripts/HandManager.cs
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("References")]
    public DeckManager deckManager;
    public GameObject cardPrefab;          // PlayerPrefab_UI
    public RectTransform handTransform;    // Canvas/HandPosition

    [Header("Layout")]
    public float fanSpread = 7.5f;         // 각도/곡률 느낌용(현재 미사용)
    public float cardSpacing = 220f;       // 가로 간격(px)
    public float verticalSpacing = 40f;    // 세로 오프셋(px)
    public float handScale = 1.6f;         // 카드 스케일

    readonly List<RectTransform> _items = new();

    void Awake()
    {
        if (!handTransform)
            handTransform = GetComponent<RectTransform>();
    }

    /// <summary>덱에서 뽑은 카드 SO로 손패 UI를 추가</summary>
    public void AddCardToHand(CardGameTutorial.Card cardData)
    {
        if (!cardPrefab || !handTransform) return;

        // 손패 카드 루트 생성
        var go = Instantiate(cardPrefab, handTransform);
        go.name = cardData ? cardData.cardName + "_InHand" : "Card_InHand";

        // 크기/스케일 보정
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition3D = Vector3.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one * handScale;

        // 카드 표시 세팅 (SetData -> SetCard 로 수정)
        var disp = go.GetComponentInChildren<CardDisplay>(true);
        if (disp) disp.SetCard(cardData);

        // 소유자 세팅(파괴 시 되돌려 받도록)
        var hi = go.GetComponent<HandItem>();
        if (hi) hi.owner = this;

        _items.Add(rt);
        Relayout();
        Debug.Log($"[HandManager] Added '{go.name}' parent=HandPosition pos={rt.anchoredPosition}");
    }

    /// <summary>그레이브로 버리거나 파괴 등으로 사라질 때 콜백</summary>
    public void NotifyItemDestroyed(GameObject go)
    {
        if (!go) return;
        // 목록에서 제거
        _items.RemoveAll(t => !t || t.gameObject == go);
        Relayout();
    }

    /// <summary>수동 손패 셔플 버튼에서 호출</summary>
    public void ShuffleHand()
    {
        if (_items.Count <= 1) return;
        // 단순 셔플
        for (int i = 0; i < _items.Count; i++)
        {
            int j = Random.Range(i, _items.Count);
            (_items[i], _items[j]) = (_items[j], _items[i]);
        }
        Relayout();
    }

    /// <summary>손패 위치 재배치</summary>
    void Relayout()
    {
        // null 정리
        _items.RemoveAll(t => t == null);
        if (_items.Count == 0) return;

        float totalWidth = (_items.Count - 1) * cardSpacing;
        float startX = -totalWidth * 0.5f;

        for (int i = 0; i < _items.Count; i++)
        {
            var rt = _items[i];
            if (!rt) continue;

            var target = new Vector2(startX + i * cardSpacing, verticalSpacing);
            rt.anchoredPosition = target;
            rt.localScale = Vector3.one * handScale;
            rt.SetAsLastSibling(); // 항상 위에 보이도록
        }
    }
}
