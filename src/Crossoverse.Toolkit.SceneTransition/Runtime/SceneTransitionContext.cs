using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Crossoverse.Toolkit.SceneTransition
{    
    public class SceneTransitionContext<TStage, TScene> : ISceneTransitionContext<TStage, TScene>
        where TStage : struct, Enum
        where TScene : struct, Enum
    {
        public TStage CurrentStage => _currentStage;
        public TStage PreviousStage => _previousStage;

        protected readonly Stage<TStage, TScene>[] _stages;
        protected readonly string _initialActiveSceneId;
        protected readonly HashSet<TScene> _globalScenes;
        protected readonly HashSet<TScene> _loadedScenes;

        protected TStage _currentStage;
        protected TStage _previousStage;

        public SceneTransitionContext(ISceneConfiguration<TStage, TScene> sceneConfiguration)
        {
            _stages = sceneConfiguration.Stages.ToArray();
            _initialActiveSceneId = sceneConfiguration.InitialActiveScene.ToString();
            _globalScenes = sceneConfiguration.GlobalScenes.ToHashSet();
            _loadedScenes = new HashSet<TScene>();
        }

        public virtual async UniTask LoadGlobalScenesAndInitialStageAsync()
        {
            await LoadGlobalScenesAsync(true);
            await TransitAsync(_stages[0]);
        }

        public virtual async UniTask LoadStageAsync(Stage<TStage, TScene> nextStage)
        {
            if (_stages.Contains(nextStage))
            {
                await TransitAsync(nextStage);
            }
        }

        public virtual async UniTask LoadGlobalScenesAsync(bool onInitialize = false)
        {
            foreach (var scene in _globalScenes)
            {
                await SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_initialActiveSceneId));
        }

        public virtual async UniTask TransitAsync(Stage<TStage, TScene> nextStage)
        {
            foreach (var loadedScene in _loadedScenes.ToArray())
            {
                //
                // NOTE:
                // - Avoid unloading already loaded scenes if they will be used in the next stage.
                // - 既にロードされているシーンが次のステージで使われる場合はアンロードしないようにする。
                //
                if (nextStage.SceneIds.Contains(loadedScene)) continue;

                await SceneManager.UnloadSceneAsync(loadedScene.ToString());
                _loadedScenes.Remove(loadedScene);
            }

            foreach (var nextStageScene in nextStage.SceneIds)
            {
                //
                // NOTE:
                // - Avoid reloading already loaded scenes.
                // - 既にロードされているシーンは再ロードしないようにする。
                //
                if (_loadedScenes.Contains(nextStageScene)) continue;

                await SceneManager.LoadSceneAsync(nextStageScene.ToString(), LoadSceneMode.Additive);
                _loadedScenes.Add(nextStageScene);
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextStage.ActiveSceneId.ToString()));

            _previousStage = _currentStage;
            _currentStage = nextStage.StageId;
        }
    }
}
