using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Knockback Settings")]
    public float knockbackThrust = 5f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    public bool IsKnockedBack => isKnockedBack;

    [Header("Visual Settings (DOTween)")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.15f;
    private Color originalColor;
    [SerializeField] private GameObject damageIndicatorPrefab;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("EnemyController: No GameObject with the tag 'Player' was found in the scene!");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player != null)
        {
            MoveTowardPlayer();
            FlipSprite();
        }
    }

    void MoveTowardPlayer()
    {
        float step = moveSpeed * Time.deltaTime;
        
        transform.position = Vector2.MoveTowards(transform.position, player.position, step);
        anim.SetBool("move", true);
    }

    void FlipSprite()
    {
        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }


    public void TakeDamage(int damageAmount)
    {
        Vector2 knockbackDirection = (transform.position - player.position).normalized;

        spriteRenderer.DOColor(damageColor, flashDuration)
            .OnComplete(() => spriteRenderer.DOColor(originalColor, flashDuration));

        StartCoroutine(KnockbackRoutine(knockbackDirection));

        if (damageIndicatorPrefab != null)
        {
            GameObject indicator = Instantiate(damageIndicatorPrefab, transform.position, Quaternion.identity);
            indicator.GetComponent<DamageIndicator>().Setup(damageAmount);
        }
    }

    private IEnumerator KnockbackRoutine(Vector2 direction)
    {
        isKnockedBack = true;
        anim.SetBool("move", false);

        rb.velocity = direction * knockbackThrust;

        yield return new WaitForSeconds(knockbackDuration);

        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }
}