using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Scene;
using Microsoft.Xna.Framework;
using FairyLink.Device;

namespace FairyLink.Actor
{
    /// <summary>
    /// sp吸収完了演出
    /// </summary>
    class Fireworks3 : Fireworks
    {
        private string name;
        private Vector2 position;
        private Vector2 startPosition;
        private float size;
        private int timer;
        private float rocation;

        public Fireworks3(Vector2 velocity, Stage stage, Player player) 
            : base(velocity, stage, player)
        {
            name = "electric";
            position = new Vector2(210, 105);
            startPosition = position;
            rocation = 0;
            isDead = false;
            size = 5.0f;
            timer = 0;
        }

        public override void Update()
        {
            timer++;
            position += velocity;
            if (timer % 2 == 0)
            {
                size += 0.1f;
            }

            float distence = (position.X - startPosition.X) * (position.X - startPosition.X) + (position.Y - startPosition.Y) * (position.Y - startPosition.Y);
            if (distence >= 4000)
            {
                isDead = true;
            }

        }

        public override void Draw(Renderer renderer)
        {
            if (isDead) return;
            renderer.DrawTexture(name, position, 1f, new Rectangle(0, 0, 1, 1), size, rocation, new Vector2(0, 0));
        }
    }
}
