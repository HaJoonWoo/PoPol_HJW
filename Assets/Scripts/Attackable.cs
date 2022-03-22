using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Stateable))] //stateable�� ���Ӽ��� ��Ÿ���� �ڵ����� �߰�
public class Attackable : MonoBehaviour
{
    [Header("Attackable")]
    [SerializeField] Transform attackPivot;
    [SerializeField] float attackRadius;


    Stateable stat;

    void Start()
    {
        stat = GetComponent<Stateable>();
    }
    public void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(attackPivot.position, attackRadius);

        foreach (Collider hit in hits)
        {
            Damageable target = hit.GetComponent<Damageable>();
            if (target != null)
            {
                target.OnDamaged(stat.Stat);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPivot != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(attackPivot.position, attackRadius);
        }
    }
}
