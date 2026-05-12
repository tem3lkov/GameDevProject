using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class BossPhase {
    public string phaseName = "Phase 1";
    [Range(0f, 1f)] public float healthThreshold;
    public float actionCooldown = 2f;
    public EnemyAttack[] allowedAttacks;
}

public class Boss : Enemy {
    [Header("Boss Setup")]
    public BossPhase[] phases;
    private int currentPhaseIndex = 0;

    public static event Action<float> OnBossHealthUpdatedUI;
    public static event Action<bool> OnBossFightActiveUI;

    protected override bool HasLineOfSight() {
        return Target != null;
    }

    protected override void Start() {
        base.Start();
        System.Array.Sort(phases, (a, b) => b.healthThreshold.CompareTo(a.healthThreshold));

        OnBossFightActiveUI?.Invoke(true);
        OnBossHealthUpdatedUI?.Invoke(1f);
    }

    protected virtual void OnEnable() {
        OnHealthChanged += CheckPhaseTransition;
        OnHealthChanged += UpdateBossUI;
    }

    protected virtual void OnDisable() {
        OnHealthChanged -= CheckPhaseTransition;
        OnHealthChanged -= UpdateBossUI;

        OnBossFightActiveUI?.Invoke(false);
    }

    private void UpdateBossUI(float healthPercent) {
        OnBossHealthUpdatedUI?.Invoke(healthPercent);
    }

    private void CheckPhaseTransition(float healthPercent) {
        if (currentPhaseIndex >= phases.Length - 1) return;

        if (healthPercent <= phases[currentPhaseIndex + 1].healthThreshold) {
            currentPhaseIndex++;
            Debug.Log($"Boss entered {phases[currentPhaseIndex].phaseName}!");
        }
    }

    protected override void HandleAggroBehavior() {
        if (!IsAttacking && phases.Length > 0) {
            StartCoroutine(AttackCycleRoutine());
        }
    }

    private IEnumerator AttackCycleRoutine() {
        IsAttacking = true;
        BossPhase currentPhase = phases[currentPhaseIndex];

        Rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(currentPhase.actionCooldown);

        if (currentPhase.allowedAttacks.Length > 0 && Target != null) {
            int randomIndex = UnityEngine.Random.Range(0, currentPhase.allowedAttacks.Length);
            EnemyAttack chosenAttack = currentPhase.allowedAttacks[randomIndex];

            yield return StartCoroutine(chosenAttack.Execute(this));
        }

        IsAttacking = false;
    }
}