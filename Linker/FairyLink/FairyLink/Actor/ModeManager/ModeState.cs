using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Actor
{
    /// <summary>
    /// アニメーションモードの変化をチェックする用
    /// </summary>
    class ModeState
    {
        private ActionMode currentmode;     //今のフレームのアニメーションモード
        private ActionMode previousmode;    //前のフレームのアニメーションモード

        public ModeState() { }

        public void Update(ActionMode mode) {
            previousmode = currentmode;
            currentmode = mode;
        }


        #region Link

        public bool LinkStarted() {
            return currentmode == ActionMode.LINKSTART && previousmode != ActionMode.LINKSTART;
        }
        public bool LinkDead() {
            return currentmode == ActionMode.DEATH && previousmode == ActionMode.LINKSTART;
        }

        public bool LinkEnd() {
            bool current = (currentmode != ActionMode.LINKSTART && currentmode != ActionMode.LINKSHOOT && currentmode != ActionMode.DEATH);
            bool previous = (previousmode == ActionMode.LINKSTART || previousmode == ActionMode.LINKSHOOT);
            return  current && previous;
        }

        #endregion


        #region Laser

        public bool LaserStarted() {
            return currentmode == ActionMode.LASERSTART && previousmode != ActionMode.LASERSTART;
        }

        public bool LaserShootStart() {
            return currentmode == ActionMode.LASERSHOOT && previousmode != ActionMode.LASERSHOOT;
        }

        public bool LaserEnd() {
            bool laserPause = (currentmode == ActionMode.DAMEGE && previousmode == ActionMode.LASERSTART);
            bool laserEnd = (currentmode != ActionMode.LASERSHOOT && previousmode == ActionMode.LASERSHOOT);
            return laserPause || laserEnd;
        }

        public bool LaserDead() {
            return currentmode == ActionMode.DEATH && previousmode == ActionMode.LASERSHOOT;
        }


        #endregion



        public bool AttackStarted() {
            bool attack = (currentmode == ActionMode.NOMALATTACK && previousmode != ActionMode.NOMALATTACK);
            bool jumpAttack = (currentmode == ActionMode.JUMPATTACK && previousmode != ActionMode.JUMPATTACK);
            return attack || jumpAttack;
        }

        public bool ShootStart() {
            return currentmode == ActionMode.SKILLSHOOT && previousmode != ActionMode.SKILLSHOOT;
        }

        public bool ShootEnd() {
            bool shootEnd1 = (currentmode != ActionMode.SKILLSHOOT && previousmode == ActionMode.SKILLSTART);
            bool shootEnd2 = (currentmode != ActionMode.SKILLSHOOT && previousmode == ActionMode.SKILLSHOOT);

            return shootEnd1 || shootEnd2;
        }

        public bool WallJump() {
            bool jumpStart1 = (currentmode == ActionMode.WALLJUMPSTART && previousmode != ActionMode.WALLJUMPSTART);
            bool jumpStart2 = (currentmode == ActionMode.WALLJUMP && previousmode == ActionMode.WALLJUMPSTART);
            return jumpStart1 || jumpStart2;
        }
        


    }
}
