using UnityEngine;

public class Swhich : MonoBehaviour
{
    [SerializeField]
    private GameObject destroyObj;

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
        Destroy(destroyObj);
    }
}
