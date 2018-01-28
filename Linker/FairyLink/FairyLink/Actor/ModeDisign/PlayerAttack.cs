using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;

namespace FairyLink.Actor.ModeDisign
{
    abstract class PlayerAttack
    {
        protected Motion motion;    //アニメーションを管理するクラス
        protected Timer timer;
        protected Player player;

        public PlayerAttack(Motion motion, Player player)
        {
            this.motion = motion;
            this.player =player;
            timer = new Timer(0.2f);
        }

        public abstract void Attack();
        public abstract void Update();
        protected abstract void ChangeNext();


    }
}
