using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Device;
using FairyLink.Def;
using FairyLink.Scene;

namespace FairyLink.Actor
{
    class Piece
    {
        private string name;
        private Rectangle rect;     //分割された部分のRectangleを保存する
        private Vector2 position;
        private Vector2 velocity;       //1フレームの移動量
        private bool isDead;
        private Stage stage;

        public Piece(string name, Rectangle rect,  Vector2 position, Vector2 velocity, Stage stage) {
            this.name = name;
            this.rect = rect;
            this.position = position;
            this.velocity = velocity;
            this.stage = stage;
            position += velocity;
            velocity.Y = 3;
            isDead = false;
        }

        public void Update() {
            velocity.Y += 0.5f;
            position += velocity;

            //マップ外に落ちた場合、死ぬ
            if (position.Y > StageMap.Height) { isDead = true; }
        }

        public bool IsDeath { get { return isDead; } }

        public void Draw(Renderer renderer) {
            renderer.DrawTexture(name, stage.GetScreenPosition(position), rect);
        }
        

    }
}
