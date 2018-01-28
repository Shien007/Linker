using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FairyLink.Scene;
using FairyLink.Device;
using FairyLink.Utility;
using FairyLink.Def;
using FairyLink.Actor.ModeDisign;

namespace FairyLink.Actor
{
    class Player : Character
    {
        private Gimmick gimmick;        //Map上移動しないギミックを管理するクラス（ゴール、チェックポイントなど）
        private Vector2 memoryPosition;     //取ったチェックポイントの座標を記録する用
        private int memoryStageX;           //取ったチェックポイントのスクロール距離を記録する用
        private int sp;
        private Motion motion;          //アニメーションを管理するクラス
        private PlayerAttackAI playerAttackAI;      //攻撃をまとめて管理するクラス
        private FireworksControl fireworksControl;
        private int deathCount;         //死亡回数をカウントする用
        private bool getOnState;        //moveBlockに乗る
        private bool doubleJump;
        

        //Timer
        private Timer wallJumpTimer;    //walljumpを起動した瞬間、壁を掴んで止まる時間
        private int dartle;         //beamを生成する間隔を管理する

        public Player(Stage stage, Gimmick gimmick, CharacterControl characterControl, EffectControl effectControl, GameDevice gameDevice)
            : base("player", new Vector2(128, 128), new Vector2(52, 128), stage, characterControl, effectControl, gameDevice)
        {
            this.gimmick = gimmick;
            effectControl = new EffectControl(stage);
            fireworksControl = new FireworksControl(stage, this);
            wallJumpTimer = new Timer(0.2f);
            damegeTimer = new Timer(2.0f);
            Initialize();
        }

        public override void Initialize() {
            deathCount = 0;
            attack = 5;
            dartle = 0;
            hp = 3;
            sp = 1;
            alpha = 1.0f;   //透明度
            isDead = false;
            getOnState = false;
            damegeState = false;
            doubleJump = false;
            position = new Vector2(130, 450);

            memoryPosition = Vector2.Zero;
            memoryStageX = 0;

            //playerの画像をアニメーション描画に合わせて分割して保存
            motion = new Motion();
            for (int i = 0; i < 20; i++) {
                motion.Add(i, new Rectangle((int)size.X * (i % 4), (int)size.Y * (int)(i / 4), (int)size.X, (int)size.Y));
            }

            mode = ActionMode.JUMP;
            direction = Direction.RIGHT;

            playerAttackAI = new PlayerAttackAI(motion, this, inputState);
            playerAttackAI.Initialize();
        }

        public override void Update() {
            MemoryPoint();          //チェックポイント取ったかどうかチェックして、チェックポイントの状態と座標を保存
            fireworksControl.Update();

            //動作
            Move();
            WallJumpStart();        //壁ジャンプ始める
            WallJumpUpdate();       //壁ジャンプ状態の更新
            DoubleJump();
            JumpStart();        //jump始める
            JumpUpdate();       //jump状態の更新
            FallStart();        //落下処理

            DamegeUpdate();     //ダメージ状態の更新
            Shoot();            //beamの攻撃

            //アニメーション
            motion.Update();            //アニメーション描画用クラス更新
            UpdateMotion();             //今playerの動きパタンを更新する
            modeState.Update(mode);     //パタンチェッククラスを更新する
            UpdateEffect();             //エフェクト状態を更新する

            //攻撃
            playerAttackAI.Update();    //攻撃状態の更新

            IsDeath();
            GimmickStart();
        }

        public override void Hit(){
            if (mode == ActionMode.DEATH || damegeState) {
                return;
            }

            if (gimmick.IsFire(position)) {
                sound.PlaySE("fire");
            }
            hp--;
            if (hp > 0) {
                speed.X = 0;
                damegeState = true;
                sound.PlaySE("playerDamege");
                mode = ActionMode.STAND;
                //ダメージ受けたら後退する、ただし、後退する隙間がなかったら、後退しない
                if (direction == Direction.LEFT) {
                    //後退する方向、壁があるかどうかチェックして、後退できるかどうか判断して動く
                    if (stage.CollisitionSide(position + new Vector2(StageMap.BlockSize + collisionSize.X / 2, -collisionSize.Y / 2), collisionSize.Y))
                    { position.X += StageMap.BlockSize; }
                }
                else {
                    //後退する方向、壁があるかどうかチェックして、後退できるかどうか判断して動く
                    if (stage.CollisitionSide(position - new Vector2(StageMap.BlockSize + collisionSize.X / 2, collisionSize.Y / 2), collisionSize.Y))
                    { position.X -= StageMap.BlockSize; }
                }
            }
            else {
                mode = ActionMode.DEATH;
                sound.PlaySE("playerDead");
                deathCount++;
                motion.Initialize(new Range(8, 8), timer);
                speed.X = 0;
                speed.Y = -16;
            }
        }

        public override void Hit(Character character) { }

        private void MemoryPoint() {
            //もし、今player所在のチェックポイントまだ取ってない場合
            if (gimmick.IsCrystal(position) && memoryPosition != new Vector2(((int)position.X / StageMap.BlockSize + 0.5f) * StageMap.BlockSize, position.Y)) {
                //音を流して、チェックポイントの座標とスクロール距離を記録する
                sound.PlaySE("checkpoint");
                memoryPosition = new Vector2(((int)position.X / StageMap.BlockSize + 0.5f) * StageMap.BlockSize, position.Y);
                memoryStageX = stage.StageX;
            }
        }

        private void GimmickStart() {
            if (mode == ActionMode.DEATH) { return; }

            gimmick.IsGravel(position);
           
            IsJewel();
            IsMoveBlock();
            if (gimmick.IsFire(position)) {
                Hit();
            }
            if (gimmick.IsFireBall(position, collisionSize)) {
                Hit();
            }
            gimmick.IsSwitch(position);
        }

        private void IsJewel() {
            if (gimmick.IsJewel(position)) {
                sound.PlaySE("itemGet");
                hp = 3;
                sp = 3;
            }
        }

        private void IsMoveBlock() {
            Vector2 checkPosition = position + new Vector2(-collisionSize.X / 2, collisionSize.Y / 2);
            Vector2 moveBlockSpeed = gimmick.IsMoveBlock(checkPosition, (int)collisionSize.X / 2, this);
            if (moveBlockSpeed.Y == 0) {
                getOnState = false;
                return;
            }
            if (moveBlockSpeed.Y == 0.1f)
            {
                speed.Y = 0;
            }
            else {
                speed.Y = moveBlockSpeed.Y;
            }
            position += speed;
            if (getOnState) { return; }
            getOnState = true;
            doubleJump = false;
            mode = ActionMode.STAND;

            //エフェクトを生成する
            effectControl.AddEffect(new Effect(EffectMode.LANDED, position - new Vector2(size.X / 4, 0), new Timer(0.3f), characterControl));
            timer.Initialize();
        }


        private void UpdateMotion() {
            //ジャンプ始まる瞬間、エフェクト状態を変換する
            if (modeState.ShootStart()) {
                effectMode = EffectMode.SKILL;
            }
            //壁ジャンプする瞬間、音を流す
            if (modeState.WallJump()) {
                sound.PlaySE("jump");
            }
            //シュート異常中止の場合、頻度管理タイマを初期化
            if (modeState.ShootEnd()) {
                dartle = 0;
            }


            if (mode == ActionMode.STAND) {
                motion.Initialize(new Range(4, 4), timer);
            }
            else if (mode == ActionMode.JUMP) {
                motion.Initialize(new Range(5, 5), timer);
            }
            else if (mode == ActionMode.WALLJUMPSTART) {
                motion.Initialize(new Range(16, 16), timer);
            }
            else if (mode == ActionMode.WALLJUMP) {
                motion.Initialize(new Range(17, 17), timer);
            }
            else if (mode == ActionMode.LASERSHOOT) {
                stage.Tremble();    //画面を震える処理を始める
                Vector2 trembleposition = stage.GetTrember; //震える距離をとる
                stage.StageX = stage.StageX + (int)trembleposition.X;   //画面をずらす
            }
        }


        /// <summary>
        /// エフェクトパタンを更新する
        /// </summary>
        private void UpdateEffect() {

            //攻撃が始まる
            if (modeState.AttackStarted()) {
                sound.PlaySE("playerAttack");     //音を流す

                //向きに合わせてエフェクトを描画する座標を計算する
                Vector2 attackPosition = position - size / 4;
                if (direction == Direction.RIGHT) { attackPosition.X += size.X / 2; }
                else { attackPosition.X -= size.X / 2; }

                //エフェクトを生成する
                effectControl.AddEffect(new Effect(EffectMode.ATTACK, attackPosition, new Timer(0.4f), characterControl));
            }

            //link攻撃が始まる
            else if (modeState.LinkStarted()) {
                //向きに合わせてエフェクトを描画する座標を計算する
                Vector2 linkPosition = position - size;
                if (direction == Direction.RIGHT) { linkPosition.X += 1.5f * size.X; }
                else { linkPosition.X -= 1.5f * size.X; }

                //エフェクトを生成する
                effectControl.AddEffect(new Effect(EffectMode.LINK, linkPosition, new Timer(3.4f), characterControl));
            }

            //laser攻撃が始まる
            else if (modeState.LaserStarted()) {
                //向きに合わせてエフェクトを描画する座標を計算する
                Vector2 laserPosition;
                if (direction == Direction.RIGHT) { 
                    laserPosition = position - size + new Vector2(size.X / 2, 0); 
                }
                else {
                    laserPosition = position - size * 2 + new Vector2(size.X / 2, size.Y); 
                }
                //エフェクトを生成する
                effectControl.AddEffect(new Effect(EffectMode.LASER, laserPosition, new Timer(3.4f), characterControl));
            }

            //link状態が終わると、linkのエフェクトを削除する
            if (modeState.LinkEnd()) {
                effectControl.LinkEnd();
            }
            //laser状態が終わると、laserのエフェクトを削除する
            else if (modeState.LaserEnd()) {
                stage.TremberInitialize();  //震える距離を初期化する（画面は震え始めた前の状態に戻る）
                effectControl.LaserEnd();
            }

        }

        private void IsDeath() {
            //画面外に落ちると
            if (!InMap(position)) {
                //まだ生きてる場合
                if (hp > 1) {
                    //map上のｘ座標はマップチップの座標に変換する
                    int x = (int)position.X / StageMap.BlockSize;
                    
                    //今の位置の一個左の下側から上側まで計算する
                    x--;
                    stage.StageX -= StageMap.BlockSize;
                    for (int y = StageMap.YMax - 1; y > 0; y--) {
                        //もし足場が探したらplayerは探した足場に置く、処理から抜ける
                        if (stage.IsBlock(x, y) && !stage.IsBlock(x, y - 1)) {
                            position = new Vector2((x + 0.5f) * StageMap.BlockSize, (y - 1) * StageMap.BlockSize);
                            break;
                        }
                        //探さなかったら、もう一個左の下側からまた探す
                        if (y - 1 == 0) {
                            x--;
                            stage.StageX -= StageMap.BlockSize;
                            y = StageMap.YMax;
                        }
                    }
                    hp--;
                    damegeState = true;
                    sound.PlaySE("playerDamege");
                }

                //もし死んだら
                else {
                    //チェックポイント取ってない場合、死ぬ
                    if (memoryPosition == Vector2.Zero) {
                        if (isDead) return;
                        isDead = true;
                        if (mode == ActionMode.DEATH) return;
                        deathCount++;
                        sound.PlaySE("playerDead");
                    }

                    //チェックポイント取ったら、チェックポイントに戻る、状態初期化する
                    else {
                        if (mode != ActionMode.DEATH) {
                            sound.PlaySE("playerDead");
                        }
                        deathCount++;
                        hp = 3;
                        sp = 1;
                        alpha = 1.0f;
                        speed = Vector2.Zero;
                        position = memoryPosition;
                        stage.StageX = memoryStageX;
                        mode = ActionMode.STAND;
                    }
                }
            }
        }

        public bool IsGameClear() {
            //今の座標はゴールかどうかチェックして結果出す
            if (gimmick.IsDoor(position)) { return true; }
            return false;
        }

        public int Sp {
            get { return sp; }
            set { sp = value; }
        }

        public int DeathCount {
            get { return deathCount; }
        }

        public void SpAbsorb() {
            if (sp >= 3) return;
            sp++;
            fireworksControl.SpAbsorb(stage, this);
        }

        private void DamegeUpdate() {
            if (damegeState) {
                damegeTimer.Update();
                Flash();    //点滅する

                if (damegeTimer.IsTime) {
                    damegeTimer.Initialize();
                    alpha = 1.0f;
                    damegeState = false;
                }
            }
        }

        #region 歩く

        private void Move() {
            if (mode == ActionMode.STAND || mode == ActionMode.MOVE || mode == ActionMode.JUMP || mode == ActionMode.JUMPATTACK) {
                if (inputState.IsDown(Keys.Right) || 
                    inputState.IsDown(Buttons.DPadRight) ||
                    inputState.IsDown(Buttons.LeftThumbstickRight)) {
                    direction = Direction.RIGHT;
                    MoveRight();
                }
                if (inputState.IsDown(Keys.Left) || 
                    inputState.IsDown(Buttons.DPadLeft) || 
                    inputState.IsDown(Buttons.LeftThumbstickLeft)) {
                    direction = Direction.LEFT;
                    MoveLeft();
                }

                if (inputState.IsDown(Keys.Right) ||
                    inputState.IsDown(Keys.Left) || 
                    inputState.IsDown(Buttons.DPadRight) || 
                     inputState.IsDown(Buttons.DPadLeft) ||
                     inputState.IsDown(Buttons.LeftThumbstickRight) ||
                     inputState.IsDown(Buttons.LeftThumbstickLeft)) {
                    if (mode == ActionMode.STAND) {
                        mode = ActionMode.MOVE;
                        motion.Initialize(new Range(0, 3), timer);
                    }
                }
                else {
                    if (mode == ActionMode.MOVE) {
                        mode = ActionMode.STAND;
                    }
                }
            }
        }

        private void MoveRight() {
            for (int i = 0; i < 4; i++) {
                MoveRightOne();
            }
        }

        private void MoveLeft() {
            for (int i = 0; i < 4; i++) {
                MoveLeftOne();
            }
        }

        private void MoveRightOne() {
            //チェックはじめの座標を決める
            Vector2 startPosition = position + new Vector2(collisionSize.X / 2 + 1, -collisionSize.Y / 2);
            //壁がないと
            if (stage.CollisitionSide(startPosition, collisionSize.Y) &&
                gimmick.CollisitionSide(startPosition, collisionSize.Y) &&
                !gimmick.IsSwitchDoor(startPosition))
                {
                position.X++;   //右に進む

                //ｘ座標は400より大きくなると
                if (stage.GetScreenX(position.X) > 400) {
                    stage.ScrollLeft();     //左方向のスクロールが始まる
                }
            }
        }

        private void MoveLeftOne() {
            //チェックはじめの座標を決める
            Vector2 startPosition = position - new Vector2(collisionSize.X / 2 + 1, collisionSize.Y / 2);
            //壁がないと
            if (stage.CollisitionSide(startPosition, collisionSize.Y) &&
                gimmick.CollisitionSide(startPosition, collisionSize.Y) &&
                !gimmick.IsSwitchDoor(startPosition)
                ) {
                position.X--;   //左に進む

                //ｘ座標はマップの右側から600ピクセル離れると
                if (stage.GetScreenX(position.X) < 600) {
                    stage.ScrollRight();        //右方向のスクロールが始まる
                }
            }
        }

        #endregion

        #region ジャンプ

        private void FallStart() {
            if (getOnState) { return; }
            //ジャンプ中、処理やらない
            if (mode == ActionMode.JUMP || mode == ActionMode.JUMPATTACK 
                || mode == ActionMode.WALLJUMP || mode == ActionMode.WALLJUMPSTART) { return; }
            if (mode == ActionMode.DEATH) {
                if (direction == Direction.LEFT) { speed.X += 0.1f; }
                else { speed.X -= 0.1f; }
                speed.Y += 0.5f; 
                position += speed; 
            }
            else{
                //チェック始める座標を決める
                Vector2 startPosition = position + new Vector2(-collisionSize.X / 2, collisionSize.Y / 2);
                
                //下に足場がない、縦方向の速度を初期化する
                if (stage.CollisitionUpDown(startPosition, collisionSize.X)
                    && gimmick.CollisitionUpDown(startPosition, collisionSize.X)
                    ) {
                    speed.Y = 0;
                    mode = ActionMode.JUMP;
                }
            }
        }

        private void JumpStart() {
            if (inputState.WasDown(Keys.Z) || inputState.WasDown(Buttons.A)) {
                if (mode == ActionMode.JUMP || mode == ActionMode.JUMPATTACK
                    || mode == ActionMode.DEATH
                    || mode == ActionMode.LASERSTART || mode  == ActionMode.LASERSHOOT 
                    || mode == ActionMode.WALLJUMP || mode == ActionMode.WALLJUMPSTART) {
                    return;
                }
                sound.PlaySE("jump");
                speed.Y = -12;
                mode = ActionMode.JUMP;
                effectControl.AddEffect(new Effect(EffectMode.JUMP, position - new Vector2(size.X / 4, 0), new Timer(0.3f), characterControl));
            }
        }

        private void DoubleJump() {
            if (mode == ActionMode.JUMP && !doubleJump) {
                if (inputState.WasDown(Keys.Z) || inputState.WasDown(Buttons.A)) {
                    speed.Y = -12;
                    effectControl.AddEffect(new Effect(EffectMode.JUMP, position - new Vector2(size.X / 4, 0), new Timer(0.3f), characterControl));
                    doubleJump = true;
                    sound.PlaySE("jump");
                }
            }
        }


        private void WallJumpStart() {
            if (mode == ActionMode.STAND || mode == ActionMode.MOVE) { return; }

            //ジャンプ中だけ、壁ジャンプ起動できる
            if ((mode == ActionMode.JUMP || mode == ActionMode.WALLJUMP)) {
                Vector2 startPosition;

                //右向きの場合、右側は壁だったら壁ジャンプ起動、向きを逆転
                if (direction == Direction.RIGHT) {
                    startPosition = position + new Vector2(collisionSize.X / 2 + 1, -collisionSize.Y / 2);
                    if (!stage.CollisitionSide(startPosition, collisionSize.Y)) {
                        if ((inputState.IsDown(Keys.Right) && inputState.WasDown(Keys.Z)) ||
                            ((inputState.IsDown(Buttons.DPadRight) ||
                            (inputState.IsDown(Buttons.LeftThumbstickRight))
                            && inputState.WasDown(Buttons.A))))
                        {
                            mode = ActionMode.WALLJUMPSTART;
                            direction = Direction.LEFT;
                            sound.PlaySE("jump");
                        }   
                    }
                }
                //左向きの場合、左側は壁だったら壁ジャンプ起動、向きを逆転
                else { 
                    startPosition = position - new Vector2(collisionSize.X / 2 + 1, collisionSize.Y / 2);
                    if (!stage.CollisitionSide(startPosition, collisionSize.Y)) {
                        if ((inputState.IsDown(Keys.Left) && inputState.WasDown(Keys.Z))||
                            ((inputState.IsDown(Buttons.DPadLeft)||
                            (inputState.IsDown(Buttons.LeftThumbstickLeft)) 
                            && inputState.WasDown(Buttons.A))))
                        {
                            
                            mode = ActionMode.WALLJUMPSTART;
                            direction = Direction.RIGHT;
                            sound.PlaySE("jump");
                        }
                    }
                }
            }
        }

        private void WallJumpUpdate() {
            //壁を掴んでる状態だったら、更新する
            if (mode == ActionMode.WALLJUMPSTART) {
                speed.Y = 0;        //いったん止まって、壁を掴む
                wallJumpTimer.Update();

                //掴む状態の時間終わると壁の反対側に飛ぶ
                if (wallJumpTimer.IsTime) {
                    mode = ActionMode.WALLJUMP;
                    speed.Y = -12;
                    wallJumpTimer.Initialize();     //掴む時間初期化
                    if (direction == Direction.RIGHT) {
                        //エフェクトを生成する
                        effectControl.AddEffect(new Effect(EffectMode.WALLJUMP, position - new Vector2(collisionSize.X / 4 + 64, 0), new Timer(0.2f), characterControl));
                    }
                    else {
                        //エフェクトを生成する
                        effectControl.AddEffect(new Effect(EffectMode.WALLJUMP, position + new Vector2(collisionSize.X / 4, 0), new Timer(0.2f), characterControl));
                    }
                }
            }


            //壁ジャンプしている状態だったら、更新する
            if (mode == ActionMode.WALLJUMP) {
                Vector2 startPosition;
                //横方向の速度をかける
                if (direction == Direction.LEFT) {
                    for (int i = 0; i < 5; i++) { MoveLeftOne(); }
                    //左向きの場合、左側は壁だったら、ジャンプモードにチェンジ
                    startPosition = position - new Vector2(collisionSize.X / 2 + 1, collisionSize.Y / 2);
                    if (!stage.CollisitionSide(startPosition, collisionSize.Y)) {
                        mode = ActionMode.JUMP;
                    }
                }
                else {
                    for (int i = 0; i < 5; i++) { MoveRightOne(); }
                    //右向きの場合、右側は壁だったら、ジャンプモードにチェンジ
                    startPosition = position + new Vector2(collisionSize.X / 2 + 1, -collisionSize.Y / 2);
                    if (!stage.CollisitionSide(startPosition, collisionSize.Y)) {
                        mode = ActionMode.JUMP;
                    }
                }
            }
        }

        private void JumpUpdate() {
            if (mode != ActionMode.JUMP && mode != ActionMode.JUMPATTACK 
                && mode != ActionMode.WALLJUMP) {
                return;
            }

            //下方向の加速度をかける
            for (float i = 0; i < Math.Abs(speed.Y); i += 0.5f) {
                if (speed.Y > 0) { MoveDownOne(); }
                else { MoveUpOne(); }
            }
            speed.Y += 0.5f;
        }


        /// <summary>
        /// 下側に動けるかどうかチェックしてから移動
        /// </summary>
        private void MoveDownOne() {
            if (getOnState) { return; }

            //行けると続いて落下する
            Vector2 startPosition = position + new Vector2(-collisionSize.X / 2, collisionSize.Y / 2);
            if (stage.CollisitionUpDown(startPosition, collisionSize.X) &&
                gimmick.CollisitionUpDown(startPosition, collisionSize.X))
            {
                position.Y += 0.5f;
            }

            //行けない場合、状態はStandに移行する
            else {
                doubleJump = false;
                mode = ActionMode.STAND;

                //エフェクトを生成する
                effectControl.AddEffect(new Effect(EffectMode.LANDED, position - new Vector2(size.X / 4, 0), new Timer(0.3f), characterControl));
                timer.Initialize();
            }
        }

        private void MoveUpOne() {
            Vector2 startPosition = position + new Vector2(-collisionSize.X / 2, -collisionSize.Y / 2);

            //上に行ける場合、続いて上に移動する
            if (stage.CollisitionUpDown(startPosition, collisionSize.X) &&
                gimmick.CollisitionUpDown(startPosition, collisionSize.X)) {
                position.Y -= 0.5f;
            }

            //行かない場合、Stand状態に変わる
            else {
                mode = ActionMode.STAND;
                timer.Initialize();
            }
        }

        #endregion

        #region 攻撃

        /// <summary>
        /// シュート状態だったら、更新する
        /// </summary>
        private void Shoot() {
            if (effectMode == EffectMode.SKILL &&  mode == ActionMode.SKILLSHOOT) {
                if (direction == Direction.LEFT) { ShootLeft(); }
                else { ShootRight(); }
            }
        }

        private void ShootLeft() {
            //頻度に合わせて、beamを生成する、三個生成したら、シュート状態から抜く、頻度管理する変数を初期化
            if (dartle > 60) { dartle = 0; effectMode = EffectMode.EMPTY; }
            if (dartle == 1 || dartle == 26 || dartle == 51) {
                //Beamを生成する
                characterControl.AddCharacter(new Beam(stage, position - new Vector2(size.X / 2, 15), direction, characterControl, effectControl, gameDevice));
            }
            dartle++;
        }

        private void ShootRight() {
            //頻度に合わせて、beamを生成する、三個生成したら、シュート状態から抜く、頻度管理する変数を初期化
            if (dartle > 60) { dartle = 0; effectMode = EffectMode.EMPTY; }
            if (dartle == 1 || dartle == 26 || dartle == 51) {
                //Beamを生成する
                characterControl.AddCharacter(new Beam(stage, position + new Vector2(size.X / 2, -15), direction, characterControl, effectControl, gameDevice));
            }
            dartle++;
        }

        #endregion

        #region 描画

        public override void Draw(Renderer renderer) {
            DrawGuage(renderer);
            fireworksControl.Draw(renderer);
            DrawLaser(renderer);
            if (direction == Direction.LEFT) { 
                renderer.DrawTexture(name, stage.GetScreenPosition(position - size / 2), motion.DrawRange_L(), alpha);
                effectControl.DrawEffect_L(renderer);
            }
            if (direction == Direction.RIGHT) { 
                renderer.DrawTexture(name + "2", stage.GetScreenPosition(position - size / 2), motion.DrawRange_R(), alpha);
                effectControl.DrawEffect_R(renderer);
            }
            
            DrawElectric(renderer);
            
        }

        /// <summary>
        /// hpとspを描画する用
        /// </summary>
        /// <param name="renderer"></param>
        private void DrawGuage(Renderer renderer) {
            Vector2 hpPosition = new Vector2(15, 20);
            Vector2 spPosition = new Vector2(15, 75);

            //spとhpに合わせて描画する
            renderer.DrawTexture("gauge", spPosition, new Rectangle(64, 128, 192, 64));
            for (int i = 0; i < hp; i++) {
                renderer.DrawTexture("gauge", hpPosition, new Rectangle(0, 128, 64, 64));
                hpPosition.X += 64;
            }
            
            for (int i = 0; i < sp; i++) {
                renderer.DrawTexture("gauge", spPosition, new Rectangle(64 * (i + 1), 192, 64, 64));
                spPosition.X += 64;
            }
        }


        /// <summary>
        /// レーザーを描画する用
        /// </summary>
        /// <param name="renderer"></param>
        private void DrawLaser(Renderer renderer) {
            if (mode == ActionMode.LASERSTART) {
                if (direction == Direction.RIGHT) {
                    Vector2 electricPosition = position + new Vector2(size.X / 2 + 50, -10);
                    for (int i = 0; i < Screen.Width - (stage.GetScreenX(position.X)); i++) {
                        renderer.DrawTexture("laserStart", stage.GetScreenPosition(electricPosition), 0.8f);
                        electricPosition.X++;
                    }
                }
                else {
                    Vector2 electricPosition = position - new Vector2(size.X / 2 + 50, 10);
                    for (int i = 0; i < stage.GetScreenX(position.X); i++) {
                        renderer.DrawTexture("laserStart", stage.GetScreenPosition(electricPosition), 0.8f);
                        electricPosition.X--;
                    }
                }
            }

            if (mode == ActionMode.LASERSHOOT) { 
                if (direction == Direction.RIGHT) {
                    //レーザーの描画始めの座標を決める
                    Vector2 laserPosition = stage.GetScreenPosition(new Vector2(position.X + size.X / 2 + 60, position.Y - size.X * 0.80f));
                    //レーザーをざわざわ形で描画する
                    for (int i = 0; i < Screen.Width - (stage.GetScreenX(position.X)); i++) {
                        renderer.DrawTexture("laser", laserPosition, 0.9f);
                        laserPosition.Y = position.Y - size.Y * 0.80f;
                        laserPosition.Y += rand.Next(-5, 6);
                        laserPosition.X++;
                    }
                }
                else {
                    //レーザーの描画始めの座標を決める
                    Vector2 laserPosition = stage.GetScreenPosition(new Vector2(position.X - size.X / 2 - 60, position.Y - size.Y * 0.80f));
                    //レーザーをざわざわ形で描画する
                    for (int i = 0; i < stage.GetScreenX(position.X); i++) {
                        renderer.DrawTexture("laser", laserPosition, 0.9f);
                        laserPosition.Y = position.Y - size.Y * 0.80f;
                        laserPosition.Y += rand.Next(-5, 6);
                        laserPosition.X--;
                    }
                }
            }
        }

        /// <summary>
        /// レーザーの電気エフェクトを描画する
        /// </summary>
        /// <param name="renderer"></param>
        private void DrawElectric(Renderer renderer) {
            if (sp == 3) {
                if (direction == Direction.RIGHT) {
                    fireworksControl.AddFireworks(new Fireworks1(new Vector2(-5, -5), stage, this));
                }
                else {
                    fireworksControl.AddFireworks(new Fireworks1(new Vector2(5, -5), stage, this));
                }
            }


            if (mode == ActionMode.LASERSHOOT) {
                if (direction == Direction.RIGHT) {
                    //エフェクトの描画始めの座標を決める
                    Vector2 electricPosition = position + new Vector2(size.X / 2, 0);
                    for (int i = 0; i < Screen.Width - (stage.GetScreenX(position.X)); i++) {
                        //1つのフレームの中に何個電気の粒を描画するの決めてから処理
                        for (int x = 0; x <= rand.Next(8); x++) {
                            //y座標どのぐらいの量をずらして描画するの決める
                            int randY = rand.Next(-5, 6);
                            renderer.DrawTexture("electric", stage.GetScreenPosition(electricPosition));
                            //ずらせる範囲を決めて、もし超えたら逆方向にずらす
                            if (electricPosition.Y + randY >= position.Y + size.Y || electricPosition.Y + randY <= position.Y - size.Y) {
                                randY *= -1;
                            }
                            //y座標ずらす
                            electricPosition.Y += randY;
                            //ランダムで、ｘ座標進むかどうか決める
                            if (rand.Next(2) == 0) { electricPosition.X++; }
                        }
                        //ｘ座標向きの方向に1ピクセル進む
                        electricPosition.X++;
                    }
                }
                else {
                    //エフェクトの描画始めの座標を決める
                    Vector2 electricPosition = position - new Vector2(size.X / 2, 0);
                    //1つのフレームの中に何個電気の粒を描画するの決めてから処理
                    for (int i = 0; i < stage.GetScreenX(position.X); i++) {
                        for (int x = 0; x <= rand.Next(8); x++) {
                            //y座標どのぐらいの量をずらして描画するの決める
                            int randY = rand.Next(-5, 6);
                            renderer.DrawTexture("electric", stage.GetScreenPosition(electricPosition));
                            //ずらせる範囲を決めて、もし超えたら逆方向にずらす
                            if (electricPosition.Y + randY >= position.Y + size.Y || electricPosition.Y + randY <= position.Y - size.Y) {
                                randY *= -1;
                            }
                            //y座標ずらす
                            electricPosition.Y += randY;
                            //ランダムで、ｘ座標進むかどうか決める
                            if (rand.Next(2) == 0) { electricPosition.X--; }
                        }
                        //ｘ座標向きの方向に1ピクセル進む
                        electricPosition.X--;
                    }
                }


            }
        }



        #endregion

    }
}
