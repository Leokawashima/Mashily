using UnityEngine;

public class Ray_Wall : MonoBehaviour
{
    [SerializeField] float rayDistance = 0.3f;
    
    public RaycastHit _rayInfo { get; private set; }
    public bool _wallIsHighJump { get; private set; }

    public bool RayCastCheck()
    {
        bool rayHit = false;
        Ray ray = new Ray(transform.position, transform.forward);

#if UNITY_EDITOR
        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.blue);
#endif
        if(Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            //����͒ʏ�ǂƓ���ǂ̓�݂̂Ȃ̂ŎG�Ƀ^�O�������Ă��邪�A
            //�^�O�̑O���̖��O��Wall_???�Ƃ��ɂ���SubString�Ŗ��O�𔲂��o���Ă�����ׂ�
            //�������������̃^�O�͐ݒ�Ɋւ�����񂵂������̂ŒN�������J���Ă��郉�C�u�������g����
            //gameobject�N���X���������邵���Ȃ�
            //�����K�������߂Ė��O�O�����甲���o������Ȃ񂾂�ł��ǂ����A
            //�A�N�Z�X�����������̂�GetComponent��Ray���ɂ����鎖�͐�΂ɂ������Ȃ��̂ŁA
            //�����I�Ȋg���݌v���l����̂Ȃ�Unity�̃^�O���Ǘ����₷������A
            //�܂����̃^�O�ɊY������v���n�u������������V�[����̃I�u�W�F�N�g�������N���b�N�ŗ񋓂���Ȃǂ�
            //EditorWIndow������Ă�����ׂ��@�ʔ������Ȃ̂ő��Ƃ܂łɍ�肽��
            if(hit.collider.gameObject.tag == Name.Tag.Wall)
            {
                _rayInfo = hit;
                rayHit = true;
                _wallIsHighJump = false;
            }
            else if(hit.collider.gameObject.tag == Name.Tag.HighJumpWall)
            {
                _rayInfo = hit;
                rayHit = true;
                _wallIsHighJump = true;
            }
        }

        return rayHit;
    }
}
