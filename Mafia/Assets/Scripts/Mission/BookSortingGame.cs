using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BookSortingGame : MinigameBase
{
    [SerializeField] private GameObject[] bookObjects; // Pre-placed book objects
    [SerializeField] private float successMessageDuration = 0.5f;
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

    // Initialization and game opening/closing functions

    // Store original positions of books and add Book components
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

    // UI Control (Open/Close)
    public void ToggleBookSortingUI()
    {
        if (BookMinigameUI.activeSelf)
        {
            CloseGame();
        }
    }

    // Initialize game: reset books, set active book count, activate books
    private void InitializeGame()
    {
        ResetBooks();
        bookCount = Random.Range(5, 9);
        ActivateBooks();
        CheckInitialPlacements();
    }

    // Reset all books to initial state
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

    // Randomly activate and initialize books
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

    // Check if any books are already in correct positions initially
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

    // Close game and cleanup
    // Close game and cleanup
    private void CloseGame()
    {
        // First, stop and clear any running coroutine
        if (successMessageCoroutine != null)
        {
            StopCoroutine(successMessageCoroutine);
            successMessageCoroutine = null;
        }

        // Immediately disable and clear the success message
        if (successMessageText != null && successMessageText.gameObject != null)
        {
            successMessageText.gameObject.SetActive(false);
            successMessageText.text = "";
        }

        // Then proceed with game cleanup
        ResetBooks();
        minigame.ExitCode = true;
        active = false;
        GetMinigameManager().SuccessMission(index);

        // Finally disable the game object
        this.gameObject.SetActive(false);
    }

    // Main gameplay functions

    // Check input every frame
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

    // Attempt to pick up a book
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

    // Handle book dragging
    private void DragBook()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, null, out localPoint);
        currentBook.GetComponent<RectTransform>().anchoredPosition = localPoint + dragOffset;
    }

    // Attempt to drop a book
    private void TryDropBook()
    {
        if (currentBook != null)
        {
            int nearestIndex = FindNearestBookSlot(currentBook.GetComponent<RectTransform>().anchoredPosition);

            if (nearestIndex != -1)
            {
                if (nearestIndex == currentBook.Number - 1)  // If book is in correct position
                {
                    PlaceBookCorrectly(currentBook, nearestIndex, dragStartIndex);
                }
                else  // If position is incorrect
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

    // Place book in correct position
    private void PlaceBookCorrectly(Book book, int correctIndex, int startIndex)
    {
        Book bookAtCorrectIndex = bookObjects[correctIndex]?.GetComponent<Book>();

        // Move current book to correct position
        book.GetComponent<RectTransform>().anchoredPosition = originalPositions[correctIndex];
        book.UpdateInitialPosition(originalPositions[correctIndex]);
        book.IsPlaced = true;

        // Update bookObjects array
        bookObjects[startIndex] = null;
        bookObjects[correctIndex] = book.gameObject;
        book.transform.SetSiblingIndex(correctIndex);

        // Move existing book if present
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

    // Find nearest book slot position
    private int FindNearestBookSlot(Vector2 position)
    {
        return originalPositions
            .Select((pos, index) => new { Index = index, Distance = Vector2.Distance(pos, position) })
            .OrderBy(x => x.Distance)
            .First().Index;
    }

    // Check if all books are in correct positions
    private void CheckForCompletion()
    {
        if (activeBooks.TrueForAll(book => book.IsPlaced))
        {
            CloseGame();
        }
    }

    // Display success message
    private IEnumerator ShowSuccessMessage()
    {
        // Clear any previous state
        if (successMessageText != null && successMessageText.gameObject != null)
        {
            successMessageText.text = "Success!";
            successMessageText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(successMessageDuration);

        // Check if the game is still active before proceeding
        if (this.gameObject.activeSelf)
        {
            CloseGame();
        }
        else
        {
            // If game is already closing/closed, ensure message is cleaned up
            if (successMessageText != null && successMessageText.gameObject != null)
            {
                successMessageText.gameObject.SetActive(false);
                successMessageText.text = "";
            }
        }
    }
}