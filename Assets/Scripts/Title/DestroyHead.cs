using UnityEngine;

public class DestroyHead : MonoBehaviour
{
    //少しでも軽くする為に大量に配置するObjはtfキャッシュ
    [SerializeField] Transform _tf;
    [SerializeField] Rigidbody _rb;

    void Update()
    {
        if (_tf.position.z > 100f || _tf.position.z < -10 ||
            _tf.position.x > 70f || _tf.position.x < -70f ||
            _tf.position.y > 50f || _tf.position.y < -50f)
            ResetStatu();
    }

    void ResetStatu()
    {
        transform.position = transform.parent.position;
        _rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
