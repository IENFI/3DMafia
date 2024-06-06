using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private GameObject attackCollision;
    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnMovement(float horizontal, float vertical)
    {
        animator.SetFloat("horizontal", horizontal);
        animator.SetFloat("vertical", vertical);
    }

    public void Kill()
    {
        animator.SetTrigger("kill");
    }

    public void Death()
    {
        animator.SetTrigger("death");
        // 필드에서 추방하는 코드 추가하기
    }

    public void OnAttackCollision()
    {
        attackCollision.SetActive(true);
    }

    // 애니메이션의 동작 시간을 얻는 함수
    public AnimatorStateInfo GetAnimatorTime()
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }
}
