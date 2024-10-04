using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Sudoku : MinigameBase
{
    // 4x4 스도쿠 정답 배열
    // private int[,] correctAnswers = new int[4, 4]
    // {
    //     { 1, 3, 2, 4 },
    //     { 4, 2, 1, 3 },
    //     { 2, 4, 3, 1 },
    //     { 3, 1, 4, 2 }
    // };
    [SerializeField] private List<Button> keyButton;  // 키패드 버튼
    [SerializeField] private Button AnswerButton;
    private Button selectedButton;  // 클릭된 Sudoku 버튼을 저장할 변수
    [SerializeField]
    private bool active = false;
    private int index = 1;
    [SerializeField]
    private GameObject minigameManager;

    public override void ReceiveToken()
    {
        Debug.Log("Sudoku 미니게임이 시작되었습니다.");
        active = true;
    }

    public override void  Deactivation() {
        active = false;
    }

    public override bool GetActive(){
        return active;
    }
    public override MinigameManager GetMinigameManager(){
        return minigameManager.GetComponent<MinigameManager>();
    }

    private int[,] correctAnswers = new int[4, 4]
    {
        { 1, 2, 3, 4 },
        { 4, 3, 2, 1 },
        { 2, 1, 4, 3 },
        { 3, 4, 1, 2 }
    };

    // 버튼 배열
    [SerializeField]
    private List<Button> buttonList;
    private Button[,] buttons; // 2차원 배열


    // 플레이어가 입력한 현재 상태 배열
    private int[,] playerAnswers = new int[4, 4];
    [SerializeField]
    private GameObject answerText;
    private int uncorrectNumber = 0;

    // 게임 시작 시 빈칸으로 만들 버튼의 위치를 저장하는 리스트
    private List<Vector2Int> emptyCells = new List<Vector2Int>();

    [SerializeField]
    private MinigameInteraction minigame;


    // List<Button>을 Button[4, 4]로 변환하는 메서드
    private Button[,] ConvertListTo2DArray(List<Button> buttonList, int rows, int cols)
    {
        if (buttonList.Count != rows * cols)
        {
            Debug.LogError("버튼 리스트의 크기와 2차원 배열의 크기가 맞지 않습니다.");
            return null;
        }

        Button[,] grid = new Button[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j] = buttonList[i * cols + j];
            }
        }

        return grid;
    }
    void Start()
    {
        buttons = ConvertListTo2DArray(buttonList, 4, 4);
        // 버튼 초기화 및 랜덤 빈칸 만들기
        InitializeButtons();
        HideRandomCells();
        SetKeypadButtonsInteractable(false);
        AnswerButton.interactable = false;
    }

    // 버튼을 초기화하고 각 버튼에 숫자를 설정
    void InitializeButtons()
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                // 각 버튼에 맞는 숫자를 설정 (Button component의 Text를 통해)
                buttons[row, col].GetComponentInChildren<TextMeshProUGUI>().text = correctAnswers[row, col].ToString();
                playerAnswers[row, col] = correctAnswers[row, col];

                // 버튼 클릭 이벤트 추가
                int r = row;
                int c = col;
                buttons[r, c].onClick.AddListener(() => OnSudokuButtonClick(r, c));
            }
        }
    }

    // 랜덤으로 4개의 버튼 텍스트를 빈칸으로 설정
    void HideRandomCells()
    {
        emptyCells.Clear();
        int hiddenCount = 0;
        while (hiddenCount < 4)
        {
            int randomRow = Random.Range(0, 4);
            int randomCol = Random.Range(0, 4);

            if (!emptyCells.Contains(new Vector2Int(randomRow, randomCol)))
            {
                buttons[randomRow, randomCol].GetComponentInChildren<TextMeshProUGUI>().text = "";
                playerAnswers[randomRow, randomCol] = 0; // 빈칸은 0으로 표시

                // 빈칸인 경우 테두리 강조
                Outline outline = buttons[randomRow, randomCol].GetComponent<Outline>();
                if (outline != null)
                {
                    outline.effectColor = Color.black; // 테두리 색상 설정 (원하는 색상으로 설정 가능)
                    outline.effectDistance = new Vector2(1, 1); // 테두리 두께 설정
                    outline.enabled = true; // 테두리 활성화
                }

                emptyCells.Add(new Vector2Int(randomRow, randomCol));
                
                hiddenCount++;
            }
        }
    }

        // 키패드의 모든 버튼을 비활성화하는 방법
    public void SetKeypadButtonsInteractable(bool state)
    {
        foreach (Button button in keyButton)
        {
            button.interactable = state;
        }
    }

    // 버튼 클릭 이벤트 처리
    void OnSudokuButtonClick(int row, int col)
    {
        // emptyCells 리스트를 통해 해당 좌표가 빈 칸인지 확인
        if (emptyCells.Contains(new Vector2Int(row, col)))  // 빈 칸이면
        {
            selectedButton = buttons[row, col];  // 현재 선택된 버튼 저장
            SetKeypadButtonsInteractable(true);  // 키패드 버튼 활성화
        }
        else
        {
            Debug.Log("빈 칸이 아닙니다.");  // 빈 칸이 아닌 경우 처리
            SetKeypadButtonsInteractable(false);  // 키패드 버튼 비활성화
        }
    }

    // 키패드 버튼 클릭 처리 (숫자 1 ~ 4)
    public void OnKeypadButtonClick(int number)
    {
        answerText.SetActive(false);
        if (selectedButton != null)
        {
            // 선택한 버튼에 숫자 입력
            selectedButton.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
            playerAnswers[GetButtonRow(selectedButton), GetButtonCol(selectedButton)] = number;

            // 키패드 숨기기
            SetKeypadButtonsInteractable(false); 

            // 빈칸이 다 채워졌는지 확인하고 정답 버튼 활성화 여부 결정
            CheckIfAllCellsFilled();

            if (CheckWinCondition())
            {
                Debug.Log("게임 성공!");
            }
        }
    }

    // Sudoku 버튼의 행을 가져오는 헬퍼 함수
    int GetButtonRow(Button button)
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (buttons[row, col] == button)
                    return row;
            }
        }
        return -1;
    }

    // Sudoku 버튼의 열을 가져오는 헬퍼 함수
    int GetButtonCol(Button button)
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (buttons[row, col] == button)
                    return col;
            }
        }
        return -1;
    }

    bool CheckWinCondition()
    {
        uncorrectNumber= 0 ;
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (playerAnswers[row, col] != correctAnswers[row, col])
                {
                    uncorrectNumber ++;
                }
            }
        }
        if (uncorrectNumber > 0)
            return false;
        else 
            return true;
    }

    void OnClickAnswerButton(){
        if (CheckWinCondition())
        {
            // Debug.Log("게임 성공!");
            minigame.ExitCode = true;
            active = false;
            GetMinigameManager().SuccessMission(index);
            gameObject.SetActive(false);
            HideRandomCells();
        }
        else {
            answerText.SetActive(true);
            answerText.GetComponentInChildren<TextMeshProUGUI>().text = "틀린 답이 "+uncorrectNumber.ToString()+"개 입니다.";
        }
    }

    void CheckIfAllCellsFilled()
    {
        foreach (var cell in emptyCells)
        {
            int row = cell.x;
            int col = cell.y;
            // 빈칸이 채워져 있지 않다면, 버튼 비활성화
            if (string.IsNullOrEmpty(buttons[row, col].GetComponentInChildren<TextMeshProUGUI>().text))
            {
                AnswerButton.interactable = false;
                return;
            }
        }
        // 모든 빈칸이 채워졌다면 버튼 활성화
        AnswerButton.interactable = true;
    }

}
