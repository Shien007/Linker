using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FairyLink.Def;

namespace FairyLink.Device
{
    class Renderer
    {
        private ContentManager contentManager;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        private Dictionary<string, Texture2D> textures;

        public Renderer(ContentManager content, GraphicsDevice graphics) {
            contentManager = content;
            graphicsDevice = graphics;
            spriteBatch = new SpriteBatch(graphicsDevice);
            textures = new Dictionary<string, Texture2D>();
        }

        public void LoadTexture(string name,Texture2D texture) {
            textures.Add(name, texture);
        }

        public void LoadTextures(string name, string filepath = "./IMAGE/") {
            textures.Add(name, contentManager.Load<Texture2D>(filepath + name));
        }

        public void UnLoad() {
            textures.Clear();
        }

        public void Begin() {
            spriteBatch.Begin();
        }

        public void End() {
            spriteBatch.End();
        }

        public void DrawTexture(string name, Vector2 position) {
            if (!textures.ContainsKey(name)) { return; }
            spriteBatch.Draw(textures[name], position, Color.White);
        }

        public void DrawTexture(string name, Vector2 position, float alpha = 1.0f) {
            if (!textures.ContainsKey(name)) { return; }
            spriteBatch.Draw(textures[name], position, Color.White * alpha);
        }


        public void DrawTexture(string name, Vector2 position, Rectangle rect) {
            if (!textures.ContainsKey(name)) { return; }
            spriteBatch.Draw(textures[name], position, rect, Color.White);
        }



        public void DrawTexture(string name, Vector2 position, Rectangle rect, float alpha = 1.0f) {
            if (!textures.ContainsKey(name)) { return; }
            spriteBatch.Draw(textures[name], position, rect, Color.White * alpha);
        }


        public void DrawTexture(string name, Vector2 position, float alpha, Rectangle rect, float size, float rocate, Vector2 origin)
        {
            if (!textures.ContainsKey(name)) { return; }
            spriteBatch.Draw(textures[name], position, rect, Color.White * alpha, rocate, origin, size,SpriteEffects.None,1.0f);
        }

        public void DrawNumber(string name, Vector2 position, int num, float scale = 1f) {
            if (!textures.ContainsKey(name)) { return; }
            foreach (var n in num.ToString()) {
                spriteBatch.Draw(textures[name], position,
                    new Rectangle((n - '0') * 16, 0, 16, 32), Color.White
                    , 0f,Vector2.Zero, scale, SpriteEffects.None, 0);
                position.X += 16 * scale;
            }
        }

    }
}
