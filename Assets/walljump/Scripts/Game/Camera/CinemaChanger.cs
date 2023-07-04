using UnityEngine;
using Cinemachine;

public class CinemaChanger: MonoBehaviour
{
    //�g�p�������J������񋓌^�z������̂Ŏ����ł��̃X�N���v�g�Ɋ��蓖�ĂĂ�
    //�v�f�����v�f���̏��ŕύX�������J���������g���Ă�J�����Ȃ�ς����Ȃ���
    [SerializeField] CinemachineVirtualCamera[] _vCam;
    public CinemachineVirtualCamera[] VCam { get { return _vCam; } }

    CinemachineVirtualCamera nowCam;

    void Start()
    {
            nowCam = _vCam[0];
    }

    public void ChangeVCamera(CinemachineVirtualCamera After_cam)
    {
        //�v�́@�J�����P���J�����Q�@�������J�����Q�����g���Ă�J�������w�肳��Ă�����ύX���Ȃ�����
        if(After_cam.Priority == 20)
            return;
        int priority = nowCam.Priority;
        nowCam.Priority = After_cam.Priority;
        After_cam.Priority = priority;
        nowCam = After_cam;
    }
}
