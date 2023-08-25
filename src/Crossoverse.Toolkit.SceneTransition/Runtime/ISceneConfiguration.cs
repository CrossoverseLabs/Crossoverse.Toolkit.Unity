using System;
using System.Collections.Generic;

namespace Crossoverse.Toolkit.SceneTransition
{
    public interface ISceneConfiguration<TStage, TScene>
        where TStage : struct, Enum
        where TScene : struct, Enum
    {
        TScene InitialActiveScene { get; }
        List<TScene> GlobalScenes { get; }
        List<Stage<TStage, TScene>> Stages { get; }
    }
}
