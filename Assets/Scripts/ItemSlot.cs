using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] protected Image iconImage;
    [SerializeField] protected Text countText;
    [SerializeField] protected GameObject selectedFrame;

    protected Item item;

    // ���� ���� �������� ������ ���°�ΰ�?
    public static int CurrentSlotIndex
    {
        get;
        private set;
    }
    public int SlotIndex => index;

    // �巡�� �̺�Ʈ.
    int index;
    public delegate void OnSlotDragEvent(ItemSlot slot);
    public event OnSlotDragEvent onBeginDrag;
    public event OnSlotDragEvent onDragging;
    public event OnSlotDragEvent onEndDrag;

    public void Setup(int index)
    {
        this.index = index;
    }
    public void SetItem(Item item)
    {
        iconImage.enabled = true;
        countText.enabled = true;

        // ����.
        iconImage.sprite = item.ItemSprite;
        countText.text = string.Format("{0}", item.Count);

        this.item = item;
    }
    public void SetItem(ItemSlot slot)
    {
        iconImage.sprite = slot.iconImage.sprite;
        countText.text = slot.countText.text;
    }
    public void Clear()
    {
        iconImage.enabled = false;
        countText.enabled = false;

        item = null;
    }



    // �̺�Ʈ �Լ�.
    public void OnEnterSlot()
    {
                  

        // ���� ���� ��ȣ�� ����.
        CurrentSlotIndex = index;
        selectedFrame.SetActive(true);
    }
    public void OnExitSlot()
    {
        

        // ���� ���� ��ȣ�� -1(�ܺ�)�� ����
        CurrentSlotIndex = -1;
        selectedFrame.SetActive(false);
    }

    public void OnBeginDrag()
    {
        if (item == null)
            return;

        onBeginDrag?.Invoke(this);
    }
    public void OnDragging()
    {
        if (item == null)
            return;

        onDragging?.Invoke(this);
    }
    public void OnEndDrag()
    {
        if (item == null)
            return;

        onEndDrag?.Invoke(this);
    }
}
