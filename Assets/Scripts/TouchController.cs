using UnityEngine;
using UnityEngine.UI;

public class TouchController : MonoBehaviour
{
    private SpriteRenderer _comSpriteRenderer;
    public GameObject[] Object;
    private string _TagSelect;

    private void Awake()
    {
        _comSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch currentTouch = Input.GetTouch(0);

            if (currentTouch.phase == TouchPhase.Began)
            {
                CreateObject(currentTouch.position);
            }
        }
    }

    public void SelectTag(string newTag)
    {
        _TagSelect = newTag;
        Debug.Log("Tag seleccionado: " + _TagSelect);
    }

    private Vector2 GetRealPosition(Vector2 value)
    {
        return Camera.main.ScreenToWorldPoint(value);
    }

    private void CreateObject(Vector2 positionTouch)
    {

        Vector2 positionword = GetRealPosition(positionTouch);

        for (int i = 0; i < Object.Length; i++)
        {
            if (Object[i] != null && Object[i].CompareTag(_TagSelect))
            {
                Instantiate(Object[i], positionword, Quaternion.identity);
                return;
            }
        }

        Debug.LogWarning("No hay prefab con el tag: " + _TagSelect);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Color"))
        {
            _comSpriteRenderer.color = collision.GetComponent<SpriteRenderer>().color;
        }
        else if (collision.CompareTag("Shape"))
        {
            _comSpriteRenderer.sprite = collision.GetComponent<SpriteRenderer>().sprite;
            transform.localScale = collision.transform.localScale;
        }
    }
}