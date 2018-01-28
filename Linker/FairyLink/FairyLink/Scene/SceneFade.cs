using FairyLink.Device;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Scene
{
    enum FadeSwitch {
        Off,
        On,
    }

    enum FadeState {
        None,
        Out,
        In,
    }

    class SceneFade
    {
        private float fadeAlpha;      //フェードイン、フェードアウトの透明度
        private FadeSwitch fadeSwitch;      //フェードイン、フェードアウトのスイッチ
        private FadeState fadeState;
        private float fade;

        public SceneFade() {
            fadeState = FadeState.None;
            fade = 0.03f;
            fadeAlpha = 0;
            Initalize();
        }

        public void Initalize() {
            fadeSwitch = FadeSwitch.Off;
            fadeState = FadeState.None;
        }

        public void Update() {
            if (fadeSwitch == FadeSwitch.Off) { return; }

            fadeAlpha += fade;
            if (fadeAlpha > 1.0f) {
                fade *= -1;
                fadeState = FadeState.In;
            }
            else if (fadeAlpha < 0.0f) {
                fadeSwitch = FadeSwitch.Off;
                fadeState = FadeState.None;
                fade *= -1;
                fadeAlpha = 0;
            }
        }

        public FadeSwitch GetFadeSwitch{
            get { return fadeSwitch; }
            set { fadeSwitch = value; }
        }

        public FadeState GetFadeState
        {
            get { return fadeState; }
            set { fadeState = value; }
        }

        public float FadeAlpha {
            set { fadeAlpha = value; }
        }

        public void Draw(Renderer renderer) {
            renderer.DrawTexture("fade", Vector2.Zero, fadeAlpha);
        }
    }
}
