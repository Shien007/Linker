using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Device;
using FairyLink.Scene;

namespace FairyLink.Actor
{
    class CharacterControl : ICharacterMediator
    {
        private LinkedList<Character> characters;   //存在しているキャラ全員納入する用
        private List<Character> newCharacters;      //今のフレーム追加したキャラを納入する用
        private Stage stage;
        private Gimmick gimmick;
        private static Random rand = new Random();
        private EffectControl effectControl;        //エフェクトを納入する用
        private PieceControl pieceControl;          //分解された画像を納入するよう
        private GameDevice gameDevice;
        private Sound sound;

        public CharacterControl(Stage stage, EffectControl effectControl, PieceControl pieceControl, GameDevice gameDevice, Gimmick gimmick) {
            this.stage = stage;
            this.gimmick = gimmick;
            characters = new LinkedList<Character>();
            newCharacters = new List<Character>();
            this.effectControl = effectControl;
            this.pieceControl = pieceControl;
            this.gameDevice = gameDevice;
            sound = gameDevice.GetSound;
        }

        public void Initialize() {
            foreach (var c in characters) { c.Initialize(); }
            characters.Clear();
        }

        //新しいキャラを格納する
        public void Add(Character character) {
            characters.AddLast(character);
        }

        //Playerを格納する
        public void AddCharacter(Character character) {
            newCharacters.Add(character);
        }


        private void New() {
            foreach (Character c in newCharacters) { Add(c); }
            newCharacters.Clear();
        }

        public void Update() {
            EnemyInitialize();

            foreach (Character c in characters) {
                //映してる画面以外のキャラを更新しない
                if (!c.InScreen(GetPlayerPosition())) { continue; }
                c.Update();
            }
            New();
            Remove();
        }


        /// <summary>
        /// 死んだキャラを検索して削除
        /// </summary>
        private void Remove() {
            //一番目から順次に検索する
            LinkedListNode<Character> node = characters.First;
            while (node != null) {
                LinkedListNode<Character> next = node.Next;

                //player以外のキャラ、もし死んだら、削除する
                if (node.Value.IsDead && !(node.Value is Player)) {
                    characters.Remove(node);
                }
                node = next;
            }
        }

        public bool IsPlayerDead() {
            foreach (Character c in characters) {
                if (c is Player) {
                    return c.IsDead;
                }
            }
            return false;
        }

        public bool IsGameClear() {
            foreach (Character c in characters) {
                if (c is Player) {
                    return ((Player)c).IsGameClear();
                }
            }
            return false;
        }


        public void Draw(Renderer renderer) {
            foreach (Character c in characters) {
                c.Draw(renderer);
            }
        }


        public Vector2 GetPlayerPosition() {
            foreach (Character c in characters) {
                if (c is Player) { return c.Position; }
            }
            return Vector2.Zero;
        }


        public ActionMode GetPlayerMode()
        {
            foreach (Character c in characters)
            {
                if (c is Player) { return c.Mode; }
            }
            return ActionMode.EMPTY;
        }

        public Direction GetPlayerDirection() {
            foreach (Character c in characters)
            {
                if (c is Player) { return c.GetDirection; }
            }
            return Direction.UP;
        }


        /// <summary>
        /// 敵を生成する
        /// </summary>
        private void EnemyInitialize() {
            //スデージマップによって、敵を生成する
            while (gimmick.MapEnemyBorn()) {
                Vector2 position = gimmick.GetBornPosition;

                Direction direction;

                //ランダムで敵の向きを決める
                if (rand.Next(2) == 0) {
                    direction = Direction.LEFT;
                }
                else { direction = Direction.RIGHT; }

                //64:スライム型、65:遠距離タイプ、66:砲台
                if (gimmick.GetBornType == 64) { Add(new Enemy_Slime(stage, position, 5, direction, this, effectControl, gameDevice)); }
                else if (gimmick.GetBornType == 65) { Add(new Enemy_Seed(stage, position, 5, direction, this, effectControl, pieceControl, gameDevice)); }
                else if (gimmick.GetBornType == 66) { Add(new Battery(stage, position, 5, Direction.LEFT, this, effectControl, pieceControl, gameDevice)); }
            }
        }

        public LinkedList<Character> GetCharacter { get { return characters; } }

    }
}
