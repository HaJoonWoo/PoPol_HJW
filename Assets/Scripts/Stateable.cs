using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stateable : MonoBehaviour
{
    [System.Serializable]
    public struct StatData
    {
        public float hp;                  // ü��
        public float maxHp;               // �ִ� ü��
        public float attackDamage;        // ���� ���ݷ�.        
        public float attackDefence;       // ���� ����.
        public float experience;         // ����ġ
        public float levelExperience;     // �������ٰ���ġ
        public int level;                   //����
    }

    [SerializeField] StatData stat;

    public StatData Stat => stat;
}
