using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private GameObject attackCollision;
    private Animator animator;
    // KillTimer 스크립트 참조
    public KillTimer killTimer;


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
        // KillTimer의 StartCooldown 메서드 호출
        if (killTimer != null)
        {
            killTimer.StartCooldown();
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("isMafia"))
        {
            animator.SetTrigger("kill");
        }
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
