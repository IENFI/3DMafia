using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float time = 300f;  // 기본값 설정
    private float curTime;  // 인스펙터에 노출되지 않음

    private Light directionalLight;
    private Coroutine blinkCoroutine;
    private bool isBlinking;
    private fillAmountController fillController;

    int minute;
    int second;

    [SerializeField] private GameObject Merchent; // NPC 캐릭터 GameObject를 저장할 변수

    private void Awake()
    {
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
        curTime = time;
        fillController = FindObjectOfType<fillAmountController>(); // fillAmountController 찾기
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (true)
        {
            curTime = time;  // 타이머를 초기화합니다.
            isBlinking = false;

            if (fillController != null)
            {
                fillController.ResetFillAmount(); // 각 사이클 시작 시 fill amount 초기화
            }

            text.color = Color.black;  // 텍스트 색상을 초기화합니다.

            while (curTime > 0)
            {
                curTime -= Time.deltaTime;
                minute = (int)curTime / 60;
                second = (int)curTime % 60;
                text.text = minute.ToString("00") + ":" + second.ToString("00");

                if (curTime <= 10f && !isBlinking)  // 10초 이하로 남았을 때
                {
                    text.color = Color.red;
                    blinkCoroutine = StartCoroutine(BlinkText());
                    isBlinking = true;
                }

                yield return null;
            }

            // 타이머가 종료되었을 때
            Debug.Log("시간 종료");
            curTime = 0;

            // 자연광 전환
            directionalLight.enabled = !directionalLight.enabled;

            // NPC 캐릭터 표시 여부 변경
            Merchent.SetActive(!directionalLight.enabled);

            // 반짝임 멈추기
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                text.color = Color.black;  // 색상 초기화
            }

            // fillAmount 초기화
            if (fillController != null)
            {
                fillController.ResetFillAmount();
            }

            // 다음 사이클을 위해 잠시 대기
            yield return new WaitForSeconds(1f);  // 전환 후 1초 대기 (선택 사항)
        }
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            text.enabled = !text.enabled;  // 텍스트의 활성화 상태를 토글
            yield return new WaitForSeconds(0.5f);  // 반짝임 속도 조절
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 필요한 경우 Start에서 초기화할 수 있습니다.
    }

    // Update is called once per frame
    void Update()
    {

    }
}
