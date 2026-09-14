using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Variables públicas para asignarlas en el Inspector
    public Transform jugador; 
    public Vector3 compensacion = new Vector3(0, 5, -7); 

    void LateUpdate()
    {
        if (jugador != null)
        {
            // Modificación de la posición para seguir al objeto
            transform.position = jugador.position + compensacion;
            // Hace que la cámara siempre apunte hacia el jugador
            transform.LookAt(jugador);
        }
    }
}