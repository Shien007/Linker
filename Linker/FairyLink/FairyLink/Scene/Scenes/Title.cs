using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FairyLink.Device;

namespace FairyLink.Scene
{
    class Title : IScene
    {
        private Renderer renderer;
        private Sound sound;
        private InputState inputState;
        private TitleSelect titleSelect;

        private bool isEnd;

        public Title(GameDevice gameDevice) {
            renderer = gameDevice.GetRenderer;
            sound = gameDevice.GetSound;
            inputState = gameDevice.GetInputState;
            titleSelect = new TitleSelect(inputState);
            isEnd = false;
            Initialize();
        }

        public void Initialize() {
            isEnd = false;
            titleSelect.Initialize();
        }
         
        public void Update() {
            sound.PlayBGM("title");
            titleSelect.Update();
            if (inputState.WasDown(Keys.Enter) ||
                inputState.WasDown(Buttons.A) ||
                inputState.WasDown(Buttons.Start)) {
                sound.PlaySE("beam");
                titleSelect.IsSelected = true;
            }
            if (titleSelect.IsSelectEnd) { isEnd = true; }
        }
        
        public void Draw(Renderer renderer) {
            renderer.DrawTexture("Title", Vector2.Zero);
            titleSelect.Draw(renderer);
        }
        public void Shutdown() { }

        public bool IsEnd() { return isEnd; }
        public Scene Next() {
            if (titleSelect.GetSelect == 1) { return Scene.GAMEPLAY; }
            else if (titleSelect.GetSelect == 3) { return Scene.STAFFROLL; }
            else { return Scene.OPERATE; }
        }

    }
}
