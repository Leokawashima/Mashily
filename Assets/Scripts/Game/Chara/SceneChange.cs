using UnityEngine;

public class SceneChange : MonoBehaviour
{
    [Header("変更したいシーンを選択")]
    public SceneObject scene;

    private void OnTriggerEnter(Collider other)
    {
        Initiate.Fade(scene, Color.black, 1.0f);
    }
}
