using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Shape Data")]
public class ShapeData : ScriptableObject
{
    [SerializeField] GameObject[] _shapes = new GameObject[3];

    public GameObject GetShape(int index)
    {
        if (index < 0 || index >= _shapes.Length || _shapes[index] == null)
        {
            Debug.LogError("Índice de forma inválido o prefab no asignado");
            return null;
        }
        return _shapes[index];
    }
}