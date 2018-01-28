using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Device;
using FairyLink.Def;
using FairyLink.Utility;
using FairyLink.Actor;
using FairyLink.Scene.Scenes;

namespace FairyLink.Scene
{
    enum GimmickNum {
        Jewel = 8,
        Fire,
        Crystal,
        Door,
        Gravel,
        FireBall_R = 16,
        FireBall_L,
        CollisionBox_Stage2,
        CollisionBox_Nomal,
        MoveBlock_None = 24,
        MoveBlock_Up,
        MoveBlock_Down,
        SwitchDoor_S = 32,
        SwitchDoor_D,
    }
    class Gimmick
    {
        private PieceControl pieceControl;
        private Stage stage;   //横スクロールの量
        private Motion motionDoor;  //ゴールアニメーション用
        private Sound sound;
        private static Random rand = new Random();
        private Vector2 bornPosition;   //敵発生の位置
        private int bornType;   //敵の種類

        private List<Crystal> crystals; //チェックポイント管理用
        private List<SwitchDoor> switchDoors;
        private List<Point> switchDoorPosition;
        private List<Gravel> gravels; //崩れる足場管理用
        private List<Gravel> gravelsSave; //崩れた足場管理用


        private List<MoveBlock> moveBlocks; //移動足場管理用
        private List<Fire> fires;   //火の管理用
        private List<Jewel> jewels;
        private List<FireBall> fireBalls;



        public int stageNumber { get; private set; }
        private StageLoader stageLoader;

        private int[,] gimmickData = new int[StageMap.YMax, StageMap.XMax];

        public void SetStageNumber(int stageNumber) {
            this.stageNumber = stageNumber;
        }

        public Gimmick(Stage stage, StageLoader stageLoader, PieceControl pieceControl, GameDevice gameDevice) {
            this.stage = stage;
            this.stageLoader = stageLoader;
            this.pieceControl = pieceControl;
            sound = gameDevice.GetSound;
            ObjectInitialize();
        }

        public void Initialize() {
            gimmickData = stageLoader.GimmickLoad(stageNumber);
            ObjectClear();
            NewObject();

            motionDoor = new Motion();
            for (int i = 0; i < 20; i++) {
                motionDoor.Add(i, new Rectangle(StageMap.BlockSize * (i % 8), StageMap.BlockSize * (i / 8),
                    StageMap.BlockSize, StageMap.BlockSize));
            }
            motionDoor.Initialize(new Range(8, 15), new Timer(0.1f));
        }

        public void Update() {
            UpdateObject();
        }

        private void MapInitialize(int x, int y, bool isCollize)
        {
            if (isCollize)
            {
                switch (stageNumber)
                {
                    case 1:
                        gimmickData[y, x] = 19;
                        break;
                    case 2:
                        gimmickData[y, x] = 18;
                        break;
                    default:
                        gimmickData[y, x] = 19;
                        break;
                }
            }
            else {
                switch (stageNumber)
                {
                    case 1:
                        gimmickData[y, x] = -1;
                        break;
                    case 2:
                        gimmickData[y, x] = 0;
                        break;
                    default:
                        gimmickData[y, x] = -1;
                        break;
                }
            }
        }

        //移動床の更新
        private void UpdateMoveBlock() {
            moveBlocks.ForEach(m => m.Update());
        }

        //崩れる足場の更新
        private void UpdateGravel()
        {
            foreach (var g in gravels) {
                g.Update();
                if (g.IsDeath)
                {
                    switch (stageNumber)
                    {
                        case 1:
                            gimmickData[g.Y, g.X] = -1;
                            break;
                        case 2:
                            gimmickData[g.Y, g.X] = 0;
                            break;
                        default:
                            gimmickData[g.Y, g.X] = -1;
                            break;
                    }
                }
            }

            for (int i = 0; i < gravels.Count; i++)
            {
                if (gravels[i].IsDeath)
                {
                    gravelsSave.Add(gravels[i]);
                    gravels.Remove(gravels[i]);
                    i--;
                }
            }

            for (int i = 0; i < gravelsSave.Count; i++)
            {
                gravelsSave[i].Update();
                if (gravelsSave[i].IsReLoad.IsTime)
                {
                    gravelsSave[i].Initalize();
                    switch (stageNumber)
                    {
                        case 1:
                            gimmickData[gravelsSave[i].Y, gravelsSave[i].X] = 19;
                            break;
                        case 2:
                            gimmickData[gravelsSave[i].Y, gravelsSave[i].X] = 18;
                            break;
                        default:
                            gimmickData[gravelsSave[i].Y, gravelsSave[i].X] = 19;
                            break;
                    }
                    gravels.Add(gravelsSave[i]);
                    gravelsSave.Remove(gravelsSave[i]);
                    
                    i--;
                }
            }
        }

        private void ObjectClear()
        {
            crystals.Clear();
            switchDoorPosition.Clear();
            switchDoors.Clear();
            fireBalls.Clear();
            gravels.Clear();
            gravelsSave.Clear();
            moveBlocks.Clear();
            fires.Clear();
            jewels.Clear();
        }

        private void UpdateObject() {
            crystals.ForEach(c => c.Update());
            switchDoors.ForEach(s => s.Update());
            fires.ForEach(f => f.Update());
            fireBalls.ForEach(f => f.Update());
            motionDoor.Update();
            jewels.RemoveAll(j => j.IsDead);
            UpdateGravel();
            UpdateMoveBlock();
        }

        private void NewObject()
        {
            for (int y = 0; y < StageMap.YMax; y++) {
                for (int x = 0; x < StageMap.XMax; x++) {
                    int ct = gimmickData[y, x];
                    switch (ct)
                    {
                        case 8:    //jewelsを生成
                            MapInitialize(x, y, false);
                            jewels.Add(new Jewel(x, y));
                            break;

                        case 9:    //Fireを生成する
                            MapInitialize(x, y, false);
                            fires.Add(new Fire(x, y));
                            break;

                        case 10:
                            crystals.Add(new Crystal(x, y));
                            break;

                        case 12:    //崩れるブロック生成
                            MapInitialize(x, y, true);
                            gravels.Add(new Gravel(pieceControl, stage, x, y, sound));
                            break;

                        case 16:    //FireBall
                            float rocate1 = rand.Next(12) * (float)Math.PI / 6;
                            for (int i = 0; i < 5; i++) {
                                fireBalls.Add(new FireBall(x + 0.5f, y + 0.5f, 32 * (i + 1), rocate1, 1));
                            }
                            break;

                        case 17:    //FireBall
                            float rocate2 = rand.Next(12) * (float)Math.PI / 6;
                            for (int i = 0; i < 5; i++) {
                                fireBalls.Add(new FireBall(x + 0.5f, y + 0.5f, 32 * (i + 1), rocate2, -1));
                            }
                            break;


                        case 24:    //移動ブロック生成
                            MapInitialize(x, y, false);
                            moveBlocks.Add(new MoveBlock(stage, Direction.NONE, x, y));
                            break;

                        case 25:    //移動ブロック生成
                            MapInitialize(x, y, false);
                            moveBlocks.Add(new MoveBlock(stage, Direction.UP, x, y));
                            break;

                        case 26:    //移動ブロック生成
                            MapInitialize(x, y, false);
                            moveBlocks.Add(new MoveBlock(stage, Direction.DOWN, x, y));
                            break;

                        case 32:
                            switchDoors.Add(new SwitchDoor(x, y));
                            break;

                        case 33:
                            switchDoorPosition.Add(new Point(x, y));
                            break;
                    }
                }
            }

            //object生成
            for (int i = 0; i < switchDoors.Count; i++)
            {
                switchDoors[i].DoorX = switchDoorPosition[i].X;
                switchDoors[i].DoorY = switchDoorPosition[i].Y;
            }

        }

        private void ObjectInitialize() {
            crystals = new List<Crystal>();
            switchDoorPosition = new List<Point>();
            switchDoors = new List<SwitchDoor>();
            fireBalls = new List<FireBall>();
            gravels = new List<Gravel>();
            gravelsSave = new List<Gravel>();
            moveBlocks = new List<MoveBlock>();
            fires = new List<Fire>();
            jewels = new List<Jewel>();
        }

        #region Objectとの判定

        private bool IsInGameMap(Vector2 position) {
            bool min = position.X < 0 || position.Y < 0;
            bool max = position.X >= StageMap.Width || position.Y >= StageMap.Height;
            return !min && !max;
        }

        public bool IsGimmick(int x, int y)
        {
            if (gimmickData[y, x] < 0) { return false; }
            return true;
        }

        public Vector2 IsMoveBlock(Vector2 position, int size, Player player)
        {
            if (position.X < 0 || position.X > StageMap.Width || position.Y > StageMap.Height) { return Vector2.Zero; }
            foreach (var m in moveBlocks)
            {
                if (position.X < m.GetPosition.X - 74 || position.X > m.GetPosition.X + 22) { continue; }
                else {
                    for (int i = 0; i < size; i++)
                    {
                        if (position.Y >= m.GetPosition.Y - 8 && position.Y <= m.GetPosition.Y + 15)
                        {
                            if (m.GetDirection == Direction.NONE)
                            {
                                player.Position = new Vector2(player.Position.X, m.GetPosition.Y - 8 - 64);
                                return new Vector2(0.1f, 0.1f);
                            }
                            else if (player.GetSpeed.Y < 0 && player.GetSpeed.Y != m.GetVelocity.Y) { continue; }
                            player.Position = new Vector2(player.Position.X, m.GetPosition.Y - 8 - 64);
                            return m.GetVelocity;
                        }
                        position.X++;
                    }
                }
            }
            return Vector2.Zero;
        }

        public bool IsFire(Vector2 position)
        {
            int x, y;
            x = (int)position.X / StageMap.BlockSize;
            y = (int)position.Y / StageMap.BlockSize;
            foreach (var f in fires)
            {
                if (f.X == x && f.Y == y) { return true; }
                else if (f.X == x && f.Y == y - 1) { return true; }
            }

            return false;
        }

        public bool IsJewel(Vector2 position)
        {
            int x, y;
            x = (int)position.X / StageMap.BlockSize;
            y = (int)position.Y / StageMap.BlockSize;
            foreach (var j in jewels)
            {
                if (j.X == x && j.Y == y)
                {
                    j.IsDead = true;
                    return true;
                }
                else if (j.X == x && j.Y == y - 1)
                {
                    j.IsDead = true;
                    return true;
                }
            }

            return false;
        }

        public bool IsFireBall(Vector2 position, Vector2 collisionSize)
        {
            foreach (var f in fireBalls)
            {
                float xLenghth = Math.Abs(position.X - f.Position.X);
                float yLenghth = Math.Abs(position.Y - f.Position.Y);

                //距離と自分のあたりサイズを比べてチェック
                if (xLenghth <= collisionSize.X / 2 && yLenghth <= collisionSize.Y / 2)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// チェックポイント取ったかどうかチェック
        /// </summary>
        /// <param name="position">playerの座標</param>
        /// <returns></returns>
        public bool IsCrystal(Vector2 position) {
            //playerは画面外の場合チェックしない
            if (!IsInGameMap(position)) { return false; }

            //playerの座標を換算する
            int x, y;
            x = (int)position.X / StageMap.BlockSize;
            y = (int)position.Y / StageMap.BlockSize;

            if (gimmickData[y, x] == 10) {
                foreach (var c in crystals) {
                    if (c.X == x && c.Y == y) {
                        c.Burn = true;
                    }
                }    
                return true; 
            }
            return false;
        }

        /// <summary>
        /// ゴールしたかどうかチェック
        /// </summary>
        /// <param name="position">playerの座標</param>
        /// <returns></returns>
        public bool IsDoor(Vector2 position) {
            //playerは画面外の場合チェックしない
            if (!IsInGameMap(position)) { return false; }

            //playerの座標を換算する
            int x, y;
            x = (int)position.X / StageMap.BlockSize;
            y = (int)position.Y / StageMap.BlockSize;

            if (gimmickData[y, x] == 11) { return true; }
            return false;
        }

        public void IsGravel(Vector2 position)
        {
            if (!IsInGameMap(position)) { return; }

            int x, y;
            x = (int)position.X / StageMap.BlockSize;
            y = (int)position.Y / StageMap.BlockSize;

            foreach (var g in gravels)
            {
                if (g.CrackStart) { continue; }
                if (g.X == x && g.Y == y + 1)
                {
                    g.CrackStart = true;
                }
            }
        }

        public bool IsSwitchDoor(Vector2 position)
        {
            //playerの座標を換算する
            int x, y;
            x = (int)position.X / StageMap.BlockSize;
            y = (int)position.Y / StageMap.BlockSize;


            foreach (var s in switchDoors) {
                if (s.IsOpen) { return false; }
            }

            //playerは画面外の場合チェックしない
            if (position.X < 0 || position.Y < 0) { return false; }
            else if (position.X >= StageMap.Width || position.Y >= StageMap.Height) { return false; }
            else if (gimmickData[y, x] == 33) { return true; }
            return false;
        }

        public void IsSwitch(Vector2 position)
        {
            //playerの座標を換算する
            int x, y;
            x = (int)position.X / StageMap.BlockSize;
            y = (int)position.Y / StageMap.BlockSize;

            //playerは画面外の場合チェックしない
            if (!IsInGameMap(position)) { return; }

            else if (gimmickData[y, x] == (int)GimmickNum.SwitchDoor_S)
            {
                foreach (var s in switchDoors)
                {
                    if (s.X == x && s.Y == y)
                    {
                        s.IsSwitchOn = true;
                    }
                }
            }
        }

        public bool IsGimmickCollision(Vector2 position, int GimmickDataNum) {
            //playerの座標をマップデータに換算する
            int x, y;
            x = (int)position.X / StageMap.BlockSize;
            y = (int)position.Y / StageMap.BlockSize;

            //playerは画面外の場合チェックしない
            if (!IsInGameMap(position)) { return false; }

            else {
                switch (GimmickDataNum) {
                    case (int)(GimmickNum.SwitchDoor_S):
                        foreach (var s in switchDoors)
                        {
                            if (s.X == x && s.Y == y)
                            {
                                s.IsSwitchOn = true;
                                return true;
                            }
                        }
                        break;






                }
                return false;
            }
        }




        #endregion



        #region 敵の生成

        public bool MapEnemyBorn()
        {
            for (int y = 0; y < StageMap.YMax; y++) {
                for (int x = 0; x < StageMap.XMax; x++) {
                    if (gimmickData[y, x] > 63)
                    {
                        bornPosition.X = x * StageMap.BlockSize;
                        bornPosition.Y = y * StageMap.BlockSize;
                        bornType = gimmickData[y, x];

                        if (bornType == 66) { MapInitialize(x, y, true); }
                        else { MapInitialize(x, y, false); }
                        return true;
                    }
                }
            }
            return false;
        }

        public Vector2 GetBornPosition {
            get { return bornPosition; }
        }

        public int GetBornType {
            get { return bornType; }
        }

        #endregion


        public void Draw(Renderer renderer) {
            for (int y = 0; y < StageMap.YMax; y++) {
                for (int x = 0; x < StageMap.XMax; x++) {
                    GimmickDrawOne(renderer, x, y);
                }
            }
            DrawObject(renderer);
        }

        /// <summary>
        /// 一個ずつ、ギミックを描画する
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="x">マップチップｘ座標</param>
        /// <param name="y">マップチップy座標</param>
        private void GimmickDrawOne(Renderer renderer, int x, int y) {
            int ct = gimmickData[y, x];
            if (ct < 0) { return; }
            int tx, ty;
            tx = x * StageMap.BlockSize;
            ty = y * StageMap.BlockSize;

            Rectangle rect = new Rectangle(ct % 8 * StageMap.BlockSize, ct / 8 * StageMap.BlockSize, StageMap.BlockSize, StageMap.BlockSize);


            if (stageNumber == 2 && (ct == 11 || ct == 10))
            {
                renderer.DrawTexture("gimmickMap", new Vector2(stage.GetScreenX(tx), ty), new Rectangle(0, 0, StageMap.BlockSize, StageMap.BlockSize));
            }

            if (ct == 11)
            {
                renderer.DrawTexture("gimmick", new Vector2(stage.GetScreenX(tx), ty), motionDoor.DrawRange_L());
            }
            else if (ct > 15 && ct < 24 || ct < 8)
            {
                renderer.DrawTexture("gimmickMap", new Vector2(stage.GetScreenX(tx), ty), rect);
            }
            
            //看板
            if (stageNumber != 1) { return; }
            int offset = 48; //48番目から表示
            if (ct >= offset) {
                for (int i = 0; i < 6; i++) {
                    renderer.DrawTexture("Signboard", new Vector2(stage.GetScreenX(tx), ty), new Rectangle((ct - offset) % 2 * 128, (ct - offset) / 2 * 96, 128, 96));
                }
            }
        }

        private void DrawObject(Renderer renderer)
        {
            //チェックポイント描画
            crystals.ForEach(c => c.Draw(renderer, stage));

            //SwitchDoorの描画
            switchDoors.ForEach(s => s.Draw(renderer, stage));

            //移動床の描画
            moveBlocks.ForEach(m => m.Draw(renderer));

            //火の描画
            fires.ForEach(f => f.Draw(renderer, stage));

            //宝石の描画
            jewels.ForEach(j => j.Draw(renderer, stage));

            //ファイアボールの描画
            fireBalls.ForEach(f => f.Draw(renderer, stage));

            //崩れる足場の描画
            gravels.ForEach(g => g.Draw(renderer));
        }

        #region マップ判定

        //左右判定
        public bool CollisitionSide(Vector2 position, float size)
        {
            for (int i = 0; i < size; i++) {
                if (!CollisionPoint(position)) { return false; }
                position.Y++;
            }
            return true;
        }

        //とマープのあたり判定、true：いける、false：いけない
        private bool CollisionPoint(Vector2 position)
        {
            int bx, by;
            //今の座標のStageMapの位置を換算する
            bx = (int)(position.X / StageMap.BlockSize);
            by = (int)(position.Y / StageMap.BlockSize);

            if (position.X < 0 || bx >= StageMap.XMax) { return false; }
            if (position.Y < 0 || by >= StageMap.YMax) { return true; }
            if (gimmickData[by, bx] < 16 || gimmickData[by,bx] > 23) { return true; }
            else { return false; }
        }

        //上下判定
        public bool CollisitionUpDown(Vector2 position, float size) {
            bool moveAble = true;
            for (int i = 0; i < size; i++) {
                moveAble = CollisionPoint(position);
                if (!moveAble) { return false; }
                position.X++;
            }
            return moveAble;
        }

        #endregion

    }
}
