using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Enemy Data")]
    public EnemyDetailsSO details;

    public Transform Target { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public EnemyAnimator Anim { get; private set; }
    public SpriteRenderer SpriteRend { get; private set; }
    public bool IsAttacking { get; set; } = false;

    public event Action<EnemyController> OnDeath;
    public static event Action<float> OnBossHealthUpdatedUI;
    public static event Action<bool> OnBossFightActiveUI;

    private float currentHealth;
    private int currentPhaseIndex = 0;
    private float nextAttackTime;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<EnemyAnimator>();
        SpriteRend = GetComponentInChildren<SpriteRenderer>();

        currentHealth = details.maxHealth;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) Target = player.transform;

        if (details.phases != null && details.phases.Length > 0)
        {
            Array.Sort(details.phases, (a, b) => b.healthThreshold.CompareTo(a.healthThreshold));

            nextAttackTime = Time.time + details.phases[0].timeBetweenAttacks;
        } else
        {
            nextAttackTime = Time.time + 1f;
        }

        if (details.isBoss)
        {
            OnBossFightActiveUI?.Invoke(true);
            OnBossHealthUpdatedUI?.Invoke(1f);
        }
    }

    private void Update()
    {
        HandleSpriteFlipping();

        if (Target == null || IsAttacking) return;
        HandleAttacks();
    }

    private void HandleSpriteFlipping()
    {
        if (SpriteRend == null) return;

        if (Rb.linearVelocity.x > 0.1f)
        {
            SpriteRend.flipX = false;
        } else if (Rb.linearVelocity.x < -0.1f)
        {
            SpriteRend.flipX = true;
        }
    }

    private void HandleAttacks()
    {
        if (details.phases == null || details.phases.Length == 0 || Time.time < nextAttackTime) return;

        BossPhaseSO activePhase = details.phases[currentPhaseIndex];
        List<EnemyAttackSO> validAttacks = new List<EnemyAttackSO>();

        foreach (var attack in activePhase.allowedAttacks)
        {
            if (attack.CanExecute(this))
            {
                validAttacks.Add(attack);
            }
        }

        if (validAttacks.Count > 0)
        {
            EnemyAttackSO chosenAttack = validAttacks[UnityEngine.Random.Range(0, validAttacks.Count)];
            StartCoroutine(RunAttackSequence(chosenAttack));
        }
    }

    private IEnumerator RunAttackSequence(EnemyAttackSO attackToRun)
    {
        IsAttacking = true;

        yield return StartCoroutine(attackToRun.ExecuteAttack(this));

        IsAttacking = false;

        nextAttackTime = Time.time + attackToRun.cooldownTime + details.phases[currentPhaseIndex].timeBetweenAttacks;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (details.isBoss) OnBossHealthUpdatedUI?.Invoke(currentHealth / details.maxHealth);

        CheckPhaseTransition();

        if (currentHealth <= 0) Die();
    }

    private void CheckPhaseTransition()
    {
        if (details.phases == null || currentPhaseIndex >= details.phases.Length - 1) return;

        float healthPercent = currentHealth / details.maxHealth;
        if (healthPercent <= details.phases[currentPhaseIndex + 1].healthThreshold)
        {
            currentPhaseIndex++;
            Debug.Log($"{details.enemyName} entered {details.phases[currentPhaseIndex].phaseName}!");
        }
    }

    public void Die()
    {
        if (details.isBoss) OnBossFightActiveUI?.Invoke(false);

        StopAllCoroutines();

        OnDeath?.Invoke(this);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DealContactDamage(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DealContactDamage(collision.gameObject);
    }

    private void DealContactDamage(GameObject hitObject)
    {
        if (hitObject.CompareTag("Player"))
        {
            if (hitObject.TryGetComponent(out IDamageable playerHit))
            {
                playerHit.TakeDamage(details.damageToPlayer);
            }
        }
    }
}