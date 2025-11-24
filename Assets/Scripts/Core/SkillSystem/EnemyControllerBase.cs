using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains;
using MoreMountains.TopDownEngine;

[RequireComponent(typeof(Health))]
public class EnemyControllerBase : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int baseDamage = 5;
    [SerializeField] private int enemyIndex = 0;
    [SerializeField] private float timerToStartFollow = 0.2f;
    [SerializeField] private float distanceToResetPosition = 0.2f;

    private Animator anim;
    private Rigidbody2D rb;
    private Health playerHealth;
    private bool canFollowTarget = true;
    private bool canMove = true;
    private bool canApplyDamage = true;

    public bool IsDead { get; private set; } = false;

    private void Start()
    {
        spriteRender = GetComponentInChildren<SpriteRenderer>();
        anim = spriteRender.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Setup health events
        playerHealth = GetComponent<Health>();
        playerHealth.OnDeath += () => IsDead = true;

        SetCharacterData(enemyIndex);
    }

    private Coroutine smoothMovementCoroutine;

    private void FixedUpdate()
    {
        if (IsDead) return;

        // Set walking animation
        if (canFollowTarget)
            anim.SetBool("IsWalking", rb.linearVelocity.x != 0 || rb.linearVelocity.y != 0);
        else anim.SetBool("IsWalking", false);

        if (player != null && canFollowTarget)
        {
            if (playerHealth.CurrentHealth <= 0)
            {
                canFollowTarget = false;
                StartCoroutine(SmoothStopMovement());
                return;
            }

            Vector2 targetPosition = new Vector2(player.transform.position.x, player.transform.position.y);
            Vector2 direction = (targetPosition - rb.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            Vector2 directionToTarget = targetPosition - rb.position;
            spriteRender.flipX = directionToTarget.x < 0;
        }
    }

    private void SetCharacterData(int enemyIndex) // (CharacterData newData)
    {
        // spriteRender.sprite = data.characterSprite;
        // speed = newData.status.speed;
        anim.SetInteger("Character", enemyIndex);
    }

    private IEnumerator SmoothStopMovement()
    {
        Vector2 initialVelocity = rb.linearVelocity;
        Vector2 targetVelocity = Vector2.zero;
        float elapsedTime = 0f;
        float smoothTime = 0.8f;

        while (elapsedTime < smoothTime)
        {
            if (IsDead) yield break;

            rb.linearVelocity = Vector2.Lerp(initialVelocity, targetVelocity, elapsedTime / smoothTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rb.linearVelocity = targetVelocity;
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDead) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!canFollowTarget)
                return;

            canFollowTarget = false;
            StartCoroutine(TryApplyDamageInPlayer());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Invoke(nameof(ResetCanFollowTarget), timerToStartFollow);
        }
    }

    private void ResetCanFollowTarget()
    {
        canFollowTarget = true;
    }

    private IEnumerator TryApplyDamageInPlayer()
    {
        while (!canFollowTarget)
        {
            if (canApplyDamage == false || playerHealth.CurrentHealth <= 0) yield break;

            if (player != null) playerHealth = player.GetComponent<Health>();
            if (playerHealth == null) yield break;

            canApplyDamage = false;
            playerHealth.Damage(baseDamage, gameObject, 0.1f, 0.2f, Vector3.zero);
            yield return new WaitForSeconds(0.5f);
            canApplyDamage = true;
        }
    }
}
