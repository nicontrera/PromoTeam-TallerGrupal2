using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private enemigo1 _inventarioEnemigoRata;//tomo el script enemigo1 y se lo asigno a la variable _enemigo
    [SerializeField] private Escudos _inventarioEscudo;
    [SerializeField] private Espadas _inventarioEspada;

    private int _puntos;
    private string _nivel;
    private int _escudoCantidad;
    private int _espadaCantidad;
    private int _ataquesRata;
    private int _lobo;
    private int _serpiente;
    public TextMeshProUGUI textoNivel;
    public TextMeshProUGUI textoInventario;
    public TextMeshProUGUI textoEnemigos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _puntos = 1;
        _nivel = "Caballero Noble";
        _escudoCantidad = 0;
        _espadaCantidad = 0;
        _ataquesRata = 0;
        _lobo = 0;
        _serpiente = 0;
        
    }

    // Update is called once per frame
    private void Update()
    {
        _escudoCantidad = _inventarioEscudo.coleccion;
        _espadaCantidad = _inventarioEspada.coleccion;
        _ataquesRata = _inventarioEnemigoRata.ataquePositivo;

        _puntos = _escudoCantidad * 3 + _espadaCantidad * 3 - _ataquesRata;

        textoNivel.text = $"Puntos : {_puntos.ToString()}\nNivel: {_nivel}";
        ;
        textoInventario.text = $"Escudo : { _escudoCantidad}\nEspadas: {_espadaCantidad}";
        textoEnemigos.text = $"Ratas: {_ataquesRata}\nLobos: {_lobo}\nSerpientes: {_serpiente}";
    }
}
