using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Stateable))] //stateable�� ���Ӽ��� ��Ÿ���� �ڵ����� �߰�
public class Damageable : MonoBehaviour
{
    [SerializeField] Transform damagePivot;
    [SerializeField] UnityEvent OnDamagedEvent;
    [SerializeField] UnityEvent OnDeadEvent;

    Stateable stat;
    public float MaxHp => stat.Stat.hp;        // �ִ� ü��.
    public float hp { get; private set; }      // ���� ü��.

    private void Start()
    {
        stat = GetComponent<Stateable>();
        hp = stat.Stat.hp;
    }

    public void OnDamaged(Stateable.StatData attacker)
    {
        OnDamagedEvent?.Invoke();

        // ������ ���.        
        float adDamage = Mathf.Clamp(attacker.attackDamage - stat.Stat.attackDefence, 0, 9999);

        // ġ��Ÿ ���. (20% Ȯ��)
        bool isCritical = (Random.value * 100f) < 20;

        // ���� �������� ���� ���.
        float finalDamage = adDamage * (isCritical ? 1.5f : 1.0f);
        //int finalDamageInt = Mathf.RoundToInt(finalDamage);
        //DAMAGE_TYPE damageType = isCritical ? DAMAGE_TYPE.Critical : DAMAGE_TYPE.Normal;

        // UI�� ���.
        //DamageManager.Instance.AppearDamage(damagePivot.position, finalDamageInt, damageType);

        // ���� ü�� ����.
        hp = Mathf.Clamp(hp - finalDamage, 0, MaxHp);
        if (hp <= 0)
        {
            attacker.levelExperience += stat.Stat.experience;
            OnDead();
        }
    }
    private void OnDead()
    {
        if (OnDeadEvent.GetPersistentEventCount() <= 0)         // ��ϵ� �̺�Ʈ�� ���ٸ�.
            Destroy(gameObject);                                // ������Ʈ ����.
        else                                                    // ��ϵ� �̺�Ʈ�� �ִٸ�.
            OnDeadEvent?.Invoke();                              // ó���� �̺�Ʈ���� �絵.
    }
}
