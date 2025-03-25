using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Color Data")]
public class ColorData : ScriptableObject
{
    [SerializeField] Color[] _colors = new Color[3];

    public Color GetColor(int index)
    {
        return _colors[Mathf.Clamp(index, 0, _colors.Length - 1)];
    }
}