using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{

    public const uint MaxValue = 11; 

    public uint Value { get; private set; }
    public uint Points => IsEmpty ? 0 : (uint)Mathf.Pow(2, Value);

    public bool IsEmpty => Value == 0;

    public bool HasMerged { get; private set; }

    public uint X { get; private set; }
    public uint Y { get; private set; }

    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    private CellAnimation _currentAnimation;

    public void ResetFlags()
    {
        HasMerged = false;
    }

    public void ResetCell(bool updateUI = true)
    {
        Value = 0;

        if (updateUI)
            UpdateCell();
    }

    public void SetCell(uint x, uint y, uint value, bool updateUI = true)
    {
        X = x;
        Y = y;
        Value = value;

        if (updateUI)
            UpdateCell();
    }

    public void IncreaseValue()
    {
        Value++;
        HasMerged = true;

        Root.Instance.AddPoints(Points);
    }

    public void MoveTo(Cell target)
    {
        CellAnimator.Instance.SmoothTransition(this, target, false);

        target.SetCell(target.X, target.Y, Value, false);
        ResetCell();
    }

    public void Merge(Cell other)
    {
        CellAnimator.Instance.SmoothTransition(this, other, true);

        other.IncreaseValue();

        ResetCell();
    }

    public void UpdateCell()
    {
        _text.text = IsEmpty ? string.Empty : Points.ToString();
        _text.color = Value <= 2 ? Vars.Instance.PointsDarkColor : Vars.Instance.PointsLightColor;

        _image.color = Vars.Instance.CellColors[Value];
    }

    public void SetAnimation(CellAnimation animation)
    {
        _currentAnimation = animation;
    }
}
