using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/CreateNew")]

public class ItemData : ScriptableObject
{
    public enum TYPE
    {
        Equipment,      //���
        Useable,        //�Һ�
        Etc,            //��Ÿ
    }

    //������ �⺻����
    public string itemKey;
    public Sprite itemSprite;
    public string itemName;
    public TYPE itemType;
    //������ ����
    public float hp_Healing;
    public float attackDamage;
    public float attackDefence;
    public float price;
    //��ȭ��ġ
    public int rank;
}
