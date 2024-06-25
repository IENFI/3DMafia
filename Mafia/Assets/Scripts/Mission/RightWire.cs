using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightWire : MonoBehaviour
{
    public EWireColor WireColor { get; private set; }

    [SerializeField]
    private List<Image> mWireImages;

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
}
