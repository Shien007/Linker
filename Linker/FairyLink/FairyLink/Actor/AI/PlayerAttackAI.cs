using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using FairyLink.Device;
using Microsoft.Xna.Framework;

namespace FairyLink.Actor.ModeDisign
{
    class PlayerAttackAI
    {
        private Motion motion;  //アニメーション
        private InputState inputState;  //入力チェック
        private Player player;
        
        //playerの攻撃パタンを登録する用
        private Dictionary<ActionMode, PlayerAttack> playerAttacks;
  

        public PlayerAttackAI(Motion motion, Player player, InputState inputState) {
            this.motion = motion;
            this.player = player;
            this.inputState = inputState;
            playerAttacks = new Dictionary<ActionMode, PlayerAttack>();
        }

        //攻撃にかかわるモードを登録する
        public void Initialize() {
            playerAttacks.Add(ActionMode.NOMALATTACK, new NomalAttack(motion, player));
            playerAttacks.Add(ActionMode.JUMPATTACK, new JumpAttack(motion, player));
            playerAttacks.Add(ActionMode.SKILLSTART, new SkillStart(motion, player));
            playerAttacks.Add(ActionMode.SKILLSHOOT, new SkillShoot(motion, player));
            playerAttacks.Add(ActionMode.LINKSTART, new LinkStart(motion, player));
            playerAttacks.Add(ActionMode.LINKSHOOT, new LinkShoot(motion, player));
            playerAttacks.Add(ActionMode.LASERSTART, new LaserStart(motion, player));
            playerAttacks.Add(ActionMode.LASERSHOOT, new LaserShoot(motion, player));
        }

        public void Update() {
            Check();
            //モードはStandの場合、処理を中止する
            if (player.Mode == ActionMode.STAND) { return; }
            foreach (var p in playerAttacks) { p.Value.Update(); }
        }

        //攻撃のキーを押したら、相応の処理に移す
        public void Check() {
            //今は攻撃状態だったら、処理を中止する
            if (playerAttacks.ContainsKey(player.Mode)) { return; }

            if (inputState.WasDown(Keys.X) || inputState.WasDown(Buttons.B)) {
                if (player.Mode == ActionMode.JUMP) { playerAttacks[ActionMode.JUMPATTACK].Attack(); }
                else { playerAttacks[ActionMode.NOMALATTACK].Attack(); }
            }
            else if(inputState.WasDown(Keys.A) || inputState.WasDown(Buttons.RightTrigger)) {
                //SPが足りない場合、攻撃処理に移さない
                if (player.Sp < 3) { return; }

                //攻撃状態に変更して、spを減らす
                playerAttacks[ActionMode.LASERSTART].Attack();
            }
            else if (inputState.WasDown(Keys.S) || inputState.WasDown(Buttons.RightShoulder)) {
                //SPが足りない場合、攻撃処理に移さない
                if (player.Sp < 1) { return; }

                //攻撃状態に変更して、spを減らす
                playerAttacks[ActionMode.SKILLSTART].Attack();
            }
            else if (inputState.WasDown(Keys.C) || inputState.WasDown(Buttons.X)) {
                playerAttacks[ActionMode.LINKSTART].Attack();
            }
        }

        

    }
}
