using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/CreateNew")]

public class ItemData : ScriptableObject
{
    public enum TYPE
    {
        Equipment,      //장비
        Useable,        //소비
        Etc,            //기타
    }

    //아이템 기본정보
    public string itemKey;
    public Sprite itemSprite;
    public string itemName;
    public TYPE itemType;
    //아이템 스텟
    public float hp_Healing;
    public float attackDamage;
    public float attackDefence;
    public float price;
    //강화수치
    public int rank;
}
