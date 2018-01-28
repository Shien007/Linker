using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using FairyLink.Scene;
using FairyLink.Device;

namespace FairyLink.Actor
{
    abstract class Fireworks
    {
        protected Vector2 velocity;
        protected Player player;
        protected Stage stage;
        protected bool isDead;

        public Fireworks(Vector2 velocity, Stage stage, Player player) {
            this.velocity = velocity;
            this.stage = stage;
            this.player = player;
            isDead = false;
        }

        public abstract void Update();

        public abstract void Draw(Renderer renderer);

        public bool IsDead {
            get { return isDead; }
            set { isDead = value; }
        }

    }
}
