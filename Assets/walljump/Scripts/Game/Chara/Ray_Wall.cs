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
            //今回は通常壁と特殊壁の二つのみなので雑にタグ分けしているが、
            //タグの前半の名前をWall_???とかにしてSubStringで名前を抜き出してあげるべき
            //そもそも既存のタグは設定に関する取り回しが悪いので誰かが公開しているライブラリを使うか
            //gameobjectクラスを改造するしかない
            //命名規則を決めて名前前半から抜き出したりなんだりでも良いが、
            //アクセス効率が悪いのでGetComponentをRay毎にさせる事は絶対にしたくないので、
            //将来的な拡張設計を考えるのならUnityのタグを管理しやすくする、
            //またそのタグに該当するプレハブを検索したりシーン上のオブジェクトをワンクリックで列挙するなどの
            //EditorWIndowを作ってあげるべき　面白そうなので卒業までに作りたい
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
