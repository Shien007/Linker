using FairyLink.Def;
using FairyLink.Device;
using FairyLink.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Scene.Scenes
{
    class SwitchDoor
    {

        private bool switchOn;
        private bool isOpen;
        private int x, y, dx = 0,dy = 0;
        private Vector2 doorPosition;
        private float doorPasent;
        private Motion motion;  //アニメーション用
        private Timer timer;


        public SwitchDoor(int x, int y)
        {
            this.x = x; //マップチップのx座標
            this.y = y; //マップチップのy座標
            Initalize();
        }

        public void Initalize()
        {
            switchOn = false;
            isOpen = false;
            timer = new Timer(0.01f);
            doorPasent = 1.0f;

            motion = new Motion();

            //画像を分割して保存
            for (int i = 0; i < 40; i++)
            {
                motion.Add(i, new Rectangle(StageMap.BlockSize * (i % 8), StageMap.BlockSize * (i / 8),
                    StageMap.BlockSize, StageMap.BlockSize));
            }
            //チェックされてない状態で初期化
            motion.Initialize(new Range(16, 16), new Timer(0.2f));
            
        }

        public void Update()
        {
            motion.Update();
            if (!switchOn) { return; }

            if (doorPasent <= 0.1f) {
                isOpen = true;
                return;
            }
            OpenDoor();
        }

        private void OpenDoor() {
            timer.Update();
            if (timer.IsTime)
            {
                doorPosition.Y += StageMap.BlockSize * 2 * 0.01f;
                doorPasent -= 0.01f;
                timer.Initialize();
            }
        }

        /// <summary>
        /// チェックポイントをチェックする
        /// </summary>
        public bool IsSwitchOn
        {
            get { return switchOn; }
            set
            {
                if (switchOn) { return; }
                switchOn = value;
                motion.Initialize(new Range(16, 17), new Timer(0.1f));
                motion.Roll = false;
            }
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public int DoorX {
            set {
                dx = value;
            }
        }
        public int DoorY
        {
            set {
                dy = value;
                if (dx != 0)
                {
                    doorPosition = new Vector2(dx * StageMap.BlockSize, dy * StageMap.BlockSize);
                }
                else {
                    doorPosition = Vector2.Zero;
                }
            }
        }

        public bool IsOpen {
            get { return isOpen; }
        }

        public void Draw(Renderer renderer, Stage stage)
        {
            //マップ上の座標を換算する
            int tx, ty;
            tx = x * StageMap.BlockSize;
            ty = y * StageMap.BlockSize;
            

            //Switch
            renderer.DrawTexture("gimmick", new Vector2(stage.GetScreenX(tx), ty), motion.DrawRange_L());

            //Door
            Rectangle doorRect = new Rectangle(2 * StageMap.BlockSize, 2 * StageMap.BlockSize, StageMap.BlockSize, (int)(StageMap.BlockSize * 2 * doorPasent));
            renderer.DrawTexture("gimmick", stage.GetScreenPosition(doorPosition), doorRect);
        }



    }
}
