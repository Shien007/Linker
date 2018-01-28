using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;

namespace FairyLink.Actor.ModeDisign
{
    class LaserShoot : PlayerAttack
    {
        private Timer laserShootTimer;      //このモードの保持時間
        public LaserShoot(Motion motion, Player player)
            : base(motion, player)
        { laserShootTimer = new Timer(3.0f); }

        public override void Attack() { }

        //時間尽くと次の状態に移す
        public override void Update() {
            if (player.Mode != ActionMode.LASERSHOOT) { laserShootTimer.Initialize(); return; }
            laserShootTimer.Update();
            if (laserShootTimer.IsTime) {
                ChangeNext();
                laserShootTimer.Initialize();
            }
        }

        //次の状態を設置する
        protected override void ChangeNext() {
            player.Mode = ActionMode.STAND;
        }

    }
}
