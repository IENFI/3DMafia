using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BookSortingGame : MonoBehaviour
{
    [SerializeField] private GameObject[] bookObjects; // 미리 배치된 책 오브젝트들
    [SerializeField] private float successMessageDuration = 2f;
    [SerializeField] private TMP_Text successMessageText;
    [SerializeField] public GameObject BookMinigameUI;

    private List<Book> activeBooks = new List<Book>();
    private int bookCount;
    private bool isDragging = false;
    private Book currentBook;
    private Vector2[] originalPositions;
    private Vector2 dragOffset;
    private int dragStartIndex;
    private Coroutine successMessageCoroutine;

    // 초기화 및 게임 시작/종료 관련 함수들

    // 책들의 원래 위치를 저장하고 Book 컴포넌트 추가
    private void InitializeBookPositions()
    {
        originalPositions = new Vector2[bookObjects.Length];

        for (int i = 0; i < bookObjects.Length; i++)
        {
            if (bookObjects[i] != null)
            {
                RectTransform rectTransform = bookObjects[i].GetComponent<RectTransform>();
                originalPositions[i] = rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero;

                if (bookObjects[i].GetComponent<Book>() == null)
                {
                    bookObjects[i].AddComponent<Book>();
                }
            }
            else
            {
                Debug.LogWarning($"Book object at index {i} is null. Using default position.");
                originalPositions[i] = Vector2.zero;
            }
        }
    }

    // UI 토글 (열기/닫기)
    public void ToggleBookSortingUI()
    {
        if (BookMinigameUI.activeSelf)
        {
            CloseGame();
        }
        else
        {
            OpenGame();
        }
    }

    // 게임 UI 열고 초기화
    private void OpenGame()
    {
        BookMinigameUI.SetActive(true);
        InitializeBookPositions();
        InitializeGame();
    }

    // 게임 초기화: 책 리셋, 랜덤 책 수 설정, 책 활성화
    private void InitializeGame()
    {
        ResetBooks();
        bookCount = Random.Range(5, 9);
        ActivateBooks();
        CheckInitialPlacements();
    }

    // 모든 책 상태 초기화
    private void ResetBooks()
    {
        for (int i = 0; i < bookObjects.Length; i++)
        {
            if (bookObjects[i] != null)
            {
                Book book = bookObjects[i].GetComponent<Book>();
                if (book != null)
                {
                    book.IsPlaced = false;
                    book.ResetPosition(originalPositions[i]);
                }
                bookObjects[i].SetActive(false);
                bookObjects[i].transform.SetSiblingIndex(i);
            }
        }
        activeBooks.Clear();
    }

    // 랜덤하게 책 활성화 및 초기화
    private void ActivateBooks()
    {
        List<int> numbers = Enumerable.Range(1, bookCount).ToList();
        List<int> availableSlots = Enumerable.Range(0, bookObjects.Length).OrderBy(x => Random.value).ToList();

        for (int i = 0; i < bookCount; i++)
        {
            int slotIndex = availableSlots[i];
            GameObject bookObj = bookObjects[slotIndex];

            if (bookObj != null)
            {
                bookObj.SetActive(true);
                Book book = bookObj.GetComponent<Book>();
                if (book != null)
                {
                    int number = numbers[Random.Range(0, numbers.Count)];
                    numbers.Remove(number);
                    book.Initialize(number, originalPositions[slotIndex]);
                    activeBooks.Add(book);
                }
            }
        }
    }

    // 초기 배치 시 이미 올바른 위치에 있는 책 체크
    private void CheckInitialPlacements()
    {
        foreach (var book in activeBooks)
        {
            int correctIndex = book.Number - 1;
            if (bookObjects[correctIndex] == book.gameObject)
            {
                book.IsPlaced = true;
            }
        }
    }

    // 게임 종료 및 초기화
    private void CloseGame()
    {
        if (successMessageCoroutine != null)
        {
            StopCoroutine(successMessageCoroutine);
            successMessageCoroutine = null;
            successMessageText.gameObject.SetActive(false);
        }
        BookMinigameUI.SetActive(false);
        ResetBooks();
    }

    // 게임 플레이 관련 함수들

    // 매 프레임마다 입력 체크
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && BookMinigameUI.activeSelf)
        {
            CloseGame();
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryPickUpBook();
        }

        if (Input.GetMouseButtonUp(0))
        {
            TryDropBook();
        }

        if (isDragging && currentBook != null)
        {
            DragBook();
        }
    }

    // 책 집기 시도
    private void TryPickUpBook()
    {
        Vector2 clickPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, null, out clickPosition);

        for (int i = 0; i < bookObjects.Length; i++)
        {
            if (bookObjects[i] != null && bookObjects[i].activeSelf)
            {
                RectTransform bookRect = bookObjects[i].GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(bookRect, Input.mousePosition, null))
                {
                    Book book = bookObjects[i].GetComponent<Book>();
                    if (book != null && !book.IsPlaced)
                    {
                        currentBook = book;
                        isDragging = true;
                        dragOffset = bookRect.anchoredPosition - clickPosition;
                        dragStartIndex = i;
                        break;
                    }
                }
            }
        }
    }

    // 책 드래그
    private void DragBook()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, null, out localPoint);
        currentBook.GetComponent<RectTransform>().anchoredPosition = localPoint + dragOffset;
    }

    // 책 놓기 시도
    private void TryDropBook()
    {
        if (currentBook != null)
        {
            int nearestIndex = FindNearestBookSlot(currentBook.GetComponent<RectTransform>().anchoredPosition);

            if (nearestIndex != -1)
            {
                if (nearestIndex == currentBook.Number - 1)  // 책의 올바른 위치인 경우
                {
                    PlaceBookCorrectly(currentBook, nearestIndex, dragStartIndex);
                }
                else  // 올바른 위치가 아닌 경우
                {
                    currentBook.ResetToInitialPosition();
                }
            }
            else
            {
                currentBook.ResetToInitialPosition();
            }

            isDragging = false;
            currentBook = null;
        }
    }

    // 책을 올바른 위치에 배치
    private void PlaceBookCorrectly(Book book, int correctIndex, int startIndex)
    {
        Book bookAtCorrectIndex = bookObjects[correctIndex]?.GetComponent<Book>();

        // 현재 책을 올바른 위치로 이동
        book.GetComponent<RectTransform>().anchoredPosition = originalPositions[correctIndex];
        book.UpdateInitialPosition(originalPositions[correctIndex]);
        book.IsPlaced = true;

        // bookObjects 배열 업데이트
        bookObjects[startIndex] = null;
        bookObjects[correctIndex] = book.gameObject;
        book.transform.SetSiblingIndex(correctIndex);

        // 원래 있던 책 이동 (있었다면)
        if (bookAtCorrectIndex != null)
        {
            bookAtCorrectIndex.GetComponent<RectTransform>().anchoredPosition = originalPositions[startIndex];
            bookAtCorrectIndex.UpdateInitialPosition(originalPositions[startIndex]);
            bookAtCorrectIndex.IsPlaced = (startIndex == bookAtCorrectIndex.Number - 1);
            bookObjects[startIndex] = bookAtCorrectIndex.gameObject;
            bookAtCorrectIndex.transform.SetSiblingIndex(startIndex);
        }

        CheckForCompletion();
    }

    // 가장 가까운 책 슬롯 찾기
    private int FindNearestBookSlot(Vector2 position)
    {
        return originalPositions
            .Select((pos, index) => new { Index = index, Distance = Vector2.Distance(pos, position) })
            .OrderBy(x => x.Distance)
            .First().Index;
    }

    // 모든 책이 올바른 위치에 있는지 확인
    private void CheckForCompletion()
    {
        if (activeBooks.TrueForAll(book => book.IsPlaced))
        {
            if (successMessageCoroutine != null)
            {
                StopCoroutine(successMessageCoroutine);
            }
            successMessageCoroutine = StartCoroutine(ShowSuccessMessage());
        }
    }

    // 성공 메시지 표시
    private IEnumerator ShowSuccessMessage()
    {
        successMessageText.text = "성공!";
        successMessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(successMessageDuration);
        successMessageText.gameObject.SetActive(false);
        successMessageCoroutine = null;
        CloseGame();
    }
}