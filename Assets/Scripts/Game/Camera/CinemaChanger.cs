using UnityEngine;
using Cinemachine;

public class CinemaChanger: MonoBehaviour
{
    //使用したいカメラを列挙型配列を作るので自分でこのスクリプトに割り当ててね
    //要素数→要素数の順で変更したいカメラが今使ってるカメラなら変えられないよ
    [SerializeField] CinemachineVirtualCamera[] _vCam;
    public CinemachineVirtualCamera[] VCam { get { return _vCam; } }

    CinemachineVirtualCamera nowCam;

    void Start()
    {
            nowCam = _vCam[0];
    }

    public void ChangeVCamera(CinemachineVirtualCamera After_cam)
    {
        //要は　カメラ１→カメラ２　ただしカメラ２が今使ってるカメラが指定されていたら変更しない処理
        if(After_cam.Priority == 20)
            return;
        int priority = nowCam.Priority;
        nowCam.Priority = After_cam.Priority;
        After_cam.Priority = priority;
        nowCam = After_cam;
    }
}
