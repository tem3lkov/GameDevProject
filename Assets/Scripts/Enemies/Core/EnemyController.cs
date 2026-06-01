using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyController : MonoBehaviour, IDamageable
{
    public EnemyDetailsSO details;

    public Transform Target { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public EnemyAnimator Anim { get; private set; }

    public bool IsAttacking { get; set; } = false;

    public event Action<EnemyController> OnDeath;

    // Global UI Events for Bosses
    public static event Action<float> OnBossHealthUpdatedUI;
    public static event Action<bool> OnBossFightActiveUI;

    private float currentHealth;
    private int currentPhaseIndex = 0;
    private float nextAttackTime;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<EnemyAnimator>();
        currentHealth = details.maxHealth;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) Target = player.transform;

        System.Array.Sort(details.phases, (a, b) => b.healthThreshold.CompareTo(a.healthThreshold));

        if (details.isBoss)
        {
            OnBossFightActiveUI?.Invoke(true);
            OnBossHealthUpdatedUI?.Invoke(1f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out IDamageable hit))
        {
            hit.TakeDamage(details.damageToPlayer);
        }
    }

    private void Update()
    {
        if (Target == null || IsAttacking) return;
        HandleAttacks();
    }

    private void HandleAttacks()
    {
        if (details.phases.Length == 0 || Time.time < nextAttackTime) return;

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
        if (currentPhaseIndex >= details.phases.Length - 1) return;

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
}