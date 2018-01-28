using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Scene;
using FairyLink.Device;
using FairyLink.Utility;
using FairyLink.Def;

namespace FairyLink.Actor
{
    class Bullet: Character
    {
        private PieceControl pieceControl;

        public Bullet(string name,Stage stage, Vector2 position, Direction direction, CharacterControl characterControl, EffectControl effectControl, PieceControl pieceControl, GameDevice gameDevice)
            : base(name, new Vector2(32, 32), new Vector2(16, 16), stage, characterControl, effectControl, gameDevice)
        {
            speed.X = 10;
            this.position = position;
            this.direction = direction;
            attack = 1;
            this.pieceControl = pieceControl;
            sound.PlaySE("bullet");   //生成されると、音を流す
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
            if (character is Player) {
                ToPieces();
                isDead = true;
            }
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
                    effectControl.AddEffect(new Effect(EffectMode.BEAMCOLLISION, startPosition - size, timer, characterControl));
                    ToPieces();
                    isDead = true;
                }
            }
            else {
                Vector2 startPosition = position + new Vector2(collisionSize.X / 2, 0);
                if (!stage.CollisitionSide(startPosition, 1)) {
                    effectControl.AddEffect(new Effect(EffectMode.BEAMCOLLISION, startPosition - size, timer, characterControl));
                    ToPieces();
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

        /// <summary>
        /// 画像を分割して、オブジエンドとして保存して、欠片を管理するクラスに登録する
        /// </summary>
        private void ToPieces() {
            for (int i = 0; i < 32; i++) {
                pieceControl.Add(new Piece(name, 
                    new Rectangle((i % 8) * (int)size.X / 8, (i / 8) * (int)size.Y / 8, (int)size.X / 8, (int)size.Y / 8),
                    position, new Vector2(rand.Next(-3, 3),
                    rand.Next(-10, 0)), stage));
            }
        }

    }
}
