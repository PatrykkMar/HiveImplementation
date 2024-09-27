using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService
{
    private SynchronizationContext _mainThreadContext;

    public SceneService(string defaultScene)
    {
        CurrentScene = defaultScene;
        _mainThreadContext = SynchronizationContext.Current;
    }

    public event Action<string> OnSceneChange;

    public string CurrentScene { get; private set; }

    public bool ChangeSceneForStateIfNecessary(ClientState state)
    {
        Debug.Log(IsMainThread());
        var strategy = StateStrategyFactory.GetStrategy(state);

        if (strategy.Scene != CurrentScene)
        {
            if (IsMainThread())
            {
                ChangeScene(strategy.Scene);
            }
            else
            {
                _mainThreadContext.Post(_ => ChangeScene(strategy.Scene), null);
            }
            return true;
        }
        return false;
    }

    private bool IsMainThread()
    {
        return SynchronizationContext.Current == _mainThreadContext;
    }

    private void ChangeScene(string sceneName)
    {
        Debug.Log($"Changing scene to: {sceneName}");
        SceneManager.LoadScene(sceneName);
        CurrentScene = sceneName;
        OnSceneChange?.Invoke(sceneName);
    }
}