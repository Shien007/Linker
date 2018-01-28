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

    /// <summary>
    /// 遠距離タイプ敵
    /// </summary>
    class Enemy_Seed : Character
    {
        private List<Vector2> damegePosition;   //攻撃された時のダメージを描画する座標を保存用
        private int damege;     //攻撃された時のダメージ
        private int speedX;     //x方向の速度
        private int linkTime;   //リンク攻撃を受けた時のカウントダウン（1秒linkされると死ぬ）
        private Motion motion;      //アニメーション用

        private Timer shootTimer;   //シュート状態の保持時間
        private int shooting;
        private PieceControl pieceControl;  //分割された画像を管理用用

        public Enemy_Seed(Stage stage, Vector2 position, int speedX, Direction direction, CharacterControl characterControl, EffectControl effectControl, PieceControl pieceControl, GameDevice gameDevice)
            : base("enemy_seed", new Vector2(96, 96), new Vector2(48, 48), stage, characterControl, effectControl, gameDevice)
        {
            this.speedX = speedX;   //初期速度を保存する
            speed.X = speedX;       //速度初期化
            speed.Y = 0;
            this.position = position;
            this.direction = direction;  //敵の向きを初期化
            this.pieceControl = pieceControl;   //分割された画像をオブジェクトとして保存して管理する
            mode = ActionMode.JUMP;
            timer = new Timer(0.2f);
            shootTimer = new Timer(0.4f);
            damegePosition = new List<Vector2>();
            damegeTimer = new Timer(0.4f);  //ダメージを受けた時、damege状態の保持時間

            Initialize();
        }

        public override void Initialize() {
            linkTime = 0;      //link攻撃受けた時間を初期化
            shooting = 0;
            hp = 10;
            isDead = false;

            //敵キャラの絵を分割して保存
            motion = new Motion();
            for (int i = 0; i < 16; i++) {
                motion.Add(i, new Rectangle((int)size.X * (i % 4), (int)size.Y * (int)(i / 4), (int)size.X, (int)size.Y));
            }
        }

        public override void Update() {
            shooting++;

            IsDeath();
            Move();
            FallNoBlock();      //落下する時、下は床があるかどうかをチェックしてから処理
            LinkUpdate();       //link攻撃受けたらの処理
            DamegeUpdate();     //ダメージ受けた時の処理
            LaserUpdate();      //レーザー攻撃を受けたらの処理
            Shoot();
            ShootUpdate();

            ReverseNoBlock();   //進んでる方向は床がなかったら向きを変える
            ReverseWall();      //進んでる方向は壁があったら向きを変える

            modeState.Update(mode);     //敵の状態の流れをチェック
            UpdateEffect();             //敵にかかわるエフェクトの描画チェンジ
            UpdateMotion();             //敵のアニメーション描画チェンジ
            motion.Update();            //アニメーションを動かせる
        }


        /// <summary>
        /// ダメージ状態になると、動けないけど、無敵
        /// </summary>
        private void DamegeUpdate() {
            //今描画しているダメージの数字の座標を更新する（上に浮く）
            for (int i = 0; i < damegePosition.Count; i++) { damegePosition[i] += new Vector2(0, -1); }
            if (mode == ActionMode.DAMEGE) {
                speed.X = 0;
                damegeTimer.Update();
                Flash();        //点滅する

                if (damegeTimer.IsTime) {
                    damegeTimer.Initialize();
                    mode = ActionMode.STAND;
                    speed.X = speedX;
                    alpha = 1.0f;
                    damegePosition.Clear();     //ダメージ状態が終わると、ダメージの数字も消える
                }
            }
        }

        /// <summary>
        /// 通常攻撃を受けた時の処理
        /// </summary>
        /// <param name="other">攻撃しに来るキャラ</param>
        public override void Hit(Character other) {
            if (other is Bullet) { return; }    //チャラは敵側の弾のとき、当たらない
            hp -= other.GetAttack;
            damege = other.GetAttack;
            damegePosition.Add(stage.GetScreenPosition(position));      //ダメージ数値を表示する座標を保存

            //beamから攻撃を受けた場合、相応なエフェクトを生成
            if (other is Beam) {
                effectControl.AddEffect(new Effect(EffectMode.BEAMATTACK, position - size / 4, new Timer(0.15f), characterControl));
            }

            if (hp > 0) { mode = ActionMode.DAMEGE; }
            else { 
                mode = ActionMode.DEATH;
                sound.PlaySE("enemyDead");
                //攻撃受けた方向によって敵を飛ばす
                if (other.GetDirection != direction) {
                    speed.X *= -1;
                }
                speed.Y -= 16;
            }
        }

        public void Laser(int otherAttack) {
            mode = ActionMode.LASERSHOOT;

            //もし、レーザーに当たったら、音を流す
            if (modeState.LaserShootStart()) { sound.PlaySE("laserburn"); }
            motion.Initialize(new Range(4, 4), timer);     //アニメーションを描画は状態に合わせて変える
            damege = otherAttack;
            speed.X = 0;
        }

        private void LaserUpdate() {
            if (modeState.LaserEnd()) {
                hp -= damege;
                if (hp <= 0) {
                    sound.PlaySE("enemyDead");
                    mode = ActionMode.DEATH;
                    speed.Y -= 16;
                }
                else { mode = ActionMode.DAMEGE; }
                //ダメージの数値を描画する座標を登録する
                damegePosition.Add(stage.GetScreenPosition(position));
            }
        }

        private void UpdateEffect() {
            //レーザーが始まるとエフェクトを追加する
            if (modeState.LaserShootStart()) {
                effectControl.AddEffect(new Effect(EffectMode.LASERBURN, position - size / 4, new Timer(3.0f), characterControl));
            }

            //レーサーが中止されるとエフェクトを削除する
            if (modeState.LaserEnd()) {
                effectControl.LaserEnd();
            }
        }

        public void Link() {
            mode = ActionMode.LINKSTART;
            speed.X = 0;
            motion.Initialize(new Range(4, 4), timer);
        }

        private void LinkUpdate() {
            if (mode == ActionMode.LINKSTART) {
                linkTime++;

                //linkされる時間が1秒が経つと、直接死ぬ
                if (linkTime >= 60) {
                    mode = ActionMode.DEATH;
                    speed.Y -= 16;
                    damege = hp;    //ダメージ数値を保存する
                    hp -= hp;
                    sound.PlaySE("enemyDead");

                    //ダメージ数値の描画座標を登録する
                    damegePosition.Add(stage.GetScreenPosition(position));
                    linkTime = 0;
                }
            }

            //linkが中止されると、速度とlinkされる時間を初期化する
            if (modeState.LinkEnd()) {
                speed.X = speedX;
                linkTime = 0;
            }
        }

        public override void Hit() {
            if (hp > 0) { mode = ActionMode.DAMEGE; }
            else {
                mode = ActionMode.DEATH;
                speed.X *= -1;
                speed.Y -= 16;
            }    
        }

        private void IsDeath() {
            //画面外に落ちると死ぬ
            if (!InMap(position)) {
                isDead = true;
                damegePosition.Clear();
            }
        }

        private void UpdateMotion() {
            if (mode == ActionMode.STAND) { motion.Initialize(new Range(0, 0), timer); }
            if (mode == ActionMode.JUMP) { motion.Initialize(new Range(0, 0), timer); }
            if (speed.X != 0 && mode != ActionMode.MOVE && mode != ActionMode.DEATH) { 
                mode = ActionMode.MOVE;  
                motion.Initialize(new Range(1, 2), new Timer(0.3f)); 
            }
            if (mode == ActionMode.DAMEGE || mode == ActionMode.DEATH) {
                motion.Initialize(new Range(3, 3), timer);
            }
            if (mode == ActionMode.LINKSTART) { motion.Initialize(new Range(4, 4), timer); }
        }


        #region 攻撃

        private void Shoot() {
            if (mode == ActionMode.DAMEGE || mode == ActionMode.DEATH || mode == ActionMode.LASERSHOOT) { return; }
            if (shooting % 90 == 0) {
                speed.X = 0;
                mode = ActionMode.SKILLSTART;
            }
        }

        private void ShootLeft() {
            characterControl.AddCharacter(new Bullet("bullet",stage, position - new Vector2(size.X / 2, 0), direction, characterControl, effectControl, pieceControl, gameDevice));
        }

        private void ShootRight() {
            characterControl.AddCharacter(new Bullet("bullet",stage, position + new Vector2(size.X / 2, 0), direction, characterControl, effectControl, pieceControl, gameDevice));
        }

        private void ShootUpdate() {
            if (mode == ActionMode.SKILLSTART) {
                shootTimer.Update();
                timer.Update();
                if (timer.IsTime) {
                    mode = ActionMode.SKILLSHOOT;
                    if (direction == Direction.LEFT) { ShootLeft(); }
                    else { ShootRight(); }
                    timer.Initialize();
                }
            }

            if (mode == ActionMode.SKILLSHOOT) {
                shootTimer.Update();
                if (shootTimer.IsTime) { 
                    mode = ActionMode.STAND;
                    speed.X = speedX;
                    shootTimer.Initialize();
                }
            }
        }

        #endregion

        #region マープ認識

        private void ReverseWall() {
            if (mode == ActionMode.DEATH) { return; }
            if (direction == Direction.RIGHT) {
                Vector2 startPosition = position + new Vector2(collisionSize.X / 2, 0);
                if (!stage.CollisitionSide(startPosition, 1)) {
                    direction = Direction.LEFT;
                }
            }
            else {
                Vector2 startPosition = position - new Vector2(collisionSize.X / 2, 0);
                if (!stage.CollisitionSide(startPosition, 1)) {
                    direction = Direction.RIGHT;
                }
            }
        }

        private void ReverseNoBlock() {
            if (mode == ActionMode.DEATH) { return; }
            if (direction == Direction.RIGHT) {
                Vector2 startPosition = position + new Vector2(collisionSize.X / 2 + 1, collisionSize.Y / 2);
                if (stage.CollisitionUpDown(startPosition, 1)) {
                    direction = Direction.LEFT;
                }
            }
            else {
                Vector2 startPosition = position - new Vector2(collisionSize.X / 2 + 1, -collisionSize.Y / 2);
                if (stage.CollisitionUpDown(startPosition, 1)) {
                    direction = Direction.RIGHT;
                }
            }
        }

        private void FallNoBlock() {
            if (mode == ActionMode.DEATH) { return; }
            for (int i = 0; i < 10; i++) {
                Vector2 startPosition = position - new Vector2(collisionSize.X / 2, -collisionSize.Y / 2);
                if (stage.CollisitionUpDown(startPosition, collisionSize.X)) {
                    mode = ActionMode.JUMP;
                    position.Y++;
                }
                else {
                    if (mode == ActionMode.JUMP) {
                        mode = ActionMode.MOVE;
                        timer.Initialize();
                    }
                }
            }        
        }

        #endregion

        #region 動き
        
        private void Move() {
            if (mode == ActionMode.JUMP) { return; }
            
            if (direction == Direction.RIGHT) { MoveRight();  }
            else { MoveLeft();  }
            if (mode == ActionMode.DEATH) {
                speed.Y += 0.5f;
                position.Y += speed.Y;
            }
        }

        private void MoveRight() { position.X += speed.X; }
        private void MoveLeft() { position.X -= speed.X; }

        #endregion

        public override void Draw(Renderer renderer) {
            if (direction == Direction.LEFT) { renderer.DrawTexture(name, stage.GetScreenPosition(position - size / 2), motion.DrawRange_L(), alpha); }
            if (direction == Direction.RIGHT) { renderer.DrawTexture(name + "2", stage.GetScreenPosition(position - size / 2), motion.DrawRange_R(), alpha); }

            //ダメージの数値を描画する
            //foreach (var d in damegePosition) {
            //    renderer.DrawNumber("number", d, damege);
            //}
        }


    }
}
