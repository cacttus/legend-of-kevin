using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class Audio
    {
        public enum MusicStateE { Play, Pause };
        public MusicStateE MusicState { get; private set; } = MusicStateE.Play;
        public List<SoundEffect> Sounds { get; set; } = new List<SoundEffect>();
        public List<Song> Songs { get; set; } = new List<Song>();
        Song _playing = null;
        public bool Enabled { get; set; } = true;

        string CurrentSong()
        {
            if (_playing != null)
            {
                return _playing.Name;
            }
            else
            {
                return "";
            }
        }
        public void PauseMusic()
        {
            MediaPlayer.Pause();
            MusicState = MusicStateE.Pause;
        }
        public void ResumeMusic()
        {
            if (_playing != null)
            {
                if (MusicState == MusicStateE.Play)
                {
                    return;
                }
                else
                {
                    MediaPlayer.Resume();
                    MusicState = MusicStateE.Play;
                }
            }
        }
        public void PlayMusic(string name)
        {
            //this keeps the pause state, because the player may change levels
            //so we change tracks, but we keep the track paused if the player muted it
            _playing = Songs.Find(x => x.Name == name);
            if (_playing != null)
            {
                MediaPlayer.Play(_playing);
                MediaPlayer.IsRepeating = true;
                if (MusicState == MusicStateE.Pause)
                {
                    PauseMusic();
                }
            }
            else
            {
                MediaPlayer.Stop();
            }
        }
        public SoundEffectInstance PlaySound(string name)
        {
            if (Enabled == false)
            {
                return null;
            }
            SoundEffect s = Sounds.Find(x => x.Name == name);
            if (s != null)
            {
                SoundEffectInstance inst = s.CreateInstance();
                try
                {
                    inst.Play();
                }
                catch(Exception ex)
                {
                    //this.
                    Globals.IgnoreException(ex);
                }
                return inst;
            }
            return null;
        }
        public void PlaySound(List<string> randomList)
        {
            if (Enabled == false)
            {
                return;
            }
            int s = Globals.RandomInt(0, randomList.Count);
            if (s >= 0 && s < randomList.Count)
            {
                PlaySound(randomList[s]);
            }
        }
        //Sound PlayMusic(string name)
        //{
        //    Sound s = Sounds.Find(x => x.Name == name);

        //    s.SoundEffect.Play();

        //    return s;
        //}
    }

}
