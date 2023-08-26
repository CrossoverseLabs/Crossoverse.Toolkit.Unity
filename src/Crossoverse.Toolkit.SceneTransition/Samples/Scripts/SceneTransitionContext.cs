using System;
using Cysharp.Threading.Tasks;

namespace Crossoverse.Toolkit.SceneTransition.Samples
{
    public sealed class SceneTransitionContext : SceneTransitionContext<StageName, SceneName>
    {
        public SceneTransitionContext(SceneConfiguration sceneConfiguration) : base(sceneConfiguration)
        {
        }

        public async UniTask LoadStageAsync(StageName nextStageId, TimeSpan delayTimeOfSwitchingActiveScene = default, IProgress<float> progress = null)
        {
            foreach (var stage in _stages)
            {
                if (stage.StageId == nextStageId)
                {
                    await base.LoadStageAsync(stage, delayTimeOfSwitchingActiveScene, Progress.Create<float>(value => 
                    {
                        UnityEngine.Debug.Log($"<color=lime>[{nameof(Samples.SceneTransitionContext)}] Stage loading progress: {value}</color>");
                    }));
                    return;
                }
            }
        }

        public override async UniTask LoadGlobalScenesAndInitialStageAsync(TimeSpan delayTimeOfSwitchingActiveScene = default, IProgress<float> progress = null)
        {
            await base.LoadGlobalScenesAndInitialStageAsync(delayTimeOfSwitchingActiveScene, Progress.Create<float>(value => 
            {
                UnityEngine.Debug.Log($"<color=lime>[{nameof(Samples.SceneTransitionContext)}] Stage loading progress: {value}</color>");
            }));
        }
    }
}
