using UnityEngine;

public class TitleOwner : MonoBehaviour
{
    public void GameStart()
    {
        Initiate.Fade(Name.Scene.Stage, Color.black, 1.0f);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
