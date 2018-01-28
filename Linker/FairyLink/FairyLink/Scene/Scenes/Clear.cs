using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FairyLink.Device;
using FairyLink.Utility;

namespace FairyLink.Scene
{
    class Clear : IScene
    {
        private Renderer renderer;
        private Sound sound;
        private InputState inputState;
        private Rankings rankings;

        private bool isEnd;

        public Clear(GameDevice gameDevice)
        {
            renderer = gameDevice.GetRenderer;
            sound = gameDevice.GetSound;
            inputState = gameDevice.GetInputState;
            rankings = gameDevice.GetRankings;
            isEnd = false;
        }

        public void Initialize() { isEnd = false; }

        public void Update(){
            sound.PlayBGM("clear");
            if (inputState.WasDown(Keys.Enter) ||
                inputState.WasDown(Buttons.A) ||
                inputState.WasDown(Buttons.Start)) {
                sound.PlaySE("cancel");
                isEnd = true;
            }
        }

        public void Draw(Renderer renderer)
        {
            renderer.DrawTexture("Clear", Vector2.Zero);
            renderer.DrawNumber("number", new Vector2(550, 375), rankings.DeathCount);
            renderer.DrawNumber("number", new Vector2(510, 450), rankings.TimeCalculat(rankings.PlayTime)[0]);
            renderer.DrawNumber("number", new Vector2(660, 450), rankings.TimeCalculat(rankings.PlayTime)[1]);
        }

        public void Shutdown() { }

        public bool IsEnd() { return isEnd; }
        public Scene Next()
        {
            Initialize();
            return Scene.RANKING;
        }

    }
}
