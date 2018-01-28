using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using FairyLink.Def;
using FairyLink.Utility;

namespace FairyLink.Device
{
    class GameDevice
    {
        private Renderer renderer;          //描画用
        private InputState inputState;          //入力用
        private Sound sound;
        private GraphicsDevice graphics;
        private Rankings rankings;

        public GameDevice(ContentManager content, GraphicsDevice graphics) {
            renderer = new Renderer(content, graphics);
            rankings = new Rankings();
            inputState = new InputState();
            sound = new Sound(content);
            this.graphics = graphics;
        }

        //リソースを登録する
        public void Initialize() {
            renderer.LoadTextures("number");
            renderer.LoadTextures("Loading");
            renderer.LoadTextures("player2");
            NewPixel();
        }

        public void NewPixel() {
            //エフェクト用絵を作り出す

            //レーザーシュート
            //画像のサイズを決めて変数を作る
            Texture2D laser = new Texture2D(graphics, 1, 206);
            //サイズを決めてカラーデータの変数を作る
            Color[] lasercolor = new Color[laser.Width * laser.Height];
            //カラー変数にピクセルずつ色を埋める
            for (int i = 0; i < laser.Height; i++)
            {
                if (i < 41) { lasercolor[i] = new Color(150, 150, 255); }
                else if (i < 71) { lasercolor[i] = new Color(150, 200, 255); }

                else if (i > 166) { lasercolor[i] = new Color(150, 150, 255); }
                else if (i > 136) { lasercolor[i] = new Color(150, 200, 255); }
                else { lasercolor[i] = new Color(255, 255, 255); }
            }
            //カラーを画像変数の中に設置する
            laser.SetData(lasercolor);
            //描画用クラスに登録する
            renderer.LoadTexture("laser", laser);


            //レーザースタート
            Texture2D laserStart = new Texture2D(graphics, 1, 20);
            Color[] laserStartColor = new Color[laserStart.Width * laserStart.Height];
            for (int i = 0; i < laserStart.Height; i++)
            {
                if (i < 5) { laserStartColor[i] = new Color(150, 150, 255); }
                else if (i > 15) { laserStartColor[i] = new Color(150, 150, 255); }
                else { laserStartColor[i] = new Color(255, 255, 255); }
            }
            laserStart.SetData(laserStartColor);
            renderer.LoadTexture("laserStart", laserStart);

            //電気エフェクト用
            Texture2D electric = new Texture2D(graphics, 1, 2);
            Color[] electricColor = new Color[1 * 2];
            electricColor[0] = new Color(255, 255, 0);
            electricColor[1] = new Color(255, 255, 0);
            electric.SetData(electricColor);
            renderer.LoadTexture("electric", electric);


            //フェードイン・フェードアウト用
            Texture2D fade = new Texture2D(graphics, Screen.Width, Screen.Height);
            Color[] fadeColor = new Color[Screen.Width * Screen.Height];
            int index = 0;
            for (int i = 0; i < Screen.Height; i++)
            {
                for (int j = 0; j < Screen.Width; j++)
                {
                    fadeColor[index] = new Color(0, 0, 0);
                    index++;
                }
            }
            fade.SetData(fadeColor);
            renderer.LoadTexture("fade", fade);



            //ロード用
            Texture2D loadBar = new Texture2D(graphics, 1, 32);
            Color[] loadBarColor = new Color[1 * 32];
            for (int i = 0; i < loadBar.Height; i++)
            {
                if (i == 9 || i == 10 || i == 11) { loadBarColor[i] = new Color(255, 255, 255); }
                else { loadBarColor[i] = new Color(255, 255, 0); }
            }
            loadBar.SetData(loadBarColor);
            renderer.LoadTexture("loadBar", loadBar);

        }



        public void Update() { inputState.Update(); }

        public void UnLoad() {
            sound.Unload();
            renderer.UnLoad();
            
        }

        public InputState GetInputState {
            get { return inputState; }
        }

        public Renderer GetRenderer {
            get { return renderer; }
        }

        public Rankings GetRankings {
            get { return rankings; }
        }

        public Sound GetSound {
            get { return sound; }
        }
    }
}
