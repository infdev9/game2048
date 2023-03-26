using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellAnimation : MonoBehaviour
{

    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    private float _moveTime = .1f;
    private float _appearTime = .1f;

    private Sequence _sequence;

    public void Move(Cell from, Cell to, bool isMerging)
    {
        to.SetAnimation(this);

        _image.color = Vars.Instance.CellColors[from.Value];
        _text.text = from.Points.ToString();
        _text.color = from.Value <= 2 ? Vars.Instance.PointsDarkColor : Vars.Instance.PointsLightColor;

        transform.position = from.transform.position;

        _sequence = DOTween.Sequence();

        _sequence.Append(transform.DOMove(to.transform.position, _moveTime).SetEase(Ease.InOutQuad));

        if (isMerging)
        {
            _sequence.AppendCallback(() =>
            {
                _image.color = Vars.Instance.CellColors[to.Value];
                _text.text = to.Points.ToString();
                _text.color = to.Value <= 2 ? Vars.Instance.PointsDarkColor : Vars.Instance.PointsLightColor;
            });

            _sequence.Append(transform.DOScale(1.2f, _appearTime));
            _sequence.Append(transform.DOScale(1, _appearTime));
        }

        _sequence.AppendCallback(() =>
        {
            to.UpdateCell();
            DestroySelf();
        });
    }

    public void Appear(Cell cell) 
    {
        cell.SetAnimation(this);

        _image.color = Vars.Instance.CellColors[cell.Value];
        _text.text = cell.Points.ToString();
        _text.color = cell.Value <= 2 ? Vars.Instance.PointsDarkColor : Vars.Instance.PointsLightColor;

        transform.position = cell.transform.position;
        transform.localScale = Vector2.zero;

        _sequence = DOTween.Sequence();

        _sequence.Append(transform.DOScale(1.2f, _appearTime * 2));
        _sequence.Append(transform.DOScale(1f, _appearTime * 2));
        _sequence.AppendCallback(() =>
        {
            cell.UpdateCell();
            DestroySelf();
        });
    }

    public void DestroySelf()
    {
        _sequence.Kill();
        Destroy(gameObject);
    }
}
