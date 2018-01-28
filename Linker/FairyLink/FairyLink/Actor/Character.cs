using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Device;
using FairyLink.Scene;
using FairyLink.Utility;
using FairyLink.Def;

namespace FairyLink.Actor
{
    abstract class Character
    {
        protected string name;
        protected Vector2 position;     //中心座標
        protected Vector2 size;
        protected Vector2 collisionSize;        //当たり範囲のサイズ
        protected int hp;
        protected Direction direction;
        protected Vector2 speed;
        protected bool isDead;
        protected bool damegeState;
        protected float alpha;          //透明度
        protected int attack;

        protected ActionMode mode;          //キャラのアニメーション管理用
        protected EffectMode effectMode;        //エフェクトのモードが登録されているenum

        protected Stage stage;
        protected Timer timer;          //通用Timer
        protected Timer damegeTimer;    //ダーメジ状態の保持時間
        protected Timer flashTimer;     //点滅頻度管理用

        protected static Random rand = new Random();

        protected ModeState modeState;
        protected CharacterControl characterControl;    //キャラの集中管理用
        protected EffectControl effectControl;          //エフェクトの集中管理用
        protected GameDevice gameDevice;
        protected Sound sound;
        protected InputState inputState;

        public Character(string name, Vector2 size, Vector2 collisionSize, Stage stage, CharacterControl characterControl, EffectControl effectControl, GameDevice gameDevice)
        {
            this.name = name;
            this.size = size;
            this.stage = stage;
            this.collisionSize = collisionSize;
            alpha = 1;
            isDead = false;

            this.gameDevice = gameDevice;
            sound = gameDevice.GetSound;
            inputState = gameDevice.GetInputState;

            timer = new Timer(0.2f);
            flashTimer = new Timer(0.2f);
            damegeTimer = new Timer(1.0f);            
            modeState = new ModeState();

            this.characterControl = characterControl;
            this.effectControl = effectControl;
        }

        public virtual void Initialize() { }
        public abstract void Update();
        public abstract void Hit(Character other);
        
        public abstract void Hit();

        //キャラを点滅させる
        protected void Flash() {
            flashTimer.Update();
            if(flashTimer.IsTime) {
                if (alpha == 1.0f) { alpha = 0.5f; flashTimer.Initialize(); }
                else { alpha = 1.0f; flashTimer.Initialize(); }
            }
        }

        public bool LinkDead() {
            if (modeState.LinkDead()) { speed.X = 0; return true; }
            return false; 
        }

        public virtual void Draw(Renderer renderer) {
            renderer.DrawTexture(name, stage.GetScreenPosition(position - size / 2));
        }


        #region get&set

        public Vector2 Position {
            get { return position; }
            set { position = value; }
        }

        public ActionMode Mode {
            get { return mode; }
            set { mode = value; }
        }

        //キャラの向きゲット
        public Direction GetDirection {
            get { return direction; }
        }

        public Vector2 GetSpeed {
            get { return speed; }
        }

        public bool IsDead {
            get { return isDead; }
        }

        public int GetAttack {
            get { return attack; }
        }

        #endregion

        /// <summary>
        /// Map上かどうかチェックする
        /// </summary>
        /// <param name="position">キャラの座標</param>
        /// <returns></returns>
        protected bool InMap(Vector2 position) {
            int bx,by;
            bx = (int)(position.X / StageMap.BlockSize);
            by = (int)(position.Y / StageMap.BlockSize);

            if (by > StageMap.YMax || bx > StageMap.XMax) { return false; }
            else { return true; }
        }


        /// <summary>
        /// キャラは今映ってる画面の中にいるかどうかチェックする（playerの位置を基準として）
        /// </summary>
        /// <param name="playerPosition">playerの座標</param>
        /// <returns></returns>
        public bool InScreen(Vector2 playerPosition) {
            int px = (int)stage.GetScreenX(playerPosition.X);

            if ((stage.GetScreenX(position.X) >= px - 900) && stage.GetScreenX(position.X) <= px + 900) { return true; }
            else { return false; }
        }


        #region あたり判定

        /// <summary>
        /// リンク攻撃とのあたり判定（円と円のあたり判定）
        /// </summary>
        /// <param name="other">敵の方</param>
        /// <returns></returns>
        public bool IsLinkCollision(Character other) { 
            if (other.mode == ActionMode.DEATH) { return false; }       //敵が死んだらチェックしない

            //playerはリンク状態じゃないとチャックしない
            if (mode != ActionMode.LINKSTART && mode != ActionMode.LINKSHOOT) { return false; }

            //playerの向きによってチェック範囲の中心を決める
            Vector2 linkPosition = position + size / 2;
            if (direction == Direction.RIGHT) { linkPosition.X += 1.5f * size.X; }
            else { linkPosition.X -= 1.5f * size.X; }

            //敵の中心座標を計算して出す
            Vector2 otherCenter = other.position + other.size / 2;

            //当たる範囲を計算する
            float distance = (float)Math.Sqrt((linkPosition.X - otherCenter.X) * (linkPosition.X - otherCenter.X) +
                ((linkPosition.Y - otherCenter.Y) * (linkPosition.Y - otherCenter.Y)));

            //当たる範囲入ってるかどうかチェックする
            if (distance < other.collisionSize.X + StageMap.BlockSize) { return true; }
            else { return false; }
        }

        /// <summary>
        /// レーザー攻撃とのあたり判定（四角いあたり判定）
        /// </summary>
        /// <param name="other">敵の方</param>
        /// <returns></returns>
        public bool IsLaserCollision(Character other) {
            if (other.mode == ActionMode.DEATH) { return false; }       //敵が死んだらチェックしない

            //playerはLASERSHOOT状態じゃないとチャックしない
            if (mode != ActionMode.LASERSHOOT) { return false; }
            if (direction == Direction.RIGHT) {         //playerの向きは右の場合

                //敵座標とplayer座標の距離から計算して、当たり範囲の中に入ってるかどうかがチェック
                //レーザの長さはplayerから、スクリーンの枠まで
                if (other.Position.X - position.X <= Screen.Width - stage.GetScreenX(position.X) //敵とplayerの距離≦playerとスクリーンの枠の距離
                    && other.Position.X - position.X > 0     //敵はplayerの右側にいる
                    && Math.Abs(position.Y - other.Position.Y) <= size.Y) {      //敵縦方向の座標はレーザーに当たられる範囲の中
                    return true;
                }
            }
            else {
                if (position.X - other.Position.X <= stage.GetScreenX(position.X)
                    &&position.X - other.Position.X > 0          //敵はplayerの左側にいる
                    && Math.Abs(position.Y - other.Position.Y) <= size.Y) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 攻撃のあたり判定（円と円のあたり判定）
        /// </summary>
        /// <param name="other">敵の方</param>
        /// <returns></returns>
        public bool IsAttackCollision(Character other) { 

            //敵はダメージと死亡状態の場合、チェックしない
            if (other.mode == ActionMode.DAMEGE || other.mode == ActionMode.DEATH) { return false; }

            //playerは攻撃状態じゃないと、チェックしない
            if (mode != ActionMode.NOMALATTACK && mode != ActionMode.JUMPATTACK) { return false; }

            //向きによってplayerの攻撃当たり範囲の中心を決める
            Vector2 attackPosition = position + size / 2;
            if (direction == Direction.RIGHT) { attackPosition.X += size.X / 2; }
            else { attackPosition.X -= size.X / 2; }

            //敵キャラ座標の中心を計算する
            Vector2 otherCenter = other.position + other.size / 2;

            //攻撃中心と敵の中心点の距離を計算して当たったかどうかチェック
            float distance = (float)Math.Sqrt((attackPosition.X - otherCenter.X) * (attackPosition.X - otherCenter.X) +
                ((attackPosition.Y - otherCenter.Y) * (attackPosition.Y - otherCenter.Y)));
            if (distance < other.collisionSize.X + 15 ) {
                other.effectControl.AddEffect(new Effect(EffectMode.WALLJUMP, other.position - other.size / 2, new Timer(0.2f), characterControl));
                return true; 
            }
            else { return false; }
        }

        /// <summary>
        /// キャラとのあたり判定（円と円のあたり判定）
        /// </summary>
        /// <param name="other">敵の方</param>
        /// <returns></returns>
        public bool IsCollision(Character other) { 

            //player或いは敵はダメージ、死亡状態の場合チェックしない
            if (other.mode == ActionMode.DAMEGE || other.mode == ActionMode.DEATH || 
                damegeState || mode == ActionMode.DEATH ) {
                return false;
            }

            //playerと敵ｘ座標とｙ座標の差を計算する
            float xLenghth = position.X - other.position.X;
            float yLenghth = position.Y - other.position.Y;

            //実際の距離の二乗と当たる距離の二乗を計算して、チェック
            float squareLength = (xLenghth * xLenghth + yLenghth * yLenghth);
            float squareRadius = (collisionSize.X + other.collisionSize.X) * (collisionSize.X + other.collisionSize.X) / 4;
            if (squareLength <= squareRadius) { return true;  }
            return false;
        }

        /// <summary>
        /// 弾とのあたり判定（円と四角いのあたり判定）
        /// </summary>
        /// <param name="other">敵の方</param>
        /// <returns></returns>
        public bool IsCollisionBullet(Character other) {
            
            //player或いは敵はダメージ、死亡状態の場合チェックしない
            if (other.mode == ActionMode.DAMEGE || other.mode == ActionMode.DEATH ||
                damegeState || mode == ActionMode.DEATH) {
                return false;
            }

            //playerと敵ｘ座標とｙ座標の距離を計算する
            float xLenghth = Math.Abs(position.X - other.position.X);
            float yLenghth = Math.Abs(position.Y - other.position.Y);

            //距離と自分のあたりサイズを比べてチェック
            if (xLenghth <= collisionSize.X / 2) {
                if (yLenghth <= collisionSize.Y / 2) {
                    return true;
                }
            }
            return false;
        }

        #endregion

    }
}
