using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CleanMirror : MinigameBase
{
    public Image dirtyImagePrefab; // 검은색 이미지를 위한 프리팹
    public int numberOfDirtySpots = 4; // 생성할 검은색 영역의 수
    public float cleaningSpeed = 2.0f; // 닦이는 속도
    public float cleaningRadius = 50f; // 닦이는 범위(반경)
    public Vector2 randomPositionRange = new Vector2(110f, 180f); // 검은색 영역이 나타날 수 있는 범위
    public float maxAttempts = 10; // 위치를 찾기 위한 최대 시도 횟수

    [SerializeField]
    private GameObject minigameManager;
    [SerializeField]
    private bool active = false;
    private int index = 2;
    [SerializeField]
    private MinigameInteraction minigame;

    private List<Image> dirtyImages = new List<Image>(); // 생성된 검은색 이미지들
    private List<RectTransform> dirtyImageRects = new List<RectTransform>(); // RectTransform 리스트

    void Start()
    {
        // 검은색 이미지들 랜덤 위치로 생성
        for (int i = 0; i < numberOfDirtySpots; i++)
        {
            Image newDirtyImage = Instantiate(dirtyImagePrefab, transform); // 프리팹을 복제하여 이미지 생성
            RectTransform rectTransform = newDirtyImage.GetComponent<RectTransform>();

            // 이미지를 겹치지 않게 배치하기 위한 시도
            bool validPosition = false;
            int attempts = 0;

            while (!validPosition && attempts < maxAttempts)
            {
                // 이미지 위치를 랜덤하게 설정
                rectTransform.anchoredPosition = new Vector2(
                    Random.Range(-randomPositionRange.x, randomPositionRange.x),
                    Random.Range(-randomPositionRange.y, randomPositionRange.y)
                );

                validPosition = true;

                // 다른 이미지들과 겹치는지 확인
                foreach (var otherRect in dirtyImageRects)
                {
                    if (IsOverlapping(rectTransform, otherRect))
                    {
                        validPosition = false;
                        break; // 겹친다면 새로운 위치 시도
                    }
                }

                attempts++;
            }

            if (validPosition)
            {
                dirtyImages.Add(newDirtyImage); // 생성된 이미지를 리스트에 추가
                dirtyImageRects.Add(rectTransform); // RectTransform을 리스트에 추가
            }
            else
            {
                Debug.LogWarning("이미지 배치 실패: 너무 많은 겹침 발생");
            }
        }
    }

    // RectTransform끼리 겹치는지 확인
    bool IsOverlapping(RectTransform rect1, RectTransform rect2)
    {
        // World 좌표로 변환된 Rect 계산
        Rect rect1World = GetWorldRect(rect1);
        Rect rect2World = GetWorldRect(rect2);

        return rect1World.Overlaps(rect2World);
    }

    // RectTransform의 World 좌표에서의 Rect를 반환
    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector2 size = new Vector2(rt.rect.width, rt.rect.height);
        return new Rect(corners[0], size);
    }

    public override void ReceiveToken()
    {
        Debug.Log("거울 닦기 미니게임이 시작되었습니다.");
        active = true;
    }

    public override void Deactivation()
    {
        active = false;
    }

    public override bool GetActive()
    {
        return active;
    }

    public override MinigameManager GetMinigameManager()
    {
        return minigameManager.GetComponent<MinigameManager>();
    }

    void Update()
    {
        // 마우스가 눌린 상태에서 드래그할 때만 닦기 처리
        if (Input.GetMouseButton(0))
        {
            for (int i = 0; i < dirtyImages.Count; i++)
            {
                Vector2 localMousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(dirtyImageRects[i], Input.mousePosition, null, out localMousePosition);

                // 마우스 위치가 dirtyImage 안에 있을 때만 처리
                if (dirtyImageRects[i].rect.Contains(localMousePosition))
                {
                    CleanAtPosition(localMousePosition, dirtyImages[i]);
                }
            }
        }
    }

    void CleanAtPosition(Vector2 localMousePosition, Image dirtyImage)
    {
        // dirtyImage의 알파값을 줄이는 로직
        Color color = dirtyImage.color;

        // 클릭한 위치 주변의 알파값을 감소시킴 (닦는 범위 적용)
        color.a -= cleaningSpeed * Time.deltaTime;
        color.a = Mathf.Clamp(color.a, 0, 1); // 알파값을 0~1 사이로 고정
        dirtyImage.color = color;

        // 알파값이 0이 되면 해당 이미지가 사라짐
        if (color.a <= 0)
        {
            dirtyImage.gameObject.SetActive(false); // 이미지 비활성화
            CheckIfAllClean(); // 모든 영역이 닦였는지 확인
        }
    }

    // 모든 검은색 이미지가 닦였는지 확인
    void CheckIfAllClean()
    {
        // 찰나의 드래그 문제로 돈이 더 벌릴 가능성 존재. 완료하면 0.1초 뒤에 들어오게 해도 될 듯
        foreach (var dirtyImage in dirtyImages)
        {
            if (dirtyImage.gameObject.activeSelf)
            {
                return; // 아직 닦이지 않은 이미지가 있으면 종료
            }
        }

        // 모든 검은색 이미지가 닦였다면 게임 종료
        Debug.Log("거울 닦기 미니게임이 종료되었습니다.");
        minigame.ExitCode = true;
        active = false;
        GetMinigameManager().SuccessMission(index);
        gameObject.SetActive(false);
    }
}