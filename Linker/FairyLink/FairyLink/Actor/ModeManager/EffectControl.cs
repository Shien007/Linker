using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Utility;
using FairyLink.Device;
using FairyLink.Scene;

namespace FairyLink.Actor
{
    class EffectControl
    {
        private List<Effect> effects;
        private EffectMotion effectMotion;  //エフェクトのパタンを登録されているenum
        private Stage stage;

        public EffectControl(Stage stage) {
            this.stage = stage;
            effects = new List<Effect>();
            effectMotion = new EffectMotion();  //エフェクトのアニメーションを管理する用
        }

        /// <summary>
        /// エフェクトの画像をパタンによって、分割してから登録
        /// </summary>
        public void Initialize() {
            //Link
            effectMotion.Add(0, new Rectangle(0, 0, 256, 256));
            effectMotion.Add(1, new Rectangle(256, 0, 256, 256));
            
            //attack
            effectMotion.Add(2, new Rectangle(0, 256, 64, 64));
            effectMotion.Add(3, new Rectangle(64, 256, 64, 64));

            //Laserburn
            effectMotion.Add(6, new Rectangle(128, 256, 64, 64));
            effectMotion.Add(7, new Rectangle(192, 256, 64, 64));

            //Laser
            effectMotion.Add(4, new Rectangle(0, 320, 256, 256));
            effectMotion.Add(5, new Rectangle(256, 320, 256, 256));
        }

        public void AddEffect(Effect effect) {
            effects.Add(effect);
        }


        public void Update() {
            foreach (var e in effects) {
                e.Update();
            }
            effects.RemoveAll(e => e.IsDead);
            effectMotion.Update();
        }

        public void LinkEnd() {
            effects.RemoveAll(e => e.Mode == EffectMode.LINK);
        }

        public void LaserEnd() {
            effects.RemoveAll(e => e.Mode == EffectMode.LASER);
            effects.RemoveAll(e => e.Mode == EffectMode.LASERBURN);
        }

        public EffectMotion GetEffectMotion {
            get { return effectMotion;  }
        }

        /// <summary>
        /// 左向きの画像を描画する
        /// </summary>
        /// <param name="renderer">描画用クラス</param>
        public void DrawEffect_L(Renderer renderer) {
            foreach (var e in effects) {
                renderer.DrawTexture("effect", stage.GetScreenPosition(e.Position), e.Motion.DrawRange_L());
            }
        }

        /// <summary>
        /// 右向きの画像を描画する
        /// </summary>
        /// <param name="renderer">描画用クラス</param>
        public void DrawEffect_R(Renderer renderer) {
            foreach (var e in effects) {
                renderer.DrawTexture("effect2", stage.GetScreenPosition(e.Position), e.Motion.DrawRange_L());
            }
        }


    }
}
