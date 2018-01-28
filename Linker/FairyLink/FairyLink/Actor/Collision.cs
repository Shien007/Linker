using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Device;
using FairyLink.Scene;

namespace FairyLink.Actor
{
    class Collision
    {
        private Sound sound;
        private Character player;
        private CharacterControl characterControl;

        public Collision(Sound sound, Character player, CharacterControl characterControl) {
            this.characterControl = characterControl;
            this.sound = sound;
            this.player = player;
        }

        public void Update() {
            CollisionPlayerEnemy();     //playerと敵のあたり判定
            CollisionPlayerAttack();     //playerの攻撃と敵のあたり判定
            CollisionPlayerLink();     //playerのlink攻撃と敵のあたり判定
            CollisionPlayerSkill();     //playerのスギール攻撃と敵のあたり判定
            CollisionPlayerLaser();     //playerのレーザー攻撃と敵のあたり判定
            CollisionPlayerBullet();     //playerと敵の弾のあたり判定
        }

        private void CollisionPlayerBullet() {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Bullet) {
                    CollisionPlayerBulletOne(c);
                }
            }
        }

        private void CollisionPlayerBulletOne(Character e) {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Player) {
                    if (c.IsCollisionBullet(e)) { 
                        c.Hit();
                        e.Hit(c);
                    }
                }
            }
        }

        private void CollisionPlayerEnemy() {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Enemy_Slime || c is Enemy_Seed) {
                    CollisionPlayerEnemyOne(c);
                }   
            }
        }

        private void CollisionPlayerEnemyOne(Character e) {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Player) {
                    if (c.IsCollision(e)) { c.Hit(); }
                }
            }
        }

        private void CollisionPlayerAttack() {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Enemy_Slime || c is Enemy_Seed) { CollisionPlayerAttackOne(c); }
            }
        }

        private void CollisionPlayerAttackOne(Character e) {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Player) {
                    if (c.IsAttackCollision(e)) { e.Hit(c); }
                }
            }
        }

        private void CollisionPlayerLink() {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Enemy_Slime || c is Enemy_Seed) { CollisionPlayerLinkOne(c); }
            }
        }

        private void CollisionPlayerLinkOne(Character e) {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Player) {
                    if (c.IsLinkCollision(e)) {
                        if (e is Enemy_Slime) { ((Enemy_Slime)e).Link(); }
                        else if (e is Enemy_Seed) { ((Enemy_Seed)e).Link(); }
                    }
                    //link攻撃に当たってない場合、もし敵の状態はlinkstartだったら、Stand状態に戻す
                    else {
                        if (e.Mode == ActionMode.LINKSTART) { e.Mode = ActionMode.STAND; }
                    }
                    //敵はリンクで死んだ場合、playerのSpが一個たまる
                    if (e.LinkDead()) { ((Player)c).SpAbsorb(); }
                }
            }
        }

        private void CollisionPlayerSkill() {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Enemy_Slime || c is Enemy_Seed) { CollisionPlayerSkillOne(c); }
            }
        }

        private void CollisionPlayerSkillOne(Character e) {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Beam || c is Bullet) {
                    if (c.IsCollision(e)) { 
                        e.Hit(c);
                        c.Hit(e);
                    }
                }
            }
        }

        private void CollisionPlayerLaser() {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Enemy_Slime || c is Enemy_Seed) { CollisionPlayerLaserOne(c); }
            }
        }

        private void CollisionPlayerLaserOne(Character e) {
            foreach (var c in characterControl.GetCharacter) {
                if (c is Player) {
                    if (c.IsLaserCollision(e)) {
                        //laser攻撃の攻撃力はplayerの通常攻撃力の5倍
                        if (e is Enemy_Slime) { ((Enemy_Slime)e).Laser(c.GetAttack * 5); }
                        else if (e is Enemy_Seed) { ((Enemy_Seed)e).Laser(c.GetAttack * 5); }
                    }
                    else {
                        //laser攻撃に当たってない場合、もし敵の状態はlaserShootだったら、Death状態に移る
                        if (e.Mode == ActionMode.LASERSHOOT) { e.Mode = ActionMode.DEATH; }
                    }
                }
            }


            






        }
    
    
    }
}
