using System;
using Cysharp.Threading.Tasks;

namespace Crossoverse.Toolkit.SceneTransition
{
    public interface ISceneTransitionContext<TStage, TScene>
        where TStage : struct, Enum
        where TScene : struct, Enum
    {
        UniTask LoadStageAsync(Stage<TStage, TScene> nextStage, IProgress<float> progress = null);
    }
}
