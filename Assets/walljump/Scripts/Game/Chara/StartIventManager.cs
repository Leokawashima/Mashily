using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class StartIventManager : MonoBehaviour
{
    [SerializeField] PlayableDirector director;

    public delegate void Ivent();
    public static event Ivent PlayableStartIvent;
    public static event Ivent PlayableEndIvent;

    void OnEnable()
    {
        director.played += OnDirectorPlayed;
        director.stopped += OnDirectorStopped;
    }
    void OnDisable()
    {
        director.played -= OnDirectorPlayed;
        director.stopped -= OnDirectorStopped;
    }

    void Start()
    {
        director.Play();
    }

    private void OnDirectorPlayed(PlayableDirector obj)
    {
        PlayableStartIvent?.Invoke();
    }

    private void OnDirectorStopped(PlayableDirector obj)
    {
        PlayableEndIvent?.Invoke();
    }

}