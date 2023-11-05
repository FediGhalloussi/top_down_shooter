using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineCamera : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    void FixedUpdate()
    {
        //todo fix that horrible thing
        if (virtualCamera.Follow == null)
        {
            var transform1 = GameManager.Instance.PlayerAvatar.transform;
            virtualCamera.Follow = transform1;
            virtualCamera.LookAt = transform1;
        }
    }
}
