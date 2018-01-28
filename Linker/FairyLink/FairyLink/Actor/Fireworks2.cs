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
    /// sp吸収演出
    /// </summary>
    class Fireworks2 : Fireworks
    {
        private string name;
        private Vector2 position;
        private Vector2 startPosition;
        private float size;
        private int timer;
        private float rocation;

        public Fireworks2(Vector2 velocity, Stage stage, Player player) 
            : base(velocity, stage, player)
        {
            name = "electric";
            startPosition = position;
            position = player.Position;
            rocation = 0;
            isDead = false;
            size = 5.0f;
            timer = 0;
        }



        public override void Update() {
            timer++;
            rocation += 0.2f;
            position += velocity * 7;
            if (timer % 2 == 0) {
                size += 0.1f;
            }
            if (position.Y <= 150) {
                isDead = true;
            }

        }

        public override void Draw(Renderer renderer) {
            if (isDead) return;
            renderer.DrawTexture(name, stage.GetScreenPosition(position), 1f , new Rectangle(0, 0, 1, 1),size, rocation, new Vector2(size, size));
        }
    }
}
