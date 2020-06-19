using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core
{
    public sealed class SoundGenerator
    {
        public static void PlaySine(float frequency, float duration)
        {
            SignalGenerator generator = new SignalGenerator(44800, 2) { Frequency = frequency, Gain = 0.15, Type = SignalGeneratorType.Sin };
            var samples = generator.Take(TimeSpan.FromSeconds(duration));
            using (var wo = new WaveOutEvent())
            {
                wo.Init(samples);
                wo.Play();
                while (wo.PlaybackState == PlaybackState.Playing)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }
        }

        public static void BeepMorse(string code)
        {
            foreach (var c in code)
            {
                switch (c)
                {
                    case '-':
                        PlaySine(1000, 0.2f);
                        break;
                    case '.':
                        PlaySine(1000, 0.1f);
                        break;
                }
            }
        }
    }
}
