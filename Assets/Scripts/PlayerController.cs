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
    [SerializeField] KeyCode interactHotKey;            // 상호작용 키.
    [SerializeField] float searchRadius;                // 탐지 범위.

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
    public float hp { get; private set; }      // 현재 체력
    public float maxHp { get; private set; }      // 최대 체력.
    public float experience { get; private set; }         // 경험치
    public float levelExperience { get; private set; }     // 레벨업바경험치
    public int level { get; private set; }                   //레벨

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
    }           // 죽었는가
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
            Movement();             // 이동.
            Jump();                 // 점프.
            StatusCheck();
            // GetMouseButtonDown(0:왼쪽, 1:오른쪽, 2:휠)
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
        gameObject.layer = LayerMask.NameToLayer("Player_Dead");
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
        anim.SetTrigger("onHit");
        isUnderAttack = true;

        CancelInvoke(nameof(ResetUnderAttack));     // 이전에 걸어둔 Invoke 취소.
        Invoke(nameof(ResetUnderAttack), 1.5f);     // 1.5초 후에 함수 호출.
    }
    
    void ResetUnderAttack()
    {
        isUnderAttack = false;
    }
    public void OnSword()   //검 착용 함수 검을 습득할때 실행, 검을 버릴때 실행
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


    //인벤토리
    void Interaction()
    {
        // Vector3.방향 : 절대 좌표 상의 방향.
        // Transform.방향 : (내 기준)로컬 좌표 상의 방향.

        // 전체 검색을 해 가장 최초로 잡힌 대상과 상호작용.
        interactionTarget = null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
        foreach (Collider collider in colliders)
        {
            interactionTarget = collider.GetComponent<IInteraction>();
            if (interactionTarget != null)
            {
                // 인터페이스를 구현한 대상을 가져와 대상의 정보를 출력.
                InteractionUI.Instance.Open(interactHotKey.ToString(), interactionTarget.GetName());
                break;
            }
        }

        // 아무것도 검색되지 않았다면 UI를 닫는다.
        if (interactionTarget == null)
        {
            InteractionUI.Instance.Close();
        }
        // 상호작용 키를 눌렀다면.
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
        // 슬롯이 아닌 칸으로 움직였다.
        if (after == -1)
        {

        }
        else
        {           
            InventoryUI.Instance.UpdateInventory(inventory);    // UI에게 그려달라고 요청.
        }
    }
    public bool AddItem(Item item)
    {
        // 추가하려는 아이템이 없는 경우.
        if (item == null)
            return false;

        //같은 아이템이 있는지 찾는다.
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null && inventory[i].Equals(item))
            {
                inventory[i].Add(item);
                InventoryUI.Instance.UpdateInventory(inventory);
                return true;
            }
        }
        //빈공간 탐색
        int emptyIndex = EmptyInven();

        //빈공간이 없을경우
        if (emptyIndex == -1)
            return false;

        //빈공간에 아이템 추가.
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
