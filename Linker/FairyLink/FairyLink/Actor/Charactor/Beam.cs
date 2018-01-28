using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Scene;
using FairyLink.Device;
using FairyLink.Utility;

namespace FairyLink.Actor
{
    class Beam: Character
    {
        public Beam(Stage stage, Vector2 position, Direction direction, CharacterControl characterControl, EffectControl effectControl, GameDevice gameDevice)
            : base("beam", new Vector2(32, 32), new Vector2(32, 6), stage, characterControl, effectControl, gameDevice)
        {
            speed.X = 10;
            this.position = position;
            this.direction = direction;
            attack = 5;
            sound.PlaySE("beam");   //生成されると、音を流す
        }

        public override void Initialize() {
            isDead = false;
        }

        public override void Update() {
            IsDeath();
            Move();
            ReverseWall();  //壁と当たると死ぬ
        }

        public override void Hit() { }
        public override void Hit(Character character) {
            if (character is Enemy_Slime || character is Enemy_Seed) { isDead = true; }
        }

        private void IsDeath() {
            //画面外に落ちると死ぬ
            if (!InMap(position)) { isDead = true; }
        }

        #region マープ認識

        private void ReverseWall() {
            if (mode == ActionMode.DEATH) { return; }
            if (direction == Direction.RIGHT) {
                Vector2 startPosition = position + new Vector2(collisionSize.X / 2, 0);
                if (!stage.CollisitionSide(startPosition, 1)) {
                    effectControl.AddEffect(new Effect(EffectMode.BEAMCOLLISION, startPosition - size, timer,characterControl));
                    isDead = true;
                }
            }
            else {
                Vector2 startPosition = position + new Vector2(collisionSize.X / 2, 0);
                if (!stage.CollisitionSide(startPosition, 1)) {
                    effectControl.AddEffect(new Effect(EffectMode.BEAMCOLLISION, startPosition - size, timer, characterControl));
                    isDead = true;
                }
            }
        }

        #endregion

        #region 動き
        
        private void Move() {
            if (direction == Direction.RIGHT) { MoveRight();  }
            else { MoveLeft();  }
        }

        private void MoveRight() { position.X += speed.X; }
        private void MoveLeft() { position.X -= speed.X; }

        #endregion

    }
}
