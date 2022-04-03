using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryUI : Singleton<InventoryUI>
{
    public bool isOpenInven
    {
        get;
        private set;
    }

    [SerializeField] GameObject panel;
    [SerializeField] Transform slotParent;
    
    [SerializeField] UnityEvent OnOpenInventory;
    [SerializeField] UnityEvent OnCloseInventory;

    ItemSlot[] slots;

    public delegate void OnChangedInvenEvent(int before, int after);
    public event OnChangedInvenEvent onChangedInven;

    private void Start()
    {
        // SlotParent에게서 자식 오브젝트 검색.
        slots = new ItemSlot[slotParent.childCount];    // 배열 객체 생성.
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotParent.GetChild(i).GetComponent<ItemSlot>();
            slots[i].Setup(i);

            // 아이템 슬롯에 이벤트 연결.
            
        }
               
        
    }

    public void ResetInventory()
    {
        
        foreach (ItemSlot slot in slots)
        {
            slot.OnExitSlot();
        }

        
    }
    public bool SwitchInventory()
    {
        // activeSelf : 게임 오브젝트가 활성화 되어있는지.
        panel.SetActive(!panel.activeSelf);
        isOpenInven = panel.activeSelf;

        if (panel.activeSelf)
        {
            OnOpenInventory?.Invoke();
        }
        else
        {
            OnCloseInventory?.Invoke();
            ResetInventory();
        }

        //패널상태에 따른 결과 반환.
        return panel.activeSelf;
    }
    public void UpdateInventory(Item[] inventory)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)               // 전달하려는 아이템이 있을 경우.
                slots[i].SetItem(inventory[i]);     // 아이템 슬롯에 세팅한다.
            else
                slots[i].Clear();                   // 아이템이 없으면 Clear 한다.
        }
    }

}
