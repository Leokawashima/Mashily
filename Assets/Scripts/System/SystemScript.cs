using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemScript: MonoBehaviour
{
    [Header("カーソルの表示")]
    [SerializeField] bool ShowCursor = true;

    void Awake()
    {
        Application.targetFrameRate = 60;
        if(!ShowCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
            Application.Quit();
        else if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(Name.Scene.Title);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}