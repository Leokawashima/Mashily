using UnityEngine;
using Cinemachine;

public class CamChangeTriiger : MonoBehaviour
{
    [Header("変更前カメラ")]
    [SerializeField] CinemachineVirtualCamera pre_Vcam;
    [Header("変更後カメラ")]
    [SerializeField] CinemachineVirtualCamera post_Vcam;

    bool already = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!already)
        {
            int pre_Priority = pre_Vcam.Priority;
            pre_Vcam.Priority = post_Vcam.Priority;
            post_Vcam.Priority = pre_Priority;
            already = true;
        }
    }
}
