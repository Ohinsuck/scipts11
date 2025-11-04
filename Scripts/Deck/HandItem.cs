// Assets/Scripts/HandItem.cs
using UnityEngine;

[DisallowMultipleComponent]
public class HandItem : MonoBehaviour
{
    [Tooltip("이 카드가 속한 손패 매니저")]
    public HandManager owner;

    bool _notified;

    void OnDestroy()
    {
        // 파괴 시 한 번만 통보
        Notify();
    }

    void OnDisable()
    {
        // 비활성화로 빠져도(파괴 직전 등) 한 번 보강
        Notify();
    }

    void Notify()
    {
        if (_notified) return;
        _notified = true;
        if (owner) owner.NotifyItemDestroyed(gameObject);
    }
}
