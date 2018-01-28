using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Actor;
using FairyLink.Def;
using FairyLink.Device;
using FairyLink.Utility;
using FairyLink.Scene;

using Microsoft.Xna.Framework;


namespace FairyLink.Actor
{
    class Battery : Character
    {
        private Motion motion;  //アニメーション用
        private int trembleTimer;
        private Vector2 tremble;
        private Timer shootTimer;
        private Timer shootingTimer;
        private bool isShooting;
        private PieceControl pieceControl;

        public Battery(Stage stage, Vector2 position, int speedX, Direction direction, CharacterControl characterControl, EffectControl effectControl, PieceControl pieceControl, GameDevice gameDevice)
            : base("Battery", new Vector2(64, 64), new Vector2(64, 64), stage, characterControl, effectControl, gameDevice)
        {
            trembleTimer = 0;
            tremble = Vector2.Zero;
            this.pieceControl = pieceControl;
            this.position = position;
            this.direction = direction;
            shootTimer = new Timer(2.0f);
            shootingTimer = new Timer(0.4f);
            direction = Direction.LEFT;
            isShooting = false;
            Initalize();
        }

        public void Initalize()
        {
            motion = new Motion();

            //画像を分割して保存
            for (int i = 0; i < 2; i++) {
                motion.Add(i, new Rectangle((int)size.X * (i % 2), (int)size.Y * (i / 2),(int)size.X, (int)size.Y));
            }

            //チェックされてない状態で初期化
            motion.Initialize(new Range(1, 1), new Timer(1.0f));
        }


        public override void Hit()
        {
            throw new NotImplementedException();
        }

        public override void Hit(Character other)
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            if (isShooting) {
                shootingTimer.Update();
                Tremble();
                if (shootingTimer.IsTime) {
                    motion.Initialize(new Range(1, 1), new Timer(1.0f));
                    shootingTimer.Initialize();
                    isShooting = false;
                }
            }
            motion.Update();
            ShootUpdate();
        }

        #region shoot
        private void ShootUpdate() {
            shootTimer.Update();
            if (shootTimer.IsTime)
            {
                isShooting = true;
                motion.Initialize(new Range(0, 1), new Timer(0.2f));
                if (direction == Direction.LEFT) { ShootLeft(); }
                else { ShootRight(); }
                shootTimer.Initialize();
                trembleTimer = 0;
                tremble = Vector2.Zero;
            }
        }

        private void ShootLeft() {
            characterControl.AddCharacter(new Bullet("shell",stage, position - new Vector2(size.X / 4, -30), direction, characterControl, effectControl, pieceControl, gameDevice));
        }

        private void ShootRight() {
            characterControl.AddCharacter(new Bullet("shell",stage, position + new Vector2(size.X / 4, 30), direction, characterControl, effectControl, pieceControl, gameDevice));
        }

        #endregion

        private void Tremble()
        {
            trembleTimer++;     //震える頻度を管理する
            if (trembleTimer == 0) {
                tremble = new Vector2(2, 0);   //左にずらす
            }    
            if (trembleTimer == 11) {
                tremble = new Vector2(-2, 0);       //右にずらす
                trembleTimer = 0;           //頻度管理のtimerを初期化する
            }
        }

        public override void Draw(Renderer renderer)
        {
            if (direction == Direction.LEFT)
            {
                renderer.DrawTexture(name, stage.GetScreenPosition(position) + tremble, motion.DrawRange_L(), alpha);
            }
            else {
                renderer.DrawTexture(name + "2", stage.GetScreenPosition(position) + tremble, motion.DrawRange_R(), alpha);
            }
        }

    }
}
