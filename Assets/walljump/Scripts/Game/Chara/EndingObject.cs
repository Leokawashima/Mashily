using System.Collections;
using UnityEngine;

public class EndingObject : MonoBehaviour
{
    [SerializeField] private Main_Process main;

    void OnCollisionEnter(Collision collision)
    {
        Initiate.Fade(Name.Scene.Title, Color.black, 1.0f);
        StartCoroutine(CursorState());
    }

    IEnumerator CursorState()
    {
        yield return new WaitForSeconds(1);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
