using UnityEngine;

public class ThirdPersonFollow : MonoBehaviour
{
    public Transform jugador;
    public Vector3 offset = new Vector3(0, 5, -6);
    public float suavizado = 5f;

    private Vector3 ultimaDireccion = Vector3.forward;

    void LateUpdate()
    {
        Rigidbody rbJugador = jugador.GetComponent<Rigidbody>();

        // Si se está moviendo, actualiza la dirección; si no, mantiene la última
        if (rbJugador.linearVelocity.sqrMagnitude > 0.1f)
        {
            Vector3 direccionPlana = rbJugador.linearVelocity;
            direccionPlana.y = 0; // ignora movimiento vertical (salto/caída)
            if (direccionPlana.sqrMagnitude > 0.01f)
                ultimaDireccion = direccionPlana.normalized;
        }

        Quaternion rotacionDireccion = Quaternion.LookRotation(ultimaDireccion);
        Vector3 posicionDeseada = jugador.position + rotacionDireccion * offset;

        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
        transform.LookAt(jugador.position + Vector3.up * 1f);
    }
}