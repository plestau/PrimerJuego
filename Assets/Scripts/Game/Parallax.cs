using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float efectoParallax;
    private Transform camara;
    private Vector3 ultimaPosicionCamara;
    
    // Start is called before the first frame update
    void Start()
    {
        camara = Camera.main.transform;
        ultimaPosicionCamara = camara.position;
    }

    private void LateUpdate()
    {
        Vector3 movimientoFondo = camara.position - ultimaPosicionCamara;
        transform.position += new Vector3(movimientoFondo.x * efectoParallax, movimientoFondo.y * efectoParallax, 0);
        ultimaPosicionCamara = camara.position;
    }
}
