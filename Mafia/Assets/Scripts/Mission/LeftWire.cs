using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftWire : MonoBehaviour
{
    public EWireColor WireColor { get; private set; }

    public bool IsConnected { get; private set; }

    [SerializeField]
    private List<Image> mWireImages;

    [SerializeField]
    private Image mLightImage;

    [SerializeField]
    private RightWire mConnectedWire;

    [SerializeField]
    private RectTransform mWireBody;

    //private LeftWire mSelectedWire;

    [SerializeField]
    private float offset = 15f;

    private Canvas mGameCanvas;

    // Start is called before the first frame update
    void Start()
    {
        mGameCanvas = FindObjectOfType<Canvas>();
    }

    public void SetTarget(Vector3 targetPosition, float offset)
    {
        // 현재 오브젝트의 위치를 RectTransform 기준으로 변환
        Vector3 localTarget = mGameCanvas.transform.InverseTransformPoint(targetPosition);
        Vector3 localStart = mGameCanvas.transform.InverseTransformPoint(transform.position);

        // 각도 계산
        float angle = Vector2.SignedAngle(Vector2.right, localTarget - localStart);

        // 거리 계산 (캔버스 스케일을 고려)
        float distance = Vector2.Distance(localStart, localTarget) - offset;
        distance = Mathf.Max(0, distance); // 음수가 되지 않도록 보정

        // 전선의 각도 및 크기 설정
        mWireBody.localRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        mWireBody.sizeDelta = new Vector2(distance, mWireBody.sizeDelta.y);
    }

    public void ResetTarget()
    {
        mWireBody.localRotation = Quaternion.Euler(Vector3.zero);
        mWireBody.sizeDelta = new Vector2(0f, mWireBody.sizeDelta.y);
    }

    public void SetWireColor(EWireColor wireColor)
    {
        WireColor = wireColor;
        Color color = Color.black;
        switch (WireColor)
        {
            case EWireColor.Red:
                color = Color.red;
                break;

            case EWireColor.Blue:
                color = Color.blue;
                break;

            case EWireColor.Yellow:
                color = Color.yellow;
                break;

            case EWireColor.Magenta:
                color = Color.magenta;
                break;
        }

        foreach (var image in mWireImages)
        {
            image.color = color;
        }
    }

    public void ConnectWire(RightWire rightWire)
    {
        if (mConnectedWire != null && mConnectedWire != rightWire)
        {
            mConnectedWire.DisconnectWire(this);
            mConnectedWire = null;
        }

        mConnectedWire = rightWire;
        if (mConnectedWire.WireColor == WireColor)
        {
            mLightImage.color = Color.yellow;
            IsConnected = true;
        }
    }

    public void DisconnectedWire()
    {
        if (mConnectedWire != null)
        {
            mConnectedWire.DisconnectWire(this);
            mConnectedWire = null;
        }
        mLightImage.color = Color.gray;
        IsConnected = false;
    }
}