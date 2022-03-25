using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DAMAGE_TYPE
{
    Normal,
    Critical,
}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Stateable))]
public class PlayerController : Singleton<PlayerController>
{
    const float GRAVITY = -9.81f;

    [SerializeField] Animator anim;
    [Header("Attack")]
    [SerializeField] Attackable attackable;
    [SerializeField] float comboDelay;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpHeight;

    [Range(1.0f, 4.0f)]
    [SerializeField] float gravityScale;

    Stateable stat;

    public float hp { get; private set; }      // 현재 체력
    public float maxHp { get; private set; }      // 최대 체력.
    public float experience { get; private set; }         // 경험치
    public float levelExperience { get; private set; }     // 레벨업바경험치
    public int level { get; private set; }                   //레벨

    CharacterController controller;
    Coroutine comboReset;
    
    float velocityY
    {
        get
        {
            return anim.GetFloat("velocityY");
        }
        set
        {
            anim.SetFloat("velocityY", value);
        }
    }
    bool isGround
    {
        get
        {
            return anim.GetBool("isGround");
        }
        set
        {
            anim.SetBool("isGround", value);
        }
    }
    float inputX
    {
        get
        {
            return anim.GetFloat("inputX");
        }
        set
        {
            anim.SetFloat("inputX", value);
        }
    }
    float inputY
    {
        get
        {
            return anim.GetFloat("inputY");
        }
        set
        {
            anim.SetFloat("inputY", value);
        }
    }

    bool isWeapon
    {
        get
        {
            return anim.GetBool("isWeapon");
        }
        set 
        {
            anim.SetBool("isWeapon", value);
        }
    }
    bool isSword
    {
        get
        {
            return anim.GetBool("isSword");
        }
        set
        {
            anim.SetBool("isSword", value);
        }
    }

    float gravity => GRAVITY * gravityScale;

    bool isDead;            // 죽었는가
    bool isAttack;          // 공격 중인가
    

    int combo
    {
        get
        {
            return anim.GetInteger("combo");
        }
        set
        {
            anim.SetInteger("combo", value);
        }
    }
   
    private void Start()
    {
        stat = GetComponent<Stateable>();

        controller = GetComponent<CharacterController>();
        
        hp = stat.Stat.hp;
        maxHp = stat.Stat.maxHp;
        levelExperience = stat.Stat.levelExperience;
        level = stat.Stat.level;

        StartCoroutine(ComboReset());
    }
    private void Update()
    {
        isGround = controller.isGrounded;
        
        if (!isDead)
        {
            Movement();             // 이동.
            Jump();                 // 점프.
            StatusCheck();
            // GetMouseButtonDown(0:왼쪽, 1:오른쪽, 2:휠)
            if (Input.GetMouseButtonDown(0) && !isAttack && combo < 3)
            {
                Attack();
            }
        }

        Gravity();
    }

    //캐릭터 이벤트
    public void StatusCheck()
    {
        if (levelExperience == 100)
        {
            level += 1;
            levelExperience = 0;
        }        

    }

    public void OnDead()
    {
        isDead = true;

        anim.SetTrigger("onDead");
    }
    void Attack()
    {
        // 연속 공격 타이머.
        if (comboReset != null)
            StopCoroutine(comboReset);

        // 변수 지정.
        isAttack = true;
        combo += 1;
        anim.SetTrigger("onAttack");
    }

    public void OnHit()
    {
        comboReset = StartCoroutine(ComboReset());
        OnEndAttack();

        attackable.Attack();        // 공격!!
    }
    public void OnEndAttack()
    {
        isAttack = false;
    }

    IEnumerator ComboReset()
    {
        float comboTime = comboDelay;                       // 연속 공격 시간.

        while ((comboTime -= Time.deltaTime) > 0.0f)       // 시간이 다 되었다면.
            yield return null;

        combo = 0;                                          // 콤보를 초기화.
    }
    public void OnDamaged()
    {

    }

    public void OnSword()   //검 착용 함수 검을 습득할때 실행, 검을 버릴때 실행
    {
        if (isSword)
        {
            isSword = false;
            return;
        }
        anim.SetTrigger("onSword");
    }



    //이동 관련 기본 조작
    private void Gravity()
    {
        if (isGround && velocityY < 0f)         
        {
            velocityY = -2f;                       
        }

        velocityY += gravity * Time.deltaTime;
        controller.Move(new Vector3(0f, velocityY, 0f) * Time.deltaTime);

        anim.SetFloat("velocityY", velocityY);
    }
    private void Movement()
    {
        // Input.GetAxisRaw = -1, 0  1.
        // Input.GetAxis = -1.0f ~ 1.0f.
        inputX = Input.GetAxis("Horizontal");       // 키보드 좌,우 (좌측,우측)
        inputY = Input.GetAxis("Vertical");         // 키보드 상,하 (정면,후면)

        if (inputX == -1 && inputY == 0 || inputX == 1 && inputY == 0)
            moveSpeed += 2 * Time.deltaTime;
                         

        // transform.방향 => 내 기준 방향 (로컬 좌표)
        Vector3 direction = (transform.right * inputX) + (transform.forward * inputY);
        controller.Move(direction * moveSpeed * Time.deltaTime);
    }
    private void Jump()
    {
        if (isGround && Input.GetButtonDown("Jump"))
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.SetTrigger("onJump");
        }
    }
}
