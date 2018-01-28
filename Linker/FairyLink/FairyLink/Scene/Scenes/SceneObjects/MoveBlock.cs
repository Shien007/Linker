using FairyLink.Actor;
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
    /// <summary>
    /// 上下移動のブロックの処理
    /// </summary>
    class MoveBlock
    {

        private int x, y;

        private Motion motion;  //アニメーション用
        private Vector2 position;   //中心座標
        private Vector2 collisionSize;
        private Vector2 velocity;
        private Direction direction;
        private Stage stage;

        public MoveBlock(Stage stage, Direction direction, int x, int y)
        {
            this.x = x; //マップチップのx座標
            this.y = y; //マップチップのy座標
            collisionSize = new Vector2(8, 32);
            position = new Vector2(x * StageMap.BlockSize + 32, y * StageMap.BlockSize + 32);
            this.stage = stage;
            this.direction = direction;
            Initalize();
        }

        public void Initalize()
        {
            motion = new Motion();

            //画像を分割して保存
            for (int i = 0; i < 60; i++) {
                motion.Add(i, new Rectangle(StageMap.BlockSize * (i % 8), StageMap.BlockSize * (i / 8),
                    StageMap.BlockSize, StageMap.BlockSize));
            }

            //チェックされてない状態で初期化
            switch (stage.stageNumber) {
                case 1:
                    motion.Initialize(new Range(48, 48), new Timer(1.0f));
                    break;
                case 2:
                    motion.Initialize(new Range(49, 49), new Timer(1.0f));
                    break;
                default:
                    motion.Initialize(new Range(48, 48), new Timer(1.0f));
                    break;
            }            
        }

        public void Update()
        {
            motion.Update();
            Move();
        }

        private void Move() {
            if (direction == Direction.UP)
            {
                velocity = new Vector2(0, -1);
                if (position.Y < -collisionSize.Y)
                {
                    position.Y = StageMap.Height + collisionSize.Y;
                }
            }
            else if (direction == Direction.DOWN)
            {
                velocity = new Vector2(0, 1);
                if (position.Y > StageMap.Height + collisionSize.Y)
                {
                    position.Y = -collisionSize.Y;
                }
            }
            else {
                velocity = Vector2.Zero;
            }
            position += velocity;
        }

        
        public Direction GetDirection { get { return direction; } }
        public Vector2 GetVelocity { get { return velocity; } }
        public Vector2 GetPosition { get { return position; } }
        public void Draw(Renderer renderer) {
            renderer.DrawTexture("gimmick",stage.GetScreenPosition(position) - new Vector2(32,32), motion.DrawRange_L());
        }



    }
}