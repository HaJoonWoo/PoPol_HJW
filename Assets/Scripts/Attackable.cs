using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Stateable))] //stateable의 종속성을 나타낸다 자동으로 추가
public class Attackable : MonoBehaviour
{
    [Header("Attackable")]
    [SerializeField] Transform attackPivot;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask exceptMask;  //공격 제외 대상.


    LayerMask attackMask = int.MaxValue; //1111 1111

    Stateable stat;

    void Start()
    {
        stat = GetComponent<Stateable>();
        attackMask = attackMask ^ exceptMask;
    }
    public void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(attackPivot.position, attackRadius, attackMask);

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
