using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stateable : MonoBehaviour
{
    [System.Serializable]
    public struct StatData
    {
        public float hp;                  // 체력
        public float maxHp;               // 최대 체력
        public float attackDamage;        // 물리 공격력.        
        public float attackDefence;       // 물리 방어력.
        public float experience;         // 경험치
        public float levelExperience;     // 레벨업바경험치
        public int level;                   //레벨
    }

    [SerializeField] StatData stat;

    public StatData Stat => stat;
}
