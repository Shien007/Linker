using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;    //wav
using Microsoft.Xna.Framework.Media;    //mp3

namespace FairyLink.Device
{
    class Sound
    {
        private ContentManager contentManager;

        private Dictionary<string, Song> bgms;  //MP3管理用
        private Dictionary<string, SoundEffect> soundEffect;    //WAC管理用
        private Dictionary<string, SoundEffectInstance> seInstances;    //WAVインスタンス管理用
        private List<SoundEffectInstance> sePlayList;   //WAVインスタンスの再生リスト
        
        private string currentBGM;  //現在再生中のアセット名

        public Sound(ContentManager content) {
            contentManager = content;
            MediaPlayer.IsRepeating = true; //mp3の再生を循環する

            bgms = new Dictionary<string, Song>();
            soundEffect = new Dictionary<string, SoundEffect>();
            seInstances = new Dictionary<string, SoundEffectInstance>();
            sePlayList = new List<SoundEffectInstance>();

            currentBGM = null;
        }

        #region BGM関連


        /// <summary>
        /// BGMファイルを読み取る
        /// </summary>
        /// <param name="name">アセット名</param>
        /// <param name="filepath">保存アドレス</param>
        public void LoadBGM(string name, string filepath = "./MP3/") {
            if (bgms.ContainsKey(name)) { return; }
            bgms.Add(name, contentManager.Load<Song>(filepath + name));
        }

        public void StopBGM() {
            MediaPlayer.Stop();
            currentBGM = null;
        }

        public void PlayBGM(string name) {
            if (currentBGM == name) { return; }
            if (IsPlayingBGM())
            {
                StopBGM();
            }
            MediaPlayer.Volume = 0.5f;
            currentBGM = name;
            MediaPlayer.Play(bgms[currentBGM]);
        }

        public void ChangeBGMLoopFlag(bool loopFlag) {
            MediaPlayer.IsRepeating = loopFlag;
        }

        public bool IsStoppedBGM() {
            return (MediaPlayer.State == MediaState.Stopped);
        }

        public bool IsPausedBGM() {
            return (MediaPlayer.State == MediaState.Paused);
        }

        public bool IsPlayingBGM() {
            return (MediaPlayer.State == MediaState.Playing);
        }

	    #endregion

        #region WAV関連

        public void LoadSE(string name, string filepath = "./WAV/") {
            if (soundEffect.ContainsKey(name)) { return; }
            soundEffect.Add(name, contentManager.Load<SoundEffect>(filepath + name));
        }

        public void CreateSEInstance(string name) {
            if (seInstances.ContainsKey(name)) { return; }
            seInstances.Add(name, soundEffect[name].CreateInstance());
        }

        public void PlaySE(string name) {
            soundEffect[name].Play();
        }

        public void PlaySEInstance(string name, bool loopFlag = false) {
            var data = seInstances[name];
            data.IsLooped = loopFlag;
            data.Play();
            sePlayList.Add(data);
        }

        public void PausedSE(string name) {
            foreach (var se in sePlayList) {
                if (se.State == SoundState.Playing) { se.Stop(); }
            }
        }

        public void RemoveSE() {
            sePlayList.RemoveAll(se => se.State == SoundState.Stopped);
        }

        #endregion

        /// <summary>
        /// 使ったlistとdictionaryを初期化する
        /// </summary>
        public void Unload() {
            bgms.Clear();
            soundEffect.Clear();
            sePlayList.Clear();
        }




    }
}
