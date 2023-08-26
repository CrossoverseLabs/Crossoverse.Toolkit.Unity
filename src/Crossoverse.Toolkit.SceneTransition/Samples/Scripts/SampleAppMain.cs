using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Crossoverse.Toolkit.SceneTransition.Samples
{
    public sealed class SampleAppMain : MonoBehaviour
    {
        [SerializeField] private SceneConfiguration _sceneConfiguration;

        private async void Start()
        {
            Debug.Log($"<color=lime>[{nameof(SampleAppMain)}] StartAsync</color>");

            var sceneTransitionContext = new SceneTransitionContext(_sceneConfiguration);

            Debug.Log($"[{nameof(SampleAppMain)}] LoadGlobalScenesAndInitialStageAsync");
            await sceneTransitionContext.LoadGlobalScenesAndInitialStageAsync();

            Debug.Log($"[{nameof(SampleAppMain)}] LoadGlobalScenes");
            await sceneTransitionContext.LoadGlobalScenesAsync();

            Debug.Log($"[{nameof(SampleAppMain)}] UniTask.Delay");
            await UniTask.Delay(TimeSpan.FromSeconds(5));

            Debug.Log($"[{nameof(SampleAppMain)}] LoadStageAsync");
            await sceneTransitionContext.LoadStageAsync(StageName.Stage02);

            Debug.Log($"<color=lime>[{nameof(SampleAppMain)}] End of StartAsync</color>");
        }
    }
}
