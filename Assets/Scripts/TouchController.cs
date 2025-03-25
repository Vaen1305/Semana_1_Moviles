using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour
{
    [System.Serializable]
    public class TouchEvents
    {
        public UnityEvent<Vector2> OnTap = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> OnSwipe = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> OnDoubleTap = new UnityEvent<Vector2>();
    }

    [Header("Configuración")]
    [SerializeField] float _swipeThreshold = 150f;
    [SerializeField] float _doubleTapTime = 0.3f;

    [Header("Datos")]
    [SerializeField] public ShapeData shapeData;
    [SerializeField] public ColorData colorData;

    [Header("Eventos")]
    public TouchEvents touchEvents = new TouchEvents();

    public int CurrentShapeIndex { get; private set; }
    public int CurrentColorIndex { get; private set; }

    private Vector2 _touchStartPos;
    private float _lastTapTime;
    private GameObject _draggedObject;

    void Update()
    {
        if (Input.touchCount > 0 && !IsOverUI(Input.GetTouch(0)))
        {
            ProcessTouch(Input.GetTouch(0));
        }
    }

    void ProcessTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                HandleTouchStart(touch);
                break;

            case TouchPhase.Moved:
                HandleTouchMove(touch);
                break;

            case TouchPhase.Ended:
                HandleTouchEnd(touch);
                break;
        }
    }

    void HandleTouchStart(Touch touch)
    {
        Debug.Log("Touch detectado en posición: " + touch.position);

        _touchStartPos = touch.position;

        if (Time.time - _lastTapTime < _doubleTapTime)
        {
            touchEvents.OnDoubleTap.Invoke(touch.position);
            _lastTapTime = 0;
        }
        else
        {
            _lastTapTime = Time.time;
        }

        DetectDraggableObject(touch.position);
    }

    void HandleTouchMove(Touch touch)
    {
        Debug.Log("Moviendo touch: " + touch.deltaPosition);

        if (_draggedObject != null)
        {
            _draggedObject.transform.position = GetWorldPos(touch.position);
        }

        if (Vector2.Distance(touch.position, _touchStartPos) > _swipeThreshold)
        {
            touchEvents.OnSwipe.Invoke(touch.position - _touchStartPos);
        }
    }

    void HandleTouchEnd(Touch touch)
    {
        if (_draggedObject != null)
        {
            _draggedObject = null;
        }
    }

    void DetectDraggableObject(Vector2 screenPos)
    {
        Vector3 worldPos = GetWorldPos(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Draggable"))
        {
            _draggedObject = hit.collider.gameObject;
        }
    }

    Vector3 GetWorldPos(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }

    bool IsOverUI(Touch touch)
    {
        return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }

    public void SetSelectedShape(int index)
    {
        CurrentShapeIndex = Mathf.Clamp(index, 0, 2);
    }

    public void SetSelectedColor(int index)
    {
        CurrentColorIndex = Mathf.Clamp(index, 0, 2);
    }
}