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
        // SlotParent���Լ� �ڽ� ������Ʈ �˻�.
        slots = new ItemSlot[slotParent.childCount];    // �迭 ��ü ����.
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotParent.GetChild(i).GetComponent<ItemSlot>();
            slots[i].Setup(i);

            // ������ ���Կ� �̺�Ʈ ����.
            
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
        // activeSelf : ���� ������Ʈ�� Ȱ��ȭ �Ǿ��ִ���.
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

        //�гλ��¿� ���� ��� ��ȯ.
        return panel.activeSelf;
    }
    public void UpdateInventory(Item[] inventory)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)               // �����Ϸ��� �������� ���� ���.
                slots[i].SetItem(inventory[i]);     // ������ ���Կ� �����Ѵ�.
            else
                slots[i].Clear();                   // �������� ������ Clear �Ѵ�.
        }
    }

}
