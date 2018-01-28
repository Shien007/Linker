using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;
using FairyLink.Def;

using Microsoft.Xna.Framework;

namespace FairyLink.Scene.Scenes
{
    class Fire
    {
        private int x, y;
        private Motion motion;  //アニメーション用
        private Timer timer;

        public Fire(int x, int y) {
            this.x = x; //マップチップのx座標
            this.y = y; //マップチップのy座標
            timer = new Timer(0.2f);
            Initalize();
        }

        public void Initalize() {
            motion = new Motion();

            //画像を分割して保存
            for (int i = 0; i < 60; i++) {
                motion.Add(i, new Rectangle((int)StageMap.BlockSize * (i % 8), (int)StageMap.BlockSize * (int)(i / 8),
                    (int)StageMap.BlockSize, (int)StageMap.BlockSize));
            }

            //チェックされてない状態で初期化
            motion.Initialize(new Range(6, 7), timer);

        }

        public void Update() {
            motion.Update();

        }

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public void Draw(Renderer renderer, Stage stage) {
            //マップ上の座標を換算する
            int tx, ty;
            tx = x * StageMap.BlockSize;
            ty = y * StageMap.BlockSize;

            renderer.DrawTexture("gimmick", new Vector2(stage.GetScreenX(tx), ty), motion.DrawRange_L());
        }




    }





}
