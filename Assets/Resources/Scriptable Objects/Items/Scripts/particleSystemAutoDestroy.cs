using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleSystemAutoDestroy : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.duration + GetComponent<ParticleSystem>().main.startLifetime.constant);
    }


}
