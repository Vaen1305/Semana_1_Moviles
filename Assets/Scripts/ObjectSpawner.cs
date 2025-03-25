using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] TouchController _touchManager;
    [SerializeField] TrailRenderer _trailPrefab;

    private List<GameObject> _spawnedObjects = new List<GameObject>();
    private TrailRenderer _activeTrail;

    void OnEnable()
    {
        _touchManager.touchEvents.OnTap.AddListener(SpawnObject);
        _touchManager.touchEvents.OnSwipe.AddListener(OnSwipe);
        _touchManager.touchEvents.OnDoubleTap.AddListener(DeleteObject);
    }

    void OnDisable()
    {
        _touchManager.touchEvents.OnTap.RemoveListener(SpawnObject);
        _touchManager.touchEvents.OnSwipe.RemoveListener(OnSwipe);
        _touchManager.touchEvents.OnDoubleTap.RemoveListener(DeleteObject);
    }

    public void SpawnObject(Vector2 screenPos)
    {
        Debug.Log("Intentando crear objeto...");

        GameObject prefab = _touchManager.shapeData.GetShape(_touchManager.CurrentShapeIndex);
        Color color = _touchManager.colorData.GetColor(_touchManager.CurrentColorIndex);

        if (prefab == null)
        {
            Debug.LogError("¡No hay prefab asignado para la forma seleccionada!");

            return;
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;

        GameObject newObj = Instantiate(prefab, worldPos, Quaternion.identity);
        newObj.GetComponent<SpriteRenderer>().color = color;
        _spawnedObjects.Add(newObj);
    }

    public void DeleteObject(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        for (int i = _spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (_spawnedObjects[i] == hit.collider?.gameObject)
            {
                Destroy(_spawnedObjects[i]);
                _spawnedObjects.RemoveAt(i);
                break;
            }
        }
    }

    public void OnSwipe(Vector2 direction)
    {
        ClearAllObjects();
        CreateTrailEffect();
    }

    void ClearAllObjects()
    {
        for (int i = _spawnedObjects.Count - 1; i >= 0; i--)
        {
            Destroy(_spawnedObjects[i]);
        }
        _spawnedObjects.Clear();
    }

    void CreateTrailEffect()
    {
        if (_activeTrail != null) Destroy(_activeTrail.gameObject);

        _activeTrail = Instantiate(_trailPrefab);
        _activeTrail.startColor = _touchManager.colorData.GetColor(_touchManager.CurrentColorIndex);
        _activeTrail.endColor = Color.clear;
        _activeTrail.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}