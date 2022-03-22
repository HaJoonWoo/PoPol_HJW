using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animation))]

public class DamageText : MonoBehaviour, IPool<DamageText>
{
    static Camera mainCam;

    const string KEY_NORMAL = "DamageText_Appear";
    const string KEY_CRI = "DamageText_Appear_Cri";

    [SerializeField] Text damageText;

    Animation anim;

    // position:���� ��ġ, amount:������ ��ġ.
    public void Appear(Vector3 position, int amount)
    {
        // amount ��ġ�� string���� ���.
        damageText.text = amount.ToString();
               

        StartCoroutine(FixPosition(position));
    }

    IEnumerator FixPosition(Vector3 position)
    {
        while (anim.isPlaying)
        {
            // WorldToScreenPoint : ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ.
            Vector2 damagePosition = mainCam.WorldToScreenPoint(position);
            transform.position = damagePosition;

            yield return null;
        }

        OnReturnPool(this);     // pool �Ŵ����� �ǵ��ư���.
    }

    OnReturnPoolEvent<DamageText> OnReturnPool;
    public void Setup(OnReturnPoolEvent<DamageText> OnReturnPool)
    {
        this.OnReturnPool = OnReturnPool;

        mainCam = Camera.main;
        anim = GetComponent<Animation>();
    }
}
