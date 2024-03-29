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

        public virtual async UniTask LoadGlobalScenesAndInitialStageAsync(TimeSpan delayTimeOfSwitchingActiveScene = default, IProgress<float> progress = null)
        {
            var totalScenesCount = _globalScenes.Count + _stages[0].SceneIds.Count;
            var globalScenesRate = (float) _globalScenes.Count / totalScenesCount;
            var stageScenesRate = (float) _stages[0].SceneIds.Count / totalScenesCount;

            await LoadGlobalScenesAsync(
                Progress.Create<float>(value => 
                {
                    progress?.Report(value * globalScenesRate);
                }));

            await LoadStageAsync(_stages[0], delayTimeOfSwitchingActiveScene,
                Progress.Create<float>(value => 
                {
                    progress?.Report(value * stageScenesRate + globalScenesRate);
                }));
        }

        public virtual async UniTask LoadGlobalScenesAsync(IProgress<float> progress = null)
        {
            var processedScenesCount = 0;
            var scenesToProcessCount = _globalScenes.Count;

            foreach (var scene in _globalScenes)
            {
                //
                // NOTE:
                // - Avoid reloading already loaded scenes.
                // - 既にロードされているシーンは再ロードしないようにする。
                //
                if (_loadedScenes.Contains(scene))
                {
                    scenesToProcessCount--;
                    continue;
                }

                _loadedScenes.Add(scene);
                await SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive)
                    .ToUniTask(Progress.Create<float>(value => 
                    {
                        progress?.Report((value + processedScenesCount) / scenesToProcessCount);
                    }));

                processedScenesCount++;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_initialActiveSceneId));
        }

        public virtual async UniTask LoadStageAsync(Stage<TStage, TScene> nextStage, TimeSpan delayTimeOfSwitchingActiveScene = default, IProgress<float> progress = null)
        {
            if (!_stages.Contains(nextStage)) return;

            var loadedScenesArray = _loadedScenes.ToArray();
            var scenesToProcessCount = loadedScenesArray.Length + nextStage.SceneIds.Count;
            var processedScenesCount = 0;

            // Unloading previous stage scenes
            foreach (var loadedScene in loadedScenesArray)
            {
                //
                // NOTE:
                // - Avoid unloading already loaded scenes if they will be used in the next stage.
                // - 既にロードされているシーンが次のステージで使われる場合はアンロードしないようにする。
                //
                if (nextStage.SceneIds.Contains(loadedScene) || _globalScenes.Contains(loadedScene))
                {                
                    scenesToProcessCount--;
                    continue;
                }

                _loadedScenes.Remove(loadedScene);
                await SceneManager.UnloadSceneAsync(loadedScene.ToString())
                    .ToUniTask(Progress.Create<float>(value => 
                    {
                        progress?.Report((value + processedScenesCount) / scenesToProcessCount);
                    }));

                processedScenesCount++;
            }

            // Loading next stage scenes
            foreach (var nextStageScene in nextStage.SceneIds)
            {
                //
                // NOTE:
                // - Avoid reloading already loaded scenes.
                // - 既にロードされているシーンは再ロードしないようにする。
                //
                if (_loadedScenes.Contains(nextStageScene))
                {
                    scenesToProcessCount--;
                    continue;
                }

                _loadedScenes.Add(nextStageScene);
                await SceneManager.LoadSceneAsync(nextStageScene.ToString(), LoadSceneMode.Additive)
                    .ToUniTask(Progress.Create<float>(value => 
                    {
                        progress?.Report((value + processedScenesCount) / scenesToProcessCount);
                    }));

                processedScenesCount++;
            }

            _previousStage = _currentStage;
            _currentStage = nextStage.StageId;

            await UniTask.Delay(delayTimeOfSwitchingActiveScene);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextStage.ActiveSceneId.ToString()));
        }
    }
}
