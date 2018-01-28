using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;

namespace FairyLink.Actor.ModeDisign
{
    class LinkStart : PlayerAttack
    {
        private Timer linkStartTimer;      //このモードの保持時間
        public LinkStart(Motion motion, Player player)
            : base(motion, player)
        { linkStartTimer = new Timer(0.4f); }

        //player今の状態をチェックしてから状態チェンジ
        public override void Attack() {
            if (player.Mode != ActionMode.STAND && player.Mode != ActionMode.MOVE) { return; }
            player.Mode = ActionMode.LINKSTART;
            motion.Initialize(new Range(9, 11), timer);
        }

        //時間尽くと次の状態に移す
        public override void Update() {
            if (player.Mode != ActionMode.LINKSTART) { return; }
            linkStartTimer.Update();
            if (linkStartTimer.IsTime) {
                ChangeNext();
                linkStartTimer.Initialize();
            }
        }

        //次の状態を設置する
        protected override void ChangeNext() {
            player.Mode = ActionMode.LINKSHOOT;
            motion.Initialize(new Range(11, 11), timer);
        }

    }
}
