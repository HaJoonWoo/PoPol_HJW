using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UIManager : ObjectPool<UIManager, DamageText>
{
    public Image hpSprite;
    public Image experienceSprite;
    public Text levelText;
    public Text hpText;
    public Text maxHPText;

    [SerializeField] PlayerController playStat;
    [SerializeField] Damageable playHp;

    
    void Start() 
    {
        
    }
    void Update()
    {
        SetUp();
    }

    public void SetUp()
    {
        hpSprite.fillAmount = playHp.hp / 100f;
        maxHPText.text = playStat.maxHp.ToString();
        experienceSprite.fillAmount = playStat.levelExperience / 100f;

        levelText.text = string.Format("{0}%",(experienceSprite.fillAmount * 100f).ToString());
        hpText.text = playStat.hp.ToString();
       
    }
    public void AppearDamage(Vector3 position, int amount)
    {
        DamageText pool = GetPool();            // Ǯ���� ������Ʈ �ϳ��� �����´�.
        pool.Appear(position, amount);    // ������ �ؽ�Ʈ�� ����Ѵ�.
    }
}
