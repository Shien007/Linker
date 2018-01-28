using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FairyLink.Device;

namespace FairyLink.Scene
{
    class Operate : IScene
    {
        private Renderer renderer;
        private Sound sound;
        private InputState inputState;
        private int page;

        private bool isEnd;

        public Operate(GameDevice gameDevice) {
            renderer = gameDevice.GetRenderer;
            sound = gameDevice.GetSound;
            inputState = gameDevice.GetInputState;
            page = 1;
            isEnd = false;
        }

        public void Initialize() {
            page = 1;
            isEnd = false;
        }

        public void Update() {
            sound.PlayBGM("operate");
            if (inputState.WasDown(Keys.Enter) ||
                inputState.WasDown(Buttons.A) ||
                inputState.WasDown(Buttons.Start)) {
                if (page == 1) {
                    page++;
                }
                else {
                    sound.PlaySE("cancel");
                    isEnd = true;
                }
            } 
        }
        
        public void Draw(Renderer renderer) {
            if (page == 1) { renderer.DrawTexture("Operate1", Vector2.Zero); }
            else { renderer.DrawTexture("Operate2", Vector2.Zero); }
        }
        
        public void Shutdown() { }

        public bool IsEnd() { return isEnd; }
        public Scene Next() {
            Initialize();
            return Scene.TITLE; 
        }

    }
}
