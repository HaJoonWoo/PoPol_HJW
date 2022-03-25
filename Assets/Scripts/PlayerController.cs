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

    public float hp { get; private set; }      // ���� ü��
    public float maxHp { get; private set; }      // �ִ� ü��.
    public float experience { get; private set; }         // ����ġ
    public float levelExperience { get; private set; }     // �������ٰ���ġ
    public int level { get; private set; }                   //����

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

    bool isDead;            // �׾��°�
    bool isAttack;          // ���� ���ΰ�
    

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
            Movement();             // �̵�.
            Jump();                 // ����.
            StatusCheck();
            // GetMouseButtonDown(0:����, 1:������, 2:��)
            if (Input.GetMouseButtonDown(0) && !isAttack && combo < 3)
            {
                Attack();
            }
        }

        Gravity();
    }

    //ĳ���� �̺�Ʈ
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
        // ���� ���� Ÿ�̸�.
        if (comboReset != null)
            StopCoroutine(comboReset);

        // ���� ����.
        isAttack = true;
        combo += 1;
        anim.SetTrigger("onAttack");
    }

    public void OnHit()
    {
        comboReset = StartCoroutine(ComboReset());
        OnEndAttack();

        attackable.Attack();        // ����!!
    }
    public void OnEndAttack()
    {
        isAttack = false;
    }

    IEnumerator ComboReset()
    {
        float comboTime = comboDelay;                       // ���� ���� �ð�.

        while ((comboTime -= Time.deltaTime) > 0.0f)       // �ð��� �� �Ǿ��ٸ�.
            yield return null;

        combo = 0;                                          // �޺��� �ʱ�ȭ.
    }
    public void OnDamaged()
    {

    }

    public void OnSword()   //�� ���� �Լ� ���� �����Ҷ� ����, ���� ������ ����
    {
        if (isSword)
        {
            isSword = false;
            return;
        }
        anim.SetTrigger("onSword");
    }



    //�̵� ���� �⺻ ����
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
        inputX = Input.GetAxis("Horizontal");       // Ű���� ��,�� (����,����)
        inputY = Input.GetAxis("Vertical");         // Ű���� ��,�� (����,�ĸ�)

        if (inputX == -1 && inputY == 0 || inputX == 1 && inputY == 0)
            moveSpeed += 2 * Time.deltaTime;
                         

        // transform.���� => �� ���� ���� (���� ��ǥ)
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
