using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GameEngine.UI
{
    public class MML
    {
        private MMLNote[][] channels;

        public IEnumerable<MMLNote[]> Channels => channels;

        public MML(params string[] mml)
        {

            this.channels = mml.Select(ParseTrack).ToArray();
        }

        private MMLNote[] ParseTrack(string track)
        {
            int octave = 4;
            List<MMLNote> notes = new List<MMLNote>();
            Regex matcher = new Regex("(?<note>[a-grA-GR][+#-]?[0-9]*\\.?(&[a-grA-GR][+#-]?[0-9]*\\.?)*)|(?<octave>[<>]|o[1-8])|(?<length>l[0-9]+\\.?)|(?<tempo>t[0-9]+\\.?)");
            int l = 1;
            int t = 60;
            foreach (Match match in matcher.Matches(track))
            {
                if (match.Groups["note"].Success)
                {
                    notes.Add(new MMLNote(match.Value, octave, l, t));
                }
                else if (match.Groups["length"].Success)
                {
                    l = int.Parse(string.Join("", match.Groups["length"].Value.Skip(1)));
                }
                else if (match.Groups["tempo"].Success)
                {
                    t = int.Parse(string.Join("", match.Groups["tempo"].Value.Skip(1)));
                }
                else if (match.Groups["octave"].Success)
                {
                    string val = match.Groups["octave"].Value;
                    if (val == "<")
                    {
                        octave--;
                        if (octave < 1)
                        {
                            octave = 1;
                        }
                    }
                    else if (val == ">")
                    {
                        octave++;
                        if (octave > 8)
                        {
                            octave = 8;
                        }
                    }
                    else
                    {
                        octave = int.Parse(string.Join("", val.Skip(1)));
                    }
                }
                else
                {

                }
            }

            return notes.ToArray();
        }

        public MMLNote[] GetChannel(int i)
        {
            return channels[i];
        }
    }

    public class MMLNote
    {
        private string note;
        private int octave;
        private float duration;
        private int tempo;

        public MMLNote(string note, int octave, float defaultDuration, int tempo)
        {
            this.note = "" + note.First();
            this.octave = octave;
            this.tempo = tempo;
            int skip = 1;
            if (note.Length > 1)
            {
                if (note.Skip(skip).First() == '-')
                {
                    skip++;
                }
                else if (note.Skip(skip).First() == '+' || note.Skip(skip).First() == '#')
                {
                    note = note.Replace("#","+");
                    skip++;
                }

                string[] durs = note.Split('&');
                if (durs.Length > 1)
                {

                }
                foreach (string d in durs)
                {
                    string dur = d;
                    bool dotted = dur.Contains(".");
                    if (dotted)
                    {
                        dur = dur.Remove(note.Length - 1);
                    }
                    if (dur.Length > skip)
                    {
                        this.duration += 1.0f / int.Parse(string.Join("", dur.Skip(skip)));
                        if (dotted)
                        {
                            this.duration += this.duration + this.duration / 2.0f;
                        }
                    }
                    else
                    {
                        this.duration += 1.0f / defaultDuration;
                    }
                }
            }
            else
            {
                this.duration = 1.0f / defaultDuration;
            }
        }

        public float GetDuration()
        {
            return duration * 4 * 60.0f / tempo;
        }

        public float GetTone()
        {
            int shift = octave - 4;
            int tone;
            switch (this.note)
            {
                case "c-":
                    tone = 247;
                    break;
                case "c":
                    tone = 262;
                    break;
                case "c+":
                    tone = 277;
                    break;
                case "d-":
                    tone = 277;
                    break;
                case "d":
                    tone = 294;
                    break;
                case "d+":
                    tone = 311;
                    break;
                case "e-":
                    tone = 311;
                    break;
                case "e":
                    tone = 330;
                    break;
                case "e+":
                    tone = 349;
                    break;
                case "f-":
                    tone = 330;
                    break;
                case "f":
                    tone = 349;
                    break;
                case "f+":
                    tone = 370;
                    break;
                case "g-":
                    tone = 370;
                    break;
                case "g":
                    tone = 392;
                    break;
                case "g+":
                    tone = 415;
                    break;
                case "a-":
                    tone = 415;
                    break;
                case "a":
                    tone = 440;
                    break;
                case "a+":
                    tone = 466;
                    break;
                case "b-":
                    tone = 466;
                    break;
                case "b":
                    tone = 494;
                    break;
                case "b+":
                    tone = 523;
                    break;
                default:
                    tone = 0;
                    break;
            }

            if (shift > 0)
            {
                tone = tone << shift;
            }
            else if (shift < 0)
            {
                tone = tone >> (4 - octave);
            }

            return tone;
        }
    }
}
