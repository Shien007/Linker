using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FairyLink.Device;

namespace FairyLink.Scene
{
    class StaffRoll : IScene
    {
        private Renderer renderer;
        private Sound sound;
        private InputState inputState;

        private bool isEnd;

        public StaffRoll(GameDevice gameDevice) {
            renderer = gameDevice.GetRenderer;
            sound = gameDevice.GetSound;
            inputState = gameDevice.GetInputState;
            isEnd = false;
        }

        public void Initialize() { isEnd = false; }

        public void Update() {
            sound.PlayBGM("staffroll");
            if (inputState.WasDown(Keys.Enter) ||
                inputState.WasDown(Buttons.A) ||
                inputState.WasDown(Buttons.Start)) {
                sound.PlaySE("cancel");
                isEnd = true;
            } 
        }
        
        public void Draw(Renderer renderer) {
            renderer.DrawTexture("staffRoll", Vector2.Zero);
        }
        
        public void Shutdown() { }

        public bool IsEnd() { return isEnd; }
        public Scene Next() {
            Initialize();
            return Scene.TITLE; 
        }

    }
}
