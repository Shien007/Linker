using Microsoft.Xna.Framework;
using FairyLink.Device;
using FairyLink.Def;
using FairyLink.Actor;

namespace FairyLink.Scene
{
    class Stage
    {
        private int stageX;   //横スクロールの量
        private AutoTiling autoTiling;
        public int stageNumber { get; private set; }
        private StageLoader stageLoader;
        private Vector2 tremble;
        private int trembleTimer;

        private int[,] mapData = new int[StageMap.YMax, StageMap.XMax];
        private PieceControl pieceControl;


        public Stage(StageLoader stageLoader, PieceControl pieceControl, Sound sound) {
            this.stageLoader = stageLoader;
            this.pieceControl = pieceControl;
        }

        public void SetStageNumber(int stageNumber) {
            this.stageNumber = stageNumber;
        }

        public void Initialize() {
            trembleTimer = 0;
            mapData = stageLoader.MapLoad(stageNumber);
            autoTiling = new AutoTiling(this);
            stageX = 0;
            autoTiling.Initialize();
        }


        public int[,] MapData {
            get { return mapData; }
            set { mapData = value; }
        }


        public void Draw(Renderer renderer) {
            autoTiling.Draw(renderer);
        }


        #region マップ判定

        //左右判定
        public bool CollisitionSide(Vector2 position, float size) {
            for (int i = 0; i < size; i++) {
                if (!CollisionPoint(position)) { return false; }
                position.Y++;
            }
            return true;
        }

        //とマープのあたり判定、true：いける、false：いけない
        private bool CollisionPoint(Vector2 position) {
            int bx, by;

            //今の座標のStageMapの位置を換算する
            bx = (int)(position.X / StageMap.BlockSize);
            by = (int)(position.Y / StageMap.BlockSize);

            if (position.X < 0 || bx >= StageMap.XMax) { return false; }
            if (position.Y < 0 || by >= StageMap.YMax) { return true; }
            if (mapData[by, bx] < 0) { return true; }
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

        public bool IsBlock(int x, int y) {
            if (mapData[y, x] < 0) { return false; }
            return true;
        }


        #region 横スクロール

        public void ScrollLeft() {
            if (stageX < StageMap.Width - Screen.Width) { stageX++; }
        }

        public void ScrollRight() {
            if (stageX > 0) { stageX--; }
        }

        public float GetScreenX(float x) {
            x -= stageX;
            return x;
        }

        public Vector2 GetScreenPosition(Vector2 position) {
            position.X -= stageX;
            return position;
        }

        public int StageX {
            get { return stageX; }
            set { stageX = value; }
        }


        #endregion


        #region 震える処理
        public void Tremble() {
            trembleTimer++;     //画面を震える頻度を管理する
            if (trembleTimer == 4) { tremble = new Vector2(-2, 2); }    //左下にずらす
            if (trembleTimer == 8)
            {
                tremble = new Vector2(2, -2);       //右上にずらす
                trembleTimer = 0;           //頻度管理のtimerを初期化する
            }
        }

        public Vector2 GetTrember {
            get { return tremble; }
        }

        public void TremberInitialize() {
            tremble = Vector2.Zero;
        }

        #endregion

    }
}

