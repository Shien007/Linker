using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;

namespace FairyLink.Actor.ModeDisign
{
    class SkillShoot : PlayerAttack
    {
        private Timer skillShootTimer;      //このモードの保持時間
        public SkillShoot(Motion motion, Player player)
            : base(motion, player)
        { skillShootTimer = new Timer(1.2f); }

        public override void Attack() { }

        //時間尽くと次の状態に移す
        public override void Update() {
            if (player.Mode != ActionMode.SKILLSHOOT) { skillShootTimer.Initialize(); return; }
            skillShootTimer.Update();
            if (skillShootTimer.IsTime) {
                ChangeNext();
                skillShootTimer.Initialize();
            }
        }

        //次の状態を設置する
        protected override void ChangeNext() {
            player.Mode = ActionMode.STAND;
        }

    }
}
