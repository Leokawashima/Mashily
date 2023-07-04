using UnityEngine;

public class HeadManager : MonoBehaviour
{
    [Header("�v���t�@�u")]
    [SerializeField] GameObject _headPrefab;
    [SerializeField] uint _headNum = 0;
    [Header("�������X�g�ݒ�")]
    [SerializeField] uint _headCapacity = 100;
    [SerializeField] string _headName = "Head";
    [Header("�������X�g")]
    [SerializeField] GameObject[] _headList;

    //Awake�ł͎q�I�u�W�F�N�g���𐳂���������m�؂��Ȃ��̂�Start�Ń��X�g�𐶐�����
    void Start()
    {
        _headList = new GameObject[_headCapacity];
        //Start���Ő������[�v���񂷂̂������ł��y�������������̂ŋ߂����Ƃ���ɃL���b�V�����Ă���
        var tf = transform;
        //���[�v�J�n�n�_���V���A���C�Y�z��̒����Ŋ��蓖�Ă邱�Ƃōŏ�����z�u���������̐�(��{1������...)
        //�����������Ă��Ή��ł���悤�ɂ��̂悤�ɂ���
        var count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            var go = transform.GetChild(i).gameObject;
            _headList[i] = go;
            go.name = _headName + "_" + i;
        }
        for (int i = count; i < _headCapacity; i++)
        {
            var go = Instantiate(_headPrefab, tf.position, transform.rotation, tf);
            _headList[i] = go;
            go.name = _headName + "_" + i;
            go.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var go = hit.collider.gameObject;
                //�T�u�X�g�����O�œ����������v���Ă��邩���m�F
                //(Start���ŏ��������Ă���̂Ŏ��s���ɔz�u�ς݂̓��̖��O�����l�[�������)
                if (go.name.Substring(0, _headName.Length) == _headName)
                {
                    var rb = go.GetComponent<Rigidbody>();
                    rb.AddForce(0, 0, 100, ForceMode.Impulse);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            CreateNewHead();
        }
    }

    void CreateNewHead()
    {
        for (int i = 0; i < _headCapacity; i++)
        {
            var offset = (_headNum + i) % _headCapacity;
            if(_headList[offset].activeSelf == false)
            {
                _headList[offset].SetActive(true);
                _headNum++;
                break;
            }
        }
    }
}
