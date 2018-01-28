using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Utility;
using FairyLink.Device;

namespace FairyLink.Actor
{
    class Effect
    {
        private EffectMode effectMode;      //エフェクトを登録されているenum
        private Vector2 effectPosition;
        private bool isDead;
        private Timer timer;
        private EffectMotion effectMotion;  //エフェクトのアニメーション管理用
        private CharacterControl characterControl;

        public Effect(EffectMode effectMode, Vector2 effectPosition, Timer timer, CharacterControl characterControl)
        {
            effectMotion = new EffectMotion();
            this.effectMode = effectMode;
            this.effectPosition = effectPosition;
            this.timer = timer;
            this.characterControl = characterControl;
            isDead = false;
            Initialize();
        }

        /// <summary>
        /// エフェクトを登録、そして生成された時のモードによって初期化する
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

            //BeamAttack
            effectMotion.Add(8, new Rectangle(256, 256, 64, 64));

            //BeamCollision
            effectMotion.Add(9, new Rectangle(320, 256, 64, 64));

            //Landed
            effectMotion.Add(10, new Rectangle(384, 256, 64, 64));

            //Jump
            effectMotion.Add(11, new Rectangle(448, 256, 64, 64));

            //Laser
            effectMotion.Add(4, new Rectangle(0, 320, 256, 256));
            effectMotion.Add(5, new Rectangle(256, 320, 256, 256));

            //WallJump
            effectMotion.Add(12, new Rectangle(0, 576, 64, 64));

            if (effectMode == EffectMode.LANDED) {
                effectMotion.Initialize(new Range(10, 10), new Timer(0.2f));
            }

            if (effectMode == EffectMode.JUMP) {
                effectMotion.Initialize(new Range(11, 11), new Timer(0.2f));
            }

            if (effectMode == EffectMode.BEAMATTACK) {
                effectMotion.Initialize(new Range(8, 8), new Timer(0.2f));
            }

            if (effectMode == EffectMode.BEAMCOLLISION) {
                effectMotion.Initialize(new Range(9, 9), new Timer(0.2f));
            }

            if (effectMode == EffectMode.ATTACK) {
                effectMotion.Initialize(new Range(2, 3), new Timer(0.2f));
            }

            if (effectMode == EffectMode.LINK) {
                effectMotion.Initialize(new Range(0, 1), new Timer(0.2f));
            }

            if (effectMode == EffectMode.LASER) {
                effectMotion.Initialize(new Range(4, 5), new Timer(0.2f));
                effectMotion.Roll = true;
            }

            if (effectMode == EffectMode.LASERBURN) {
                effectMotion.Initialize(new Range(6, 7), new Timer(0.2f));
                effectMotion.Roll = true;
            }

            if (effectMode == EffectMode.WALLJUMP) {
                effectMotion.Initialize(new Range(12, 12), new Timer(0.2f));
                effectMotion.Roll = false;
            }
        }


        public void Update() {
            if (effectMode == EffectMode.LASER) {
                if (characterControl.GetPlayerDirection() == Direction.RIGHT) {
                    effectPosition = characterControl.GetPlayerPosition() - new Vector2(64, 128);
                }
                else {
                    effectPosition = characterControl.GetPlayerPosition() - new Vector2(192, 128);
                }
            }
            if (effectMode == EffectMode.LINK)
            {
                if (characterControl.GetPlayerDirection() == Direction.RIGHT)
                {
                    effectPosition = characterControl.GetPlayerPosition() - new Vector2(-64, 128);
                }
                else {
                    effectPosition = characterControl.GetPlayerPosition() - new Vector2(320, 128);
                }
                
            }

            timer.Update();
            effectMotion.Update();      //アニメーション状態更新
            if (timer.IsTime) { isDead = true; }
            if (characterControl.GetPlayerMode() == ActionMode.DEATH) { isDead = true; }
        }

        public bool IsDead {
            get { return isDead; }
        }

        public Vector2 Position {
            get { return effectPosition; }
        }

        public Timer Time {
            get { return timer; }
        }

        public EffectMode Mode {
            get { return effectMode; }
        }

        public EffectMotion Motion {
            get { return effectMotion; }
        }


    }
}
