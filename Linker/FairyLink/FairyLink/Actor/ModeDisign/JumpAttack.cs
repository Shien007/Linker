using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;

namespace FairyLink.Actor.ModeDisign
{
    class JumpAttack : PlayerAttack
    {
        private Timer attackTimer;      //このモードの保持時間

        public JumpAttack(Motion motion, Player player)
            : base(motion, player)
        { attackTimer = new Timer(1.0f); }

        //player今の状態をチェックしてから状態チェンジ
        public override void Attack() {
            if (player.Mode != ActionMode.JUMP) { return; }
            else if (player.Mode == ActionMode.JUMPATTACK) { return; }
            attackTimer.Initialize();
            player.Mode = ActionMode.JUMPATTACK;
            
            motion.Initialize(new Range(6, 7), timer);
            motion.Roll = false;
        }

        //時間尽くと次の状態に移す
        public override void Update() {
            if (player.Mode != ActionMode.JUMPATTACK) { return; }
            attackTimer.Update();
            if (attackTimer.IsTime) {
                ChangeNext();
                motion.Roll = true;
                attackTimer.Initialize();
            }
        }

        public void InitializeAttackTimer() {
            attackTimer.Initialize();
        }

        //次の状態を設置する
        protected override void ChangeNext() {
            player.Mode = ActionMode.JUMP;
            motion.Initialize(new Range(5, 5), timer);
        }
    }
}
