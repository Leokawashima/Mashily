using UnityEngine;

public class Ray_Ground : MonoBehaviour
{
    [Header("レイを飛ばす元となるカプセルCollider")]
    [SerializeField] CapsuleCollider _capCol;
    [SerializeField] float rayDistance = 0.1f;
    float actualDistance;

    public bool _floorIsHighJump { get; private set; }

    void Awake()
    {
        //カプセルコライダーの一番下の頂点　+　設定したレイの距離
        actualDistance = _capCol.height / 2f * gameObject.transform.localScale.y + rayDistance;
    }

    public bool RayCastCheckGround()
    {
        bool rayHit = false;

        Ray ray = new Ray(transform.position, Vector3.down);
#if UNITY_EDITOR
        Debug.DrawRay(transform.position, Vector3.down * actualDistance, Color.red);
#endif
        if(Physics.Raycast(ray, out RaycastHit hit, actualDistance))
        {
            rayHit = true;
            _floorIsHighJump = hit.collider.tag == Name.Tag.HighJumpFloor ? true : false;
        }

        return rayHit;
    }
}
