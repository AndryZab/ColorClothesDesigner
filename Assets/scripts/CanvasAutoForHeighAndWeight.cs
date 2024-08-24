using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerAdjuster : MonoBehaviour
{
    private static readonly Vector2[] longScreenResolutions = new Vector2[]
    {
        new Vector2(1125, 2436),
        new Vector2(1170, 2532),
        new Vector2(1284, 2778),
        new Vector2(1179, 2556),
        new Vector2(1284, 2778)
    };

    private static readonly Vector2[] wideScreenResolutions = new Vector2[]
    {
        new Vector2(828, 1792),
        new Vector2(750, 1334),
        new Vector2(1284, 2778)
    };

    private static readonly Vector2[] tabletResolutions = new Vector2[]
    {
        new Vector2(1620, 2160),
        new Vector2(1488, 2266),
        new Vector2(1640, 2360),
        new Vector2(1668, 2388),
        new Vector2(2048, 2732)
    };

    public float matchForLongScreens = 0.5f;
    public float matchForWideScreens = 0.43f;
    public float matchForTablets = 0.3f;

    public CanvasScaler[] canvasScalers;

    void Start()
    {
        AdjustCanvasScalers();
    }

    private void AdjustCanvasScalers()
    {
        if (canvasScalers == null || canvasScalers.Length == 0)
        {
            return;
        }

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float aspectRatio = screenWidth / screenHeight;

        float bestMatch = 0.5f;
        bool isLongScreen = false;
        bool isWideScreen = false;
        bool isTablet = false;

        foreach (Vector2 resolution in longScreenResolutions)
        {
            float supportedAspectRatio = resolution.x / resolution.y;
            float aspectDifference = Mathf.Abs(aspectRatio - supportedAspectRatio);

            if (aspectDifference < 0.1f)
            {
                isLongScreen = true;
                bestMatch = matchForLongScreens;
                break;
            }
        }

        if (!isLongScreen)
        {
            foreach (Vector2 resolution in wideScreenResolutions)
            {
                float supportedAspectRatio = resolution.x / resolution.y;
                float aspectDifference = Mathf.Abs(aspectRatio - supportedAspectRatio);

                if (aspectDifference < 0.1f)
                {
                    isWideScreen = true;
                    bestMatch = matchForWideScreens;
                    break;
                }
            }
        }

        if (!isLongScreen && !isWideScreen)
        {
            foreach (Vector2 resolution in tabletResolutions)
            {
                float supportedAspectRatio = resolution.x / resolution.y;
                float aspectDifference = Mathf.Abs(aspectRatio - supportedAspectRatio);

                if (aspectDifference < 0.1f)
                {
                    isTablet = true;
                    bestMatch = matchForTablets;
                    break;
                }
            }
        }

        foreach (CanvasScaler scaler in canvasScalers)
        {
            if (scaler != null)
            {
                scaler.matchWidthOrHeight = bestMatch;
            }
        }
    }
}
