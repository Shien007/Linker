using FairyLink.Def;
using FairyLink.Device;
using FairyLink.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Scene
{
    class StageFade
    {
        private FadeSwitch fadeSwitch;      //フェードイン、フェードアウトのスイッチ
        private Vector2[,] fadePosition;
        private List<Rectangle> fadeRect;
        private int fadeSize;
        private int rollTimer;
        private bool isEnd;
        

        public StageFade() {
            fadeSize = 64;
            fadePosition = new Vector2[StageMap.Width / fadeSize, StageMap.Height / fadeSize];
            fadeRect = new List<Rectangle>();
            Initalize();
        }

        public void Initalize() {
            rollTimer = 0;
            fadeSwitch = FadeSwitch.Off;;
            isEnd = false;

            for (int i = 0; i < 12; i++) {
                fadeRect.Add(new Rectangle(i * fadeSize, 0, fadeSize, fadeSize));
            }
            for (int j = 0; j < fadePosition.GetLength(0); j++) {
                for (int i = 0; i < fadePosition.GetLength(1); i++) {
                    fadePosition[j, i] = new Vector2(j * fadeSize, i * fadeSize);
                }
            }
        }

        public Rectangle Animetion(int rollTimer) {
            int num = rollTimer % 96 / 8;
            if (rollTimer >= 480) {
                return new Rectangle(0, 0, 0, 0);
            }
            else if (rollTimer >= 240) {
                return fadeRect[num];
            }
            return new Rectangle(0,0,0,0);
        }

        public void Update() {
            if (fadeSwitch == FadeSwitch.Off) { return; }
            rollTimer+=4;
            if (rollTimer >= 560) {
                Initalize();
            }
            else if(rollTimer >= 280) {
                isEnd = true;
            }
        }

        public FadeSwitch GetFadeSwitch{
            get { return fadeSwitch; }
            set {
                if (fadeSwitch == value) { return; }
                fadeSwitch = value;
            }
        }

        public bool IsEnd {
            get { return isEnd; }
        }


        public void Draw(Renderer renderer) {
            if (fadeSwitch == FadeSwitch.Off) { return; }
            int offset = 0;
            for (int j = 0; j < fadePosition.GetLength(0); j++)
            {
                for (int i = 0; i < fadePosition.GetLength(1); i++)
                {
                    renderer.DrawTexture("fadeImage", fadePosition[j, i], Animetion(rollTimer + offset));
                }
                offset+=8;
            }
        }
    }
}
