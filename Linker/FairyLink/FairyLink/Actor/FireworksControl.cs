using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;
using Microsoft.Xna.Framework;
using FairyLink.Scene;

namespace FairyLink.Actor
{
    class FireworksControl
    {
        private Timer addTimer;
        private List<Fireworks> firworks;
        private Timer fireTimer;
        private bool fireSwich;
        private Stage stage;
        private Player player;
        private float[,] fireVelocity = new float[,]{
            { 0,-1.5f}, { 0.5f,-1.2f }, { 1,-1 }, { 1.5f,-1.2f},
            { 1.5f,0}, { 0.5f,1.2f }, { 1,1 }, { 1.5f,1.2f },
            { 0,1.5f}, { -0.5f,1.2f }, { -1,1 }, { -1.5f,1.2f },
            { -1.5f,0}, { -0.5f,-1.2f }, { -1,-1 }, { -1.5f,-1.2f },
            };

        public FireworksControl(Stage stage, Player player) {
            this.stage = stage;
            this.player = player;
            addTimer = new Timer(0.2f);
            firworks = new List<Fireworks>();
            fireTimer = new Timer(1.0f);
            fireSwich = false;
        }


        public void AddFireworks(Fireworks firework) {
            if (addTimer.IsTime) {
                firworks.Add(firework);
                if (firework is Fireworks1) {
                addTimer.Initialize();
                }
            }
        }

        public void SpAbsorb(Stage stage, Player player) {
            float velocityX = (stage.GetScreenX(player.Position.X) - 200) / (stage.GetScreenPosition(player.Position).Y - 105);

            firworks.Add(new Fireworks2(new Vector2(-velocityX, -1), stage, player));
        }


        public void Update() {
            addTimer.Update();
            firworks.ForEach(f =>
            {
                f.Update();
                if (f is Fireworks2 && f.IsDead) { fireSwich = true; }
            } );
            firworks.RemoveAll(f => f.IsDead);
            if (fireSwich) { Fire(); }
        }


        private void Fire() {
            fireTimer.Update();
            for (int j = 0; j < fireVelocity.GetLength(0); j++) {
                AddFireworks(new Fireworks3(5 * new Vector2(fireVelocity[j,0], fireVelocity[j, 1]), stage, player));
            }

            if (addTimer.IsTime){
                addTimer.Initialize();
            }

            if (fireTimer.IsTime) {
                fireTimer.Initialize();
                fireSwich = false;
            }
        }


        public void Draw(Renderer renderer) {
            firworks.ForEach(f => f.Draw(renderer));
        }
    }
}
