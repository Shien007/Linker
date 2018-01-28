using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FairyLink.Device;
using FairyLink.Actor;
using FairyLink.Def;
using FairyLink.Utility;

namespace FairyLink.Scene
{
    class GamePlay : IScene
    {
        private Renderer renderer;      //描画用
        private Sound sound;
        private InputState inputState;
        private Collision collision;      //あたり判定用
        private Stage stage;
        private Gimmick gimmick;      //チェックポイントなど
        private Character player;
        private CharacterControl characterControl;      //チャラをまとめ管理
        private EffectControl effectControl;      //エフェクトをまとめ管理
        private PieceControl pieceControl;      //分割されたオブジェクトを管理
        private StageLoader stageLoader;      //stageNumberによって、stagemapをロード
        private Rankings rankings;
        private PauseSelect pauseSelect;
        private StageFade stageFade;
        private BackgroundDraw backgroundDraw;

        private int stageNumber;
        private int playTime;
        private bool isEnd;
        private bool isPause;


        public GamePlay(GameDevice gameDevice) {
            renderer = gameDevice.GetRenderer;
            sound = gameDevice.GetSound;
            inputState = gameDevice.GetInputState;
            rankings = gameDevice.GetRankings;
            stageLoader = new StageLoader();
            pieceControl = new PieceControl();
            stage = new Stage(stageLoader, pieceControl, sound);
            effectControl = new EffectControl(stage);
            gimmick = new Gimmick(stage, stageLoader, pieceControl, gameDevice);
            characterControl = new CharacterControl(stage, effectControl, pieceControl, gameDevice, gimmick);
            player = new Player(stage, gimmick, characterControl, effectControl, gameDevice);
            collision = new Collision(sound, player, characterControl);
            pauseSelect = new PauseSelect(inputState);
            stageFade = new StageFade();
            backgroundDraw = new BackgroundDraw();
            stageNumber = 1;
        }
        
        public void Initialize() {
            if (stageNumber == 1) { playTime = 0; }
            characterControl.AddCharacter(player);
            isEnd = false;
            isPause = false;
            stage.SetStageNumber(stageNumber);
            gimmick.SetStageNumber(stageNumber);
            stage.Initialize();
            gimmick.Initialize();
            characterControl.Initialize();
            effectControl.Initialize();
            rankings.Initialize();
            pauseSelect.Initialize();
        }


        public void Update() {
            if (inputState.WasDown(Keys.Q)) {
                isPause = !isPause;
                pauseSelect.Initialize();
            }

            if (isPause) {
                Pause();
                return;
            }

            if (stageFade.GetFadeSwitch == FadeSwitch.On)
            {
                stageFade.Update();
            }

            playTime++;
            backgroundDraw.Update();
            sound.PlayBGM("stage" + stageNumber);            

            if (characterControl.IsPlayerDead()) {
                isEnd = true;
                return;
            }
            else if (characterControl.IsGameClear()) {
                
                if (stageNumber >= 3) {
                    sound.PlaySE("gameclear");
                    isEnd = true;
                    return;
                }
                stageFade.GetFadeSwitch = FadeSwitch.On;
                if (stageFade.IsEnd) {
                    stageNumber++;
                    Initialize();;
                    sound.PlaySE("gameclear");
                }
                return;
            }

            characterControl.Update();
            effectControl.Update();
            pieceControl.Update();
            collision.Update();
            gimmick.Update();
        }

        public void Pause() {
            pauseSelect.Update();

            if (inputState.WasDown(Keys.Enter) ||
            inputState.WasDown(Buttons.A) ||
            inputState.WasDown(Buttons.Start))
            {
                sound.PlaySE("beam");
                pauseSelect.IsSelected = true;
            }

            if (pauseSelect.IsSelectEnd)
            {
                switch (pauseSelect.GetSelect)
                {
                    case 1:
                        isEnd = true;
                        stageNumber = 1;
                        break;
                    case 2:
                        Initialize();
                        break;
                    case 3:
                        isPause = !isPause;
                        pauseSelect.Initialize();
                        break;
                }
            }
        }

        public void Draw(Renderer renderer) {
            backgroundDraw.Draw(renderer, stageNumber, stage.GetTrember);
            gimmick.Draw(renderer);
            stage.Draw(renderer);
            characterControl.Draw(renderer);
            pieceControl.Draw(renderer);
            stageFade.Draw(renderer);

            if (isPause) {
                renderer.DrawTexture("Pause", Vector2.Zero);
                pauseSelect.Draw(renderer);
            }
        }

        public void Shutdown() { }

        public bool IsEnd() { return isEnd; }
        public Scene Next() {
            if (isPause) { return Scene.TITLE; }
            else if (characterControl.IsGameClear()) {
                rankings.DeathCount = ((Player)player).DeathCount;
                rankings.PlayTime = playTime / 60;

                stageNumber = 1;

                return Scene.CLEAR;
            }
            rankings.DeathCount = ((Player)player).DeathCount;
            rankings.PlayTime = playTime / 60;
            
            stageNumber = 1;
            return Scene.ENDING; 
        }

    }
}
