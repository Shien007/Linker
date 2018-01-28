using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Def;
using FairyLink.Device;
using FairyLink.Utility;
using Microsoft.Xna.Framework;

namespace FairyLink.Scene.Scenes
{
    class FireBall
    {
        private float x, y;
        private float tx, ty;
        private Timer timer;
        private int radius;

        private float rocate_f;
        private float rocation_f;

        private float rocate;
        private float rocation;

        private int fireSize;

        public FireBall(float x, float y, int radius, float rocate, int dic)
        {
            this.x = x; //マップチップのx座標
            this.y = y; //マップチップのy座標
            this.radius = radius;
            tx = x * StageMap.BlockSize;
            ty = y * StageMap.BlockSize;

            timer = new Timer(0.02f);
            rocate_f = 0;
            rocation_f = (float)Math.PI / 24;

            this.rocate = rocate;
            rocation = (float)Math.PI / 48 * dic;

            fireSize = 32;
        }

        public void Initalize()
        {
            rocate_f = 0;
        }

        public void Update()
        {
            timer.Update();
            if (timer.IsTime) {
                rocate_f += rocation_f;
                rocate += rocation;
                timer.Initialize();
                if (Math.Abs(rocate) == (float)Math.PI * 2) { rocate = 0; }
                if (Math.Abs(rocate_f) == (float)Math.PI * 2) { rocate = 0; }
            }
            tx = x * StageMap.BlockSize + radius * (float)Math.Cos(rocate);
            ty = y * StageMap.BlockSize + radius * (float)Math.Sin(rocate);
        }

        public Vector2 Position { get { return new Vector2(tx + fireSize / 2, ty + fireSize / 2); } }
        
        public int Size { get { return fireSize; } }

        public void Draw(Renderer renderer, Stage stage)
        {
            Rectangle rect = new Rectangle(0, 0, fireSize, fireSize);
            renderer.DrawTexture("fireball", new Vector2(stage.GetScreenX(tx), ty),
                1.0f, rect, 1.0f, rocate_f, 
                new Vector2(fireSize / 2, fireSize / 2));
        }


    }
}
