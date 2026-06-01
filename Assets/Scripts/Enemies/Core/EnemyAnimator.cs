using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    private Animator anim;

    public event Action OnAnimationActionTriggered;

    private void Awake() => anim = GetComponent<Animator>();

    public void PlayAnimation(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    public void TriggerAttackAction()
    {
        OnAnimationActionTriggered?.Invoke();
    }
}