using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;
namespace AMSnake
{
    public static class Audio
    {
        public readonly static MediaPlayer GameOver = LoadAudio("GameOver.wav",0.4);
        public readonly static MediaPlayer Background1 = LoadAudio("Jingle-Bells-Inst.mp3", 1,true,false);
        public readonly static MediaPlayer Background2 = LoadAudio("Rudolf.mp3", 1,true, false);

        public readonly static List<MediaPlayer> BGMusic = new()
        {
            Background1, Background2
        };

        private static MediaPlayer LoadAudio(string filename, double volume = 1, bool repeat = false, bool autoReset = true)
        {
            MediaPlayer player = new();
            player.Open(new Uri($"Assets/{filename}", UriKind.Relative));
            player.Volume = volume;

            if (autoReset)
            {
                player.MediaEnded += Player_MediaEnded;
            }
            
            if (repeat)
            {
                player.MediaEnded -= Player_MediaEnded;
            }
            return player;
        }

        private static void Player_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Stop();
            m.Position = new TimeSpan(0);

        }

        private static void PlayerRepeat_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Position = new TimeSpan(0);
            m.Play();
        }
    }
}
