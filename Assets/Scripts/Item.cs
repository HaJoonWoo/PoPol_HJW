using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] private ItemData data;
    [SerializeField] private int count;

    public int Count => count;
    public Sprite ItemSprite => data.itemSprite;
    public string ItemName => data.itemName;
    public float Hp_Healing => data.hp_Healing;
    public float AttackDamage => data.attackDamage;
    public float AttackDefence => data.attackDefence;
    public float Price => data.price;
    public int rank => data.rank;

    public Item(ItemData data, int count)
    {
        this.data = data;
        this.count = count;
    }
    public Item(Item copy)
    {
        data = copy.data;
        count = copy.count;
    }

    public bool Equals(Item target)
    {
        return data.itemKey == target.data.itemKey;
    }

    public void Add(Item item)
    {
        //���� �Ű����� �������� ���ƾ� �Ѵ�.
        if (Equals(item))
            count += item.count;
    }
    public string GetTooltip()
    {
        string tip = string.Empty;

        tip = string.Concat(tip, $"������ �̸� : {data.itemName}\n");
        tip = string.Concat(tip, $"������ ���� : {count}");

        return tip;
    }
}
