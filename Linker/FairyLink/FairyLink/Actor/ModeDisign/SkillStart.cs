using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;

namespace FairyLink.Actor.ModeDisign
{
    class SkillStart: PlayerAttack
    {
        private Timer skillStartTimer;  //このモードの保持時間
        public SkillStart(Motion motion, Player player)
            : base(motion, player)
        { skillStartTimer = new Timer(0.4f); }

        //player今の状態をチェックしてから状態チェンジ
        public override void Attack() {
            if (player.Mode != ActionMode.STAND && player.Mode != ActionMode.MOVE) { return; }
            player.Sp--;
            player.Mode = ActionMode.SKILLSTART;
            motion.Initialize(new Range(12, 13), new Timer(0.4f));
        }

        //時間尽くと次の状態に移す
        public override void Update() {
            if (player.Mode != ActionMode.SKILLSTART) { return; }
            skillStartTimer.Update();
            if (skillStartTimer.IsTime) {
                ChangeNext();
                skillStartTimer.Initialize();
            }
        }

        //次の状態を設置する
        protected override void ChangeNext() {
            player.Mode = ActionMode.SKILLSHOOT;
            motion.Initialize(new Range(14, 15), timer);
        }
    }
}
