using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animation))]

public class DamageText : MonoBehaviour, IPool<DamageText>
{
    static Camera mainCam;

    const string KEY_NORMAL = "Nomal_Damage_Animation";
    const string KEY_CRI = "Cri_Damage_Animation";

    [SerializeField] Text damageText;

    Animation anim;

    // position:생성 위치, amount:데미지 수치.
    public void Appear(Vector3 position, int amount, DAMAGE_TYPE type)
    {
        // amount 수치를 string으로 출력.
        damageText.text = amount.ToString();

        switch (type)
        {
            case DAMAGE_TYPE.Normal:
                anim.Play(KEY_NORMAL);
                break;

            case DAMAGE_TYPE.Critical:
                anim.Play(KEY_CRI);
                break;
        }

        StartCoroutine(FixPosition(position));
    }

    IEnumerator FixPosition(Vector3 position)
    {
        while (anim.isPlaying)
        {
            // WorldToScreenPoint : 월드 좌표를 스크린 좌표로 반환.
            Vector2 damagePosition = mainCam.WorldToScreenPoint(position);
            transform.position = damagePosition;

            yield return null;
        }

        OnReturnPool(this);     // pool 매니저로 되돌아간다.
    }

    OnReturnPoolEvent<DamageText> OnReturnPool;
    public void Setup(OnReturnPoolEvent<DamageText> OnReturnPool)
    {
        this.OnReturnPool = OnReturnPool;

        mainCam = Camera.main;
        anim = GetComponent<Animation>();
    }
}
