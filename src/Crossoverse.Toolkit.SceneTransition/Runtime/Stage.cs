using System;
using System.Collections.Generic;

namespace Crossoverse.Toolkit.SceneTransition
{
    [Serializable]
    public class Stage<TStage, TScene>
        where TStage : struct, Enum
        where TScene : struct, Enum
    {
        public TStage StageId = default;
        public TScene ActiveSceneId = default;
        public List<TScene> SceneIds = new();
    }
}
