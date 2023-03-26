using UnityEngine;

public class Vars : MonoBehaviour
{
    public static Vars Instance;

    public Color[] CellColors;

    [Space(5)]
    public Color PointsDarkColor;
    public Color PointsLightColor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
