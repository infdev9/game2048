using DG.Tweening;
using UnityEngine;

public class CellAnimator : MonoBehaviour
{
    public static CellAnimator Instance { get; private set; }

    [SerializeField] private CellAnimation _animationPrefab;

    private void Awake()
    {
        if (Instance is null) 
            Instance = this;

        DOTween.Init();
    }

    public void SmoothTransition(Cell from, Cell to, bool isMerging)
    {
        Instantiate(_animationPrefab, transform, false).Move(from, to, isMerging);
    }

    public void SmoothAppear(Cell cell)
    {
        Instantiate(_animationPrefab, transform, false).Appear(cell);
    }
}
