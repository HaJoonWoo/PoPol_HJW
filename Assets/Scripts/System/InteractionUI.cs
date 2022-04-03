using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : Singleton<InteractionUI>
{
    [SerializeField] GameObject panel;
    [SerializeField] Text hotKeyText;
    [SerializeField] Text nameText;

    private void Start()
    {
        Close();
    }

    public void Open(string hotkey, string context)
    {
        panel.SetActive(true);

        hotKeyText.text = hotkey;
        nameText.text = context;
    }
    public void Close()
    {
        panel.SetActive(false);
    }
}
