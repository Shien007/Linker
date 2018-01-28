using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;

namespace FairyLink.Actor.ModeDisign
{
    class NomalAttack : PlayerAttack
    {
        private Timer attackTimer;

        public NomalAttack(Motion motion, Player player)
            : base(motion, player)
        { attackTimer = new Timer(0.4f); }

        public override void Attack() {
            if (player.Mode != ActionMode.STAND && player.Mode != ActionMode.MOVE) { return; }
            else if (player.Mode == ActionMode.NOMALATTACK) { return; }

            player.Mode = ActionMode.NOMALATTACK;
            motion.Initialize(new Range(6, 7), timer);
        }

        public override void Update() {
            if (player.Mode != ActionMode.NOMALATTACK) { return; }
            attackTimer.Update();
            if (attackTimer.IsTime) {
                ChangeNext();
                attackTimer.Initialize();
            }
        }

        protected override void ChangeNext() {
            player.Mode = ActionMode.STAND;
        }
    }
}
