using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviourPun
{

    public PhotonView pv;

    public float moveSpeed = 10.0f;

    void Start()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }
        else {
            SmoothMovement();
        }
    }

    private void SmoothMovement()
    {
        throw new NotImplementedException();
    }

    private void ProcessInputs()
    {
    }
}
