using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Stateable))] //stateable의 종속성을 나타낸다 자동으로 추가
public class Damageable : MonoBehaviour
{
    [SerializeField] Transform damagePivot;
    [SerializeField] UnityEvent OnDamagedEvent;
    [SerializeField] UnityEvent OnDeadEvent;

    Stateable stat;
    public float MaxHp => stat.Stat.hp;        // 최대 체력.
    public float hp { get; private set; }      // 현재 체력.

    private void Start()
    {
        stat = GetComponent<Stateable>();
        hp = stat.Stat.hp;
    }

    public void OnDamaged(Stateable.StatData attacker)
    {
        OnDamagedEvent?.Invoke();

        // 데미지 계산.        
        float adDamage = Mathf.Clamp(attacker.attackDamage - stat.Stat.attackDefence, 0, 9999);

        // 치명타 계산. (20% 확률)
        bool isCritical = (Random.value * 100f) < 20;

        // 최종 데미지와 종류 계산.
        float finalDamage = adDamage * (isCritical ? 1.5f : 1.0f);
        //int finalDamageInt = Mathf.RoundToInt(finalDamage);
        //DAMAGE_TYPE damageType = isCritical ? DAMAGE_TYPE.Critical : DAMAGE_TYPE.Normal;

        // UI에 출력.
        //DamageManager.Instance.AppearDamage(damagePivot.position, finalDamageInt, damageType);

        // 나의 체력 감소.
        hp = Mathf.Clamp(hp - finalDamage, 0, MaxHp);
        if (hp <= 0)
        {
            attacker.levelExperience += stat.Stat.experience;
            OnDead();
        }
    }
    private void OnDead()
    {
        if (OnDeadEvent.GetPersistentEventCount() <= 0)         // 등록된 이벤트가 없다면.
            Destroy(gameObject);                                // 오브젝트 삭제.
        else                                                    // 등록된 이벤트가 있다면.
            OnDeadEvent?.Invoke();                              // 처리를 이벤트에게 양도.
    }
}
