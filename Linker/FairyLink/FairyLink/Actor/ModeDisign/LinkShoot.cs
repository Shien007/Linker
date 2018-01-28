using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;

namespace FairyLink.Actor.ModeDisign
{
    class LinkShoot : PlayerAttack
    {
        private Timer linkShootTimer;      //このモードの保持時間
        public LinkShoot(Motion motion, Player player)
            : base(motion, player)
        { linkShootTimer = new Timer(3.0f); }

        public override void Attack() { }

        //時間尽くと次の状態に移す
        public override void Update() {
            if (player.Mode != ActionMode.LINKSHOOT) { linkShootTimer.Initialize(); return; }
            linkShootTimer.Update();
            if (linkShootTimer.IsTime) {
                ChangeNext();
                linkShootTimer.Initialize();
            }
        }

        //次の状態を設置する
        protected override void ChangeNext() {
            player.Mode = ActionMode.STAND;
        }
    }
}
