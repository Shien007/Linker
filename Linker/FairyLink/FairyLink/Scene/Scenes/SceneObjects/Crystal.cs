using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Device;
using FairyLink.Def;
using FairyLink.Utility;

namespace FairyLink.Scene
{
    /// <summary>
    /// チェックポイントギミック
    /// </summary>
    class Crystal
    {
        private bool burn;
        private int x, y;
        private Motion motion;  //アニメーション用
        private Timer timer;

        public Crystal(int x, int y) {
            
            this.x = x; //マップチップのx座標
            this.y = y; //マップチップのy座標
            timer = new Timer(0.2f);
            Initalize();
        }

        public void Initalize() {
            //チェックされてない状態で初期化
            burn = false;

            motion = new Motion();
            //画像を分割して保存
            for (int i = 0; i < 8; i++) {
                motion.Add(i, new Rectangle(StageMap.BlockSize * (i % 8), StageMap.BlockSize * (i / 8),
                    StageMap.BlockSize, StageMap.BlockSize));
            }
            motion.Initialize(new Range(2, 3), timer);

        }

        public void Update() {
            motion.Update();

        }

        /// <summary>
        /// チェックポイントをチェックする
        /// </summary>
        public bool Burn {
            get { return burn; }
            set {
                if (burn) { return; }
                burn = value;
                if (burn) {
                    motion.Initialize(new Range(0, 1), timer);
                }
                else {
                    motion.Initialize(new Range(2, 3), timer);
                }
            }
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public void Draw(Renderer renderer, Stage stage){
            //マップ上の座標を換算する
            int tx, ty;
            tx = x * StageMap.BlockSize;
            ty = y * StageMap.BlockSize;

            renderer.DrawTexture("gimmick", new Vector2(stage.GetScreenX(tx), ty), motion.DrawRange_L());
        }




    }
}
