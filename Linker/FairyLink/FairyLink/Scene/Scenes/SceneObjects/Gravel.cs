using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;
using FairyLink.Def;
using FairyLink.Actor;


using Microsoft.Xna.Framework;

namespace FairyLink.Scene
{
    class Gravel
    {
        private bool crackStart;
        private int x, y;
        private Timer reLoad;
        private Motion motion;  //アニメーション用
        private Timer crackTimer;
        private int trembleTimer;
        private Vector2 tremble;
        private bool isDead;
        private PieceControl pieceControl;
        private Stage stage;
        private static Random rand = new Random();
        private Sound sound;

        public Gravel(PieceControl pieceControl, Stage stage, int x, int y, Sound sound)
        {
            crackStart = false;
            isDead = false;
            trembleTimer = 0;
            tremble = Vector2.Zero;
            this.x = x; //マップチップのx座標
            this.y = y; //マップチップのy座標
            crackTimer = new Timer(2.0f);
            reLoad = new Timer(3.0f);
            this.pieceControl = pieceControl;
            this.stage = stage;
            this.sound = sound;
            Initalize();
        }

        public void Initalize()
        {
            crackStart = false;
            isDead = false;
            crackTimer.Initialize();
            reLoad.Initialize();
            motion = new Motion();
            
            //画像を分割して保存
            for (int i = 0; i < 8; i++){
                motion.Add(i, new Rectangle(StageMap.BlockSize * (i % 8), StageMap.BlockSize * (i / 8),
                    StageMap.BlockSize, StageMap.BlockSize));
            }

            //チェックされてない状態で初期化
            motion.Initialize(new Range(4, 4), new Timer(1.0f));
        }

        public void Update() {
            if (isDead) { reLoad.Update(); }
            else {
                motion.Update();
                Crack();
            }
        }

        private void Crack() {
            if (isDead) { return; }
            if (crackStart) {
                crackTimer.Update();
                Tremble();
                if (crackTimer.IsTime) {
                    isDead = true;
                    sound.PlaySE("crumble");
                    tremble = Vector2.Zero;
                    ToPieces();
                    crackTimer.Initialize();
                }
            }
        }

        private void ToPieces() {
            for (int i = 0; i < 32; i++) {
                pieceControl.Add(new Piece("gimmick", 
                    new Rectangle(StageMap.BlockSize * 5 + (i % 8) * StageMap.BlockSize / 8, 
                        (i / 8) * StageMap.BlockSize / 8, 
                        StageMap.BlockSize / 8, StageMap.BlockSize / 8),

                    new Vector2(x * StageMap.BlockSize, y * StageMap.BlockSize), 
                    new Vector2(rand.Next(-5, 5), rand.Next(-5, 0)), stage));
            }
        }


        private void Tremble(){
            trembleTimer++;     //震える頻度を管理する
            if (trembleTimer == 4) { tremble = new Vector2(-1, 1); }    //左下にずらす
            if (trembleTimer == 8)
            {
                tremble = new Vector2(1, -1);       //右上にずらす
                trembleTimer = 0;           //頻度管理のtimerを初期化する
            }
        }


        /// <summary>
        /// チェックポイントをチェックする
        /// </summary>
        public bool CrackStart
        {
            get { return crackStart; }
            set
            {
                crackStart = value;
                if (crackStart) {
                    motion.Initialize(new Range(4, 5), new Timer(1.0f));       
                }
                else {
                    motion.Initialize(new Range(4, 4), new Timer(1.0f));
                }
            }
        }

        public bool IsDeath {
            get { return isDead; }
            set { isDead = value; }
        }

        public Timer IsReLoad {
            get { return reLoad; }
            set { reLoad = value; }
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public void Draw(Renderer renderer)
        {
            //マップ上の座標を換算する
            int tx, ty;
            tx = x * StageMap.BlockSize;
            ty = y * StageMap.BlockSize;

            renderer.DrawTexture("gimmick", new Vector2(stage.GetScreenX(tx), ty) + tremble, motion.DrawRange_L());
        }





    }
}
