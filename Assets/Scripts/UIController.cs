using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Button[] _shapeButtons;
    [SerializeField] Button[] _colorButtons;

    [Header("Dependencies")]
    [SerializeField] TouchController _touchController;
    [SerializeField] ShapeData _shapeData;
    [SerializeField] ColorData _colorData;

    void Start()
    {
        InitializeShapeButtons();
        InitializeColorButtons();
    }

    void InitializeShapeButtons()
    {
        for (int i = 0; i < _shapeButtons.Length; i++)
        {
            int index = i;
            Sprite sprite = _shapeData.GetShape(i).GetComponent<SpriteRenderer>().sprite;
            _shapeButtons[i].image.sprite = sprite;
            _shapeButtons[i].onClick.AddListener(() => _touchController.SetSelectedShape(index));
        }
    }

    void InitializeColorButtons()
    {
        for (int i = 0; i < _colorButtons.Length; i++)
        {
            int index = i;
            _colorButtons[i].image.color = _colorData.GetColor(i);
            _colorButtons[i].onClick.AddListener(() => _touchController.SetSelectedColor(index));
        }
    }
}