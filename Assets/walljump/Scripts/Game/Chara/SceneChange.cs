using UnityEngine;

public class SceneChange : MonoBehaviour
{
    [Header("�ύX�������V�[����I��")]
    public SceneObject scene;

    private void OnTriggerEnter(Collider other)
    {
        Initiate.Fade(scene, Color.black, 1.0f);
    }
}
