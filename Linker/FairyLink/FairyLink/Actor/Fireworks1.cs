using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Scene;

using Microsoft.Xna.Framework;

namespace FairyLink.Actor
{
    /// <summary>
    /// sp満タン演出
    /// </summary>
    class Fireworks1 : Fireworks
    {
        private string name;
        private Vector2 position;
        private Vector2 startPosition;
        private float size;
        private int timer;
        private float rocation;

        public Fireworks1(Vector2 velocity, Stage stage, Player player)
            :base(velocity, stage, player)
        {
            name = "electric";
            startPosition = position;
            this.player = player;
            rocation = 0;
            size = 1.0f;
            timer = 0;
        }



        public override void Update() {
            position = player.Position;
            timer++;
            rocation += 0.1f;
            velocity += new Vector2(velocity.X * (float)Math.Cos(Math.PI * rocation), velocity.Y * (float)Math.Sin(Math.PI * rocation));
            position += velocity;
            if (timer % 2 == 0) {
                size += 0.05f;
            }
            if (rocation >= 23) {
                isDead = true;
            }

        }

        public override void Draw(Renderer renderer) {
            if (isDead) return;
            renderer.DrawTexture(name, stage.GetScreenPosition(position), 0.5f , new Rectangle(0, 0, 1, 1),size, rocation, new Vector2(size * 2, size * 2));
        }
    }
}
