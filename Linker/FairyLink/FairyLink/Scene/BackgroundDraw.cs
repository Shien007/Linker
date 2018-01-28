using FairyLink.Def;
using FairyLink.Device;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Scene
{
    class BackgroundDraw
    {
        private Vector2 rollPosition1;      //スクロール中景１の描画座標
        private Vector2 rollPosition2;      //スクロール中景２の描画座標
        private int timer;                  //中景スクロール用

        public BackgroundDraw() {
            rollPosition1 = Vector2.Zero;
            rollPosition2 = new Vector2(Screen.Width, 0);
            timer = 0;
        }

        public void Update() {
            timer++;
            if (timer > 60) { timer = 0; }

            rollPosition1.X--;
            rollPosition2.X--;
            if (rollPosition1.X <= -Screen.Width) { rollPosition1.X = rollPosition2.X + Screen.Width; }
            if (rollPosition2.X <= -Screen.Width) { rollPosition2.X = rollPosition1.X + Screen.Width; }
        }


        public void Draw(Renderer renderer, int stageNum, Vector2 offset) {
            DrawBackground(renderer, stageNum);
            DrawRoll(renderer, stageNum, offset);
            DrawRollAnimetion(renderer, stageNum, offset);
            DrawAnimetion(renderer, stageNum);
        }


        public void DrawBackground(Renderer renderer, int stageNum) {
            renderer.DrawTexture("stage" + stageNum + "_background", Vector2.Zero);
        }

        //中景のスクロール
        public void DrawRoll(Renderer renderer, int stageNum, Vector2 offset)
        {
            renderer.DrawTexture("stage" + stageNum + "_background_R1", rollPosition1 + offset);
            renderer.DrawTexture("stage" + stageNum + "_background_R2", rollPosition2 + offset);
        }

        //中景のアニメーションとスクロール
        public void DrawRollAnimetion(Renderer renderer, int stageNum, Vector2 offset) {
            if (timer <= 30)
            {
                renderer.DrawTexture("stage" + stageNum + "_background_RA_1", rollPosition1 + offset);
                renderer.DrawTexture("stage" + stageNum + "_background_RA_2", rollPosition2 + offset);
            }
            else {
                renderer.DrawTexture("stage" + stageNum + "_background_RA_1_2", rollPosition1 + offset);
                renderer.DrawTexture("stage" + stageNum + "_background_RA_2_2", rollPosition2 + offset);
            }
        }

        //中景のアニメーション
        public void DrawAnimetion(Renderer renderer, int stageNum)
        {
            if (timer <= 30)
            {
                renderer.DrawTexture("stage" + stageNum + "_background_A1", Vector2.Zero);
            }
            else {
                renderer.DrawTexture("stage" + stageNum + "_background_A2", Vector2.Zero);
            }
        }

    }
}
