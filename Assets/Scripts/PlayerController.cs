using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 10f; 
    public int cantidadImpactos = 0; // Variable para almacenar la cantidad de impactos[cite: 1]
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float movimientoHorizontal = 0f;
        float movimientoVertical = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) movimientoVertical = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) movimientoVertical = -1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) movimientoHorizontal = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) movimientoHorizontal = 1f;
        }

        Vector3 movimiento = new Vector3(movimientoHorizontal, 0.0f, movimientoVertical);
        rb.AddForce(movimiento * velocidad);
    }

    // Detección de colisiones[cite: 1]
    void OnCollisionEnter(Collision colision)
    {
        if (colision.gameObject.CompareTag("Muro"))
        {
            cantidadImpactos++; 
            // Generar información mediante consola al colisionar[cite: 1]
            Debug.Log("Impacto registrado. Total: " + cantidadImpactos); 
        }
    }

    // Detectar el ingreso a una zona configurada como Trigger[cite: 1]
    void OnTriggerEnter(Collider otro)
    {
        if (otro.gameObject.CompareTag("Meta"))
        {
            // Modificar propiedades de un Rigidbody mediante programación[cite: 1]
            rb.isKinematic = true; 

            // Mostrar mediante la consola de Unity información relacionada con el resultado obtenido[cite: 1]
            Debug.Log("Nivel terminado - Puntaje: 1000 - Colisiones: " + cantidadImpactos);
        }
    }
}