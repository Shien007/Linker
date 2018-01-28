using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FairyLink.Device;

namespace FairyLink.Scene
{
    class SceneManager
    {
        private Dictionary<Scene, IScene> scenes = new Dictionary<Scene,IScene>();
        private IScene currentScene;
        private SceneFade fade;
        private bool isFade;

        public SceneManager() {
            fade = new SceneFade();
            currentScene = null;
            isFade = false;
        }

        public void Update() {
            if (currentScene == null) { return; }
            currentScene.Update();

            if (currentScene.IsEnd()) {
                if (!isFade)
                {
                    fade.GetFadeSwitch = FadeSwitch.On;
                    isFade = true;
                }
                else {
                    if (fade.GetFadeState == FadeState.In)
                    {
                        Change(currentScene.Next());
                    }
                }
            }

            if (fade.GetFadeSwitch == FadeSwitch.Off) { return; }
            fade.Update();
        }

        public void Add(Scene name, IScene scene) {
            if (scenes.ContainsKey(name)) { return; }
            scenes.Add(name, scene);
        }

        public void Change(Scene name) {
            if (currentScene != null) { currentScene.Shutdown(); }
            isFade = false;
            currentScene = scenes[name];
            currentScene.Initialize();
        }

        public void Draw(Renderer renderer) {
            if (currentScene == null) { return; }
            currentScene.Draw(renderer);
            if (fade.GetFadeSwitch == FadeSwitch.Off) { return; }
            fade.Draw(renderer);
        }


    }
}
