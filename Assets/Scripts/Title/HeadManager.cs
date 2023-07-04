using UnityEngine;

public class HeadManager : MonoBehaviour
{
    [Header("プレファブ")]
    [SerializeField] GameObject _headPrefab;
    [SerializeField] uint _headNum = 0;
    [Header("生成リスト設定")]
    [SerializeField] uint _headCapacity = 100;
    [SerializeField] string _headName = "Head";
    [Header("生成リスト")]
    [SerializeField] GameObject[] _headList;

    //Awakeでは子オブジェクト情報を正しく得られる確証がないのでStartでリストを生成する
    void Start()
    {
        _headList = new GameObject[_headCapacity];
        //Start内で生成ループを回すのを少しでも軽く押さえたいので近しいところにキャッシュしておく
        var tf = transform;
        //ループ開始地点をシリアライズ配列の長さで割り当てることで最初から配置したい頭の数(基本1だけど...)
        //が複数あっても対応できるようにこのようにする
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
                //サブストリングで頭文字が合致しているかを確認
                //(Start内で初期化しているので実行時に配置済みの頭の名前もリネームされる)
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
