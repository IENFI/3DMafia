using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BookSortingGame : MinigameBase
{
    [SerializeField] private GameObject[] bookObjects; // �̸� ��ġ�� å ������Ʈ��
    [SerializeField] private float successMessageDuration = 2f;
    [SerializeField] private TMP_Text successMessageText;
    [SerializeField] public GameObject BookMinigameUI;
    [SerializeField]
    private GameObject minigameManager;

    [SerializeField]
    private MinigameInteraction minigame;
    private int index = 4;
    private bool active = false;


    private List<Book> activeBooks = new List<Book>();
    private int bookCount;
    private bool isDragging = false;
    private Book currentBook;
    private Vector2[] originalPositions;
    private Vector2 dragOffset;
    private int dragStartIndex;
    private Coroutine successMessageCoroutine;


    private void OnEnable()
    {
        InitializeBookPositions();
        InitializeGame();
    }

    // �ʱ�ȭ �� ���� ����/���� ���� �Լ���

    // å���� ���� ��ġ�� �����ϰ� Book ������Ʈ �߰�
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

    // UI ��� (����/�ݱ�)
    public void ToggleBookSortingUI()
    {
        if (BookMinigameUI.activeSelf)
        {
            CloseGame();
        }
        //else
        //{
        //    OpenGame();
        //}
    }

    // ���� UI ���� �ʱ�ȭ
    //private void OpenGame()
    //{
    //    BookMinigameUI.SetActive(true);
    //    InitializeBookPositions();
    //    InitializeGame();
    //}

    // ���� �ʱ�ȭ: å ����, ���� å �� ����, å Ȱ��ȭ
    private void InitializeGame()
    {
        ResetBooks();
        bookCount = Random.Range(5, 9);
        ActivateBooks();
        CheckInitialPlacements();
    }

    // ��� å ���� �ʱ�ȭ
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

    // �����ϰ� å Ȱ��ȭ �� �ʱ�ȭ
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
    public override void ReceiveToken()
    {
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
    // �ʱ� ��ġ �� �̹� �ùٸ� ��ġ�� �ִ� å üũ
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

    // ���� ���� �� �ʱ�ȭ
    private void CloseGame()
    {
        if (successMessageCoroutine != null)
        {
            StopCoroutine(successMessageCoroutine);
            successMessageCoroutine = null;
            successMessageText.gameObject.SetActive(false);
        }
        //BookMinigameUI.SetActive(false);
        ResetBooks();
        minigame.ExitCode = true;
        active = false;
        GetMinigameManager().SuccessMission(index);
        this.gameObject.SetActive(false);
    }

    // ���� �÷��� ���� �Լ���

    // �� �����Ӹ��� �Է� üũ
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

    // å ���� �õ�
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

    // å �巡��
    private void DragBook()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, null, out localPoint);
        currentBook.GetComponent<RectTransform>().anchoredPosition = localPoint + dragOffset;
    }

    // å ���� �õ�
    private void TryDropBook()
    {
        if (currentBook != null)
        {
            int nearestIndex = FindNearestBookSlot(currentBook.GetComponent<RectTransform>().anchoredPosition);

            if (nearestIndex != -1)
            {
                if (nearestIndex == currentBook.Number - 1)  // å�� �ùٸ� ��ġ�� ���
                {
                    PlaceBookCorrectly(currentBook, nearestIndex, dragStartIndex);
                }
                else  // �ùٸ� ��ġ�� �ƴ� ���
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

    // å�� �ùٸ� ��ġ�� ��ġ
    private void PlaceBookCorrectly(Book book, int correctIndex, int startIndex)
    {
        Book bookAtCorrectIndex = bookObjects[correctIndex]?.GetComponent<Book>();

        // ���� å�� �ùٸ� ��ġ�� �̵�
        book.GetComponent<RectTransform>().anchoredPosition = originalPositions[correctIndex];
        book.UpdateInitialPosition(originalPositions[correctIndex]);
        book.IsPlaced = true;

        // bookObjects �迭 ������Ʈ
        bookObjects[startIndex] = null;
        bookObjects[correctIndex] = book.gameObject;
        book.transform.SetSiblingIndex(correctIndex);

        // ���� �ִ� å �̵� (�־��ٸ�)
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

    // ���� ����� å ���� ã��
    private int FindNearestBookSlot(Vector2 position)
    {
        return originalPositions
            .Select((pos, index) => new { Index = index, Distance = Vector2.Distance(pos, position) })
            .OrderBy(x => x.Distance)
            .First().Index;
    }

    // ��� å�� �ùٸ� ��ġ�� �ִ��� Ȯ��
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

    // ���� �޽��� ǥ��
    private IEnumerator ShowSuccessMessage()
    {
        successMessageText.text = "����!";
        successMessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(successMessageDuration);
        successMessageText.gameObject.SetActive(false);
        successMessageCoroutine = null;
        CloseGame();
    }
}