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

    [Header("Interaction")]
    [SerializeField] KeyCode interactHotKey;            // ��ȣ�ۿ� Ű.
    [SerializeField] float searchRadius;                // Ž�� ����.

    [Header("Sword")]
    [SerializeField] Weapon sword;
    [SerializeField] Weapon sword1;
    [SerializeField] Weapon swordcase;
    [SerializeField] Transform originarPivot;
    [SerializeField] Transform swordPivot;

    [Range(1.0f, 4.0f)]
    [SerializeField] float gravityScale;

    Stateable stat;
    IInteraction interactionTarget;
    public float hp { get; private set; }      // ���� ü��
    public float maxHp { get; private set; }      // �ִ� ü��.
    public float experience { get; private set; }         // ����ġ
    public float levelExperience { get; private set; }     // �������ٰ���ġ
    public int level { get; private set; }                   //����

    CharacterController controller;
    Coroutine comboReset;

    Item[] inventory = new Item[8];

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
    bool isUnderAttack
    {
        get
        {
            return anim.GetBool("isUnderAttack");
        }
        set
        {
            anim.SetBool("isUnderAttack", value);
        }
    }
    float gravity => GRAVITY * gravityScale;

    bool isDead
    {
        get
        {
            return anim.GetBool("isDead");
        }
        set
        {
            anim.SetBool("isDead", value);
        }
    }           // �׾��°�
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

        sword.gameObject.SetActive(false);
        sword1.gameObject.SetActive(false);
        swordcase.gameObject.SetActive(false);

        StartCoroutine(ComboReset());
        InventoryUI.Instance.ResetInventory();
        InventoryUI.Instance.UpdateInventory(inventory);

    }
    private void Update()
    {
        isGround = controller.isGrounded;

        
        if (!isDead || !isUnderAttack)
        {
            Movement();             // �̵�.
            Jump();                 // ����.
            StatusCheck();
            // GetMouseButtonDown(0:����, 1:������, 2:��)
            if (Input.GetMouseButtonDown(0) && !isAttack && combo < 3 && isSword)
            {
                Attack();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                anim.SetTrigger("onSword");
                sword.gameObject.SetActive(true);
                isSword = true;
            }
        }

        Gravity();
        Interaction();
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
        gameObject.layer = LayerMask.NameToLayer("Player_Dead");
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
        anim.SetTrigger("onHit");
        isUnderAttack = true;

        CancelInvoke(nameof(ResetUnderAttack));     // ������ �ɾ�� Invoke ���.
        Invoke(nameof(ResetUnderAttack), 1.5f);     // 1.5�� �Ŀ� �Լ� ȣ��.
    }
    
    void ResetUnderAttack()
    {
        isUnderAttack = false;
    }
    public void OnSword()   //�� ���� �Լ� ���� �����Ҷ� ����, ���� ������ ����
    {
        if (isSword)
        {
            isSword = false;
            return;
        }
        anim.SetTrigger("onSword");
        sword.gameObject.SetActive(false);
    }
    private void OnSword_Event()
    {
        if (!isSword)
        {
            //sword.transform.SetParent(swordPivot.transform);
            //sword.transform.localPosition = Vector3.zero;
            //sword.transform.localEulerAngles = Vector3.zero;
            sword.gameObject.SetActive(false);
            sword1.gameObject.SetActive(true);
            swordcase.gameObject.SetActive(true);

            //isSword = true;
        }
        else if (isSword)
        {
            //sword.transform.SetParent(originarPivot.transform);
            //sword.transform.localPosition = Vector3.zero;
            //sword.transform.localEulerAngles = Vector3.zero;
            sword.gameObject.SetActive(true);

            //isSword = false;
        }
    }


    //�κ��丮
    void Interaction()
    {
        // Vector3.���� : ���� ��ǥ ���� ����.
        // Transform.���� : (�� ����)���� ��ǥ ���� ����.

        // ��ü �˻��� �� ���� ���ʷ� ���� ���� ��ȣ�ۿ�.
        interactionTarget = null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
        foreach (Collider collider in colliders)
        {
            interactionTarget = collider.GetComponent<IInteraction>();
            if (interactionTarget != null)
            {
                // �������̽��� ������ ����� ������ ����� ������ ���.
                InteractionUI.Instance.Open(interactHotKey.ToString(), interactionTarget.GetName());
                break;
            }
        }

        // �ƹ��͵� �˻����� �ʾҴٸ� UI�� �ݴ´�.
        if (interactionTarget == null)
        {
            InteractionUI.Instance.Close();
        }
        // ��ȣ�ۿ� Ű�� �����ٸ�.
        else if (Input.GetKeyDown(interactHotKey))
        {
            interactionTarget.OnInteraction();
        }
    }
    void OnStart()
    {
        InventoryUI.Instance.onChangedInven += OnChangedInven;
              

    }
    private void OnChangedInven(int before, int after)
    {
        // ������ �ƴ� ĭ���� ��������.
        if (after == -1)
        {

        }
        else
        {           
            InventoryUI.Instance.UpdateInventory(inventory);    // UI���� �׷��޶�� ��û.
        }
    }
    public bool AddItem(Item item)
    {
        // �߰��Ϸ��� �������� ���� ���.
        if (item == null)
            return false;

        //���� �������� �ִ��� ã�´�.
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null && inventory[i].Equals(item))
            {
                inventory[i].Add(item);
                InventoryUI.Instance.UpdateInventory(inventory);
                return true;
            }
        }
        //����� Ž��
        int emptyIndex = EmptyInven();

        //������� �������
        if (emptyIndex == -1)
            return false;

        //������� ������ �߰�.
        inventory[emptyIndex] = item;
        InventoryUI.Instance.UpdateInventory(inventory);
        return true;

    }
    private int EmptyInven()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                return i;
            }
        }

        return -1;
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
