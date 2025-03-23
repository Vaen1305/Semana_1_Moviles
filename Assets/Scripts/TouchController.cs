using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TouchControl : MonoBehaviour
{
    [Header("Opciones Principales")]
    [SerializeField] Color[] coloresDisponibles = new Color[3];
    [SerializeField] GameObject[] prefabsFormas = new GameObject[3];
    [SerializeField] float sensibilidadSwipe = 150f;
    [SerializeField] float tiempoMaxSwipe = 0.5f;
    [SerializeField] TrailRenderer trailPrefab;

    [Header("Botones Interfaz")]
    [SerializeField] Button[] botonesColor = new Button[3];
    [SerializeField] Button[] botonesForma = new Button[3];

    private int colorSeleccionado = 0;
    private int formaSeleccionada = 0;
    private float ultimoToque;
    private Vector2 posicionInicialSwipe;
    private float tiempoInicialSwipe;
    private GameObject objetoArrastrado;
    private List<GameObject> objetosCreados = new List<GameObject>();
    private TrailRenderer trailActual;

    void Start()
    {
        ConfigurarInterfaz();
        if (trailPrefab != null) trailPrefab.gameObject.SetActive(false);
        
        if(prefabsFormas.Length != 3 || prefabsFormas[0] == null || prefabsFormas[1] == null || prefabsFormas[2] == null){
            Debug.LogError("Asigna los 3 prefabs de formas en el Inspector!");
        }
    }

    void ConfigurarInterfaz()
    {
        for(int i = 0; i < 3; i++)
        {
            botonesColor[i].GetComponent<Image>().color = coloresDisponibles[i];
            int indiceColor = i;
            botonesColor[i].onClick.AddListener(() => SeleccionarColor(indiceColor));

            Sprite spriteForma = prefabsFormas[i].GetComponent<SpriteRenderer>().sprite;
            botonesForma[i].GetComponent<Image>().sprite = spriteForma;
            int indiceForma = i;
            botonesForma[i].onClick.AddListener(() => SeleccionarForma(indiceForma));
        }
    }

    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch toque = Input.GetTouch(0);
            
            if(!EstaTocandoUI(toque))
            {
                ManejarToque(toque);
            }
        }
    }

    void ManejarToque(Touch toque)
    {
        switch(toque.phase)
        {
            case TouchPhase.Began:
                IniciarToque(toque);
                break;

            case TouchPhase.Moved:
                MoverToque(toque);
                break;

            case TouchPhase.Ended:
                FinalizarToque(toque);
                break;
        }
    }

    void IniciarToque(Touch toque)
    {
        posicionInicialSwipe = toque.position;
        tiempoInicialSwipe = Time.time;

        Collider2D colision = Physics2D.OverlapPoint(ConvertirPosicion(toque.position));
        if(colision != null && colision.CompareTag("Movible"))
        {
            StartCoroutine(DetectarArrastre(toque));
        }
        else
        {
            IniciarTrail(toque.position);
        }
    }

    System.Collections.IEnumerator DetectarArrastre(Touch toque)
    {
        float tiempoPresionado = 0f;
        Vector2 posicionInicial = toque.position;

        while(tiempoPresionado < 0.2f && toque.phase == TouchPhase.Stationary)
        {
            tiempoPresionado += Time.deltaTime;
            yield return null;
        }

        if(tiempoPresionado >= 0.2f)
        {
            PrepararArrastre(toque.position);
        }
    }

    void PrepararArrastre(Vector2 posicionToque)
    {
        Collider2D colision = Physics2D.OverlapPoint(ConvertirPosicion(posicionToque));
        if(colision != null)
        {
            objetoArrastrado = colision.gameObject;
        }
    }

    void MoverToque(Touch toque)
    {
        if(objetoArrastrado != null)
        {
            ArrastrarObjeto(toque.position);
        }
        ActualizarTrail(toque.position);
    }

    void ArrastrarObjeto(Vector2 posicionToque)
    {
        Vector3 nuevaPosicion = ConvertirPosicion(posicionToque);
        nuevaPosicion.z = objetoArrastrado.transform.position.z;
        objetoArrastrado.transform.position = nuevaPosicion;
    }

    void FinalizarToque(Touch toque)
    {
        if(objetoArrastrado != null)
        {
            objetoArrastrado = null;
        }
        else
        {
            VerificarAcciones(toque);
        }
        DetenerTrail();
    }

    void VerificarAcciones(Touch toque)
    {
        if(EsSwipe(toque))
        {
            LimpiarPantalla();
        }
        else
        {
            ManejarToquesRapidos(toque);
        }
    }

    bool EsSwipe(Touch toque)
    {
        Vector2 diferencia = toque.position - posicionInicialSwipe;
        float duracion = Time.time - tiempoInicialSwipe;
        return diferencia.magnitude > sensibilidadSwipe && duracion < tiempoMaxSwipe;
    }

    void ManejarToquesRapidos(Touch toque)
    {
        float tiempoDesdeUltimoToque = Time.time - ultimoToque;
        
        if(tiempoDesdeUltimoToque < 0.4f)
        {
            EliminarObjeto(toque.position);
            ultimoToque = 0;
        }
        else
        {
            CrearObjeto(toque.position);
            ultimoToque = Time.time;
        }
    }

    void IniciarTrail(Vector2 posicion)
    {
        if(trailPrefab != null)
        {
            trailActual = Instantiate(trailPrefab, ConvertirPosicion(posicion), Quaternion.identity);
            trailActual.startColor = coloresDisponibles[colorSeleccionado];
            trailActual.endColor = coloresDisponibles[colorSeleccionado];
            trailActual.gameObject.SetActive(true);
        }
    }

    void ActualizarTrail(Vector2 posicion)
    {
        if(trailActual != null)
        {
            trailActual.transform.position = ConvertirPosicion(posicion);
        }
    }

    void DetenerTrail()
    {
        if(trailActual != null)
        {
            Destroy(trailActual.gameObject, trailActual.time);
            trailActual = null;
        }
    }

    void CrearObjeto(Vector2 posicionPantalla)
    {
        if(prefabsFormas[formaSeleccionada] == null) return;

        Vector3 posicionMundo = ConvertirPosicion(posicionPantalla);
        GameObject nuevoObjeto = Instantiate(prefabsFormas[formaSeleccionada], posicionMundo, Quaternion.identity);
        ConfigurarNuevoObjeto(nuevoObjeto);
        objetosCreados.Add(nuevoObjeto);
    }

    void ConfigurarNuevoObjeto(GameObject obj)
    {
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        if(renderer != null) renderer.color = coloresDisponibles[colorSeleccionado];
        
        if(obj.GetComponent<Collider2D>() == null)
        {
            obj.AddComponent<BoxCollider2D>();
        }
        obj.tag = "Movible";
    }

    void EliminarObjeto(Vector2 posicionPantalla)
    {
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(ConvertirPosicion(posicionPantalla), 0.5f);
        
        for(int i = 0; i < colisiones.Length; ++i)
        {
            if(colisiones[i].CompareTag("Movible"))
            {
                objetosCreados.Remove(colisiones[i].gameObject);
                Destroy(colisiones[i].gameObject);
                break;
            }
        }
    }

    void LimpiarPantalla()
    {
        for(int i = objetosCreados.Count - 1; i >= 0; --i)
        {
            if(objetosCreados[i] != null)
            {
                Destroy(objetosCreados[i]);
            }
        }
        objetosCreados.Clear();
    }

    Vector3 ConvertirPosicion(Vector2 posicionPantalla)
    {
        Vector3 posicionMundo = Camera.main.ScreenToWorldPoint(posicionPantalla);
        posicionMundo.z = 0;
        return posicionMundo;
    }

    bool EstaTocandoUI(Touch toque)
    {
        return EventSystem.current.IsPointerOverGameObject(toque.fingerId);
    }

    public void SeleccionarColor(int indice) => colorSeleccionado = Mathf.Clamp(indice, 0, 2);
    public void SeleccionarForma(int indice) => formaSeleccionada = Mathf.Clamp(indice, 0, 2);
}