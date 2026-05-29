using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private Image energyBar;

    [Header("Weapon Settings")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private float attackCooldown = 0.3f;
    private bool isCharging;
    private float chargeTime;
    public float AttackCooldown => attackCooldown;

    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    private Vector2 moveInput;
    private Animator anim;
    private Camera mainCam;

    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private float nextAttackTime = 0f;
    private float comboResetTimer = 0f;
    [SerializeField] private float comboWindow = 1.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        weapon.player = this;
        mainCam = Camera.main;
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        moveInput = (isCharging ? Vector2.zero : moveInput);

        anim.SetFloat("vel", moveInput.sqrMagnitude);

        if (Time.time >= nextAttackTime)
        {
            AimTowardsMouse();
        }

        if (comboStep > 0 && Time.time > comboResetTimer)
        {
            comboStep = 0;
        }

        float progress = (Time.time - lastAttackTime) / attackCooldown;
        energyBar.fillAmount = Mathf.Clamp01(progress);

        chargeTime += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && weapon != null && !isCharging)
        {
            anim.CrossFade("attack", 0.1f);
            ProcessAttackCombo();
        }
        else if (Input.GetMouseButtonDown(1) && Time.time >= nextAttackTime && weapon != null && !isCharging)
        {
            chargeTime = 0;
            isCharging = true;
            anim.CrossFade("charge", 0.1f);
        }
        else if (Input.GetMouseButtonUp(1) && isCharging)
        {
            isCharging = false;
            anim.CrossFade("idle", 0.1f);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void ProcessAttackCombo()
    {
        weapon.Attack(comboStep);

        comboStep++;
        if (comboStep > 1)
        {
            comboStep = 0;
        }

        lastAttackTime = Time.time;
        nextAttackTime = Time.time + attackCooldown;
        comboResetTimer = Time.time + comboWindow;
    }

    private void AimTowardsMouse()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z);
        Vector3 mousePos = mainCam.ScreenToWorldPoint(mouseScreenPos);
        
        Vector3 aimDirection = mousePos - transform.position;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        playerSprite.flipX = mousePos.x > transform.position.x;

        if (weapon != null)
        {
            weaponTransform.rotation = Quaternion.Euler(0, 0, angle);

            Vector3 weaponScale = Vector3.one;
            
            if (angle > 90f || angle < -90f)
            {
                weaponScale.y = -1f;
            }

            weaponTransform.localScale = weaponScale;
        }
    }
}