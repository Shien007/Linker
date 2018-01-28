using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FairyLink.Device;
using FairyLink.Scene.Scenes.Loaders;
using FairyLink.Utility;

namespace FairyLink.Scene
{
    class Loading : IScene
    {
        private Renderer renderer;
        private Sound sound;
        private Motion motion;
        private bool isEnd;

        private TextrueLoader textureLoader;
        private BGMLoader bgmLoader;
        private SELoader seLoader;

        private int totalResouceNum;

        private string[,] TextureList() {
            string path = "./IMAGE/";

            string[,] list = new string[,] {
                {"Title",path}, {"startText",path},
                {"operateText",path}, {"staffText",path},
                {"Clear",path}, {"Ending",path},
                {"Operate1",path}, {"Operate2",path},
                {"staffRoll",path},{ "fadeImage",path},
                {"keyboard",path}, {"player",path},
                {"effect",path}, {"effect2",path},
                {"gauge",path}, {"fireball",path},
                {"gimmick",path}, {"stage1_background",path},
                {"gimmickMap",path},{"MapChip",path},
                {"stage1_background_A1",path}, {"stage1_background_A2",path},
                {"stage3_background_A1",path}, {"stage3_background_A2",path},
                {"stage1_background_RA_1",path}, {"stage1_background_RA_1_2",path},
                {"stage1_background_RA_2",path}, {"stage1_background_RA_2_2",path},
                {"beam",path}, {"bullet",path},
                {"shell",path}, {"enemy_slime_fire",path},
                {"enemy_slime_fire2",path}, {"enemy_seed",path},
                {"enemy_seed2",path}, {"Battery",path},
                {"Battery2",path}, {"Ranking1",path},
                {"Ranking2",path}, {"RankInPut1",path},
                {"RankInPut2",path}, {"bar",path},
                {"cursor",path}, { "Pause",path},
                { "Signboard",path},
            };

            return list;
        }

        private string[,] BGMList() {
            string path = "./MP3/";

            string[,] list = new string[,] {
                {"title",path}, {"clear",path},
                {"stage1",path}, {"operate",path},
                {"stage3",path}, {"gameover",path},
                {"staffroll",path}, {"stage2",path},
            };

            return list;
        }

        private string[,] SEList() {
            string path = "./WAV/";

            string[,] list = new string[,] {
                {"beam",path}, {"bullet",path},
                {"cancel",path}, {"jump",path},
                {"laserburn",path}, {"playerAttack",path},
                {"gameclear",path}, {"checkpoint",path},
                {"playerDamege",path}, {"playerDead",path},
                {"fire",path}, {"itemGet",path},
                {"enemyDead",path}, {"crumble",path},
            };

            return list;
        }

        public Loading(GameDevice gameDevice) {
            renderer = gameDevice.GetRenderer;
            sound = gameDevice.GetSound;

            textureLoader = new TextrueLoader(renderer, TextureList());
            bgmLoader = new BGMLoader(sound, BGMList());
            seLoader = new SELoader(sound, SEList());
        }

        public void Initialize() {
            isEnd = false;
            textureLoader.Initialize();
            bgmLoader.Initialize();
            seLoader.Initialize();
            totalResouceNum = textureLoader.Count + bgmLoader.Count + seLoader.Count;

            motion = new Motion();

            for (int i = 0; i < 4; i++) {
                motion.Add(i, new Rectangle(i * 128, 0, 128, 128));
            }

            motion.Initialize(new Range(0, 3), new Timer(0.2f));
            

        }
         
        public void Update() {
            //sound.PlayBGM("title");
            if (!textureLoader.IsEnd) { textureLoader.Update(); }
            else if (!bgmLoader.IsEnd) { bgmLoader.Update(); }
            else if (!seLoader.IsEnd) { seLoader.Update(); }

            if (seLoader.IsEnd) { isEnd = true; }
            motion.Update();
        }

        public void Draw(Renderer renderer) {
            renderer.DrawTexture("Loading", Vector2.Zero);
            int currentCount = textureLoader.CurrentCount + seLoader.CurrentCount + bgmLoader.CurrentCount;
            int pasent = (int)(currentCount / (float)totalResouceNum * 100f);
            int currentPasent = 0;
            Vector2 pasentPosition = new Vector2(27, 671);

            for (int j = 0; j < 970; j++) {
                renderer.DrawTexture("loadBar", pasentPosition);
                if (currentPasent >= 970 * pasent / 100) { break; }
                pasentPosition.X++;
                currentPasent++;
            }

            if (totalResouceNum != 0) {
                renderer.DrawNumber("number", new Vector2(850, 600),
                    pasent, 1.5f);
            }

            renderer.DrawTexture("player2", new Vector2(200, 230), motion.DrawRange_R());
            
        }
        public void Shutdown() { }

        public bool IsEnd() { return isEnd; }
        public Scene Next() {
            return Scene.TITLE; 
        }
    }
}
