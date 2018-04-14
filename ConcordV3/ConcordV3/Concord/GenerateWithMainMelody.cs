using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConcordV3.Concord.Regeneration;

namespace ConcordV3.Concord
{
    public class GenerateWithMainMelody
    {
        double happy = 1;
        double sad = 0;
        public List<int>[] Apply(int[] pitches, int[] time)
        {
            List<List<int>> result = new List<List<int>>();
            //List<List<int>> identity = new List<List<int>>();
            List<string> progress = new List<string>();
            int tMelody = 0;
            int timePt = -1;
            int totalTime = time.Sum();

            int nextProgressCD = 0, breakChordPoint = -1;

            for (int t = 0; t < totalTime; t++)
            {
                if (nextProgressCD > 0 && timePt < pitches.Length - 2)
                {
                    nextProgressCD--;
                    result.Add(new List<int>());
                    tMelody++;
                    if (timePt == -1 || tMelody >= time[timePt])
                    {
                        timePt++;
                        tMelody = 0;
                    }
                    result[result.Count - 1].Add(pitches[timePt]);
                    if (nextProgressCD == 0)
                    {
                        Expansion.BreakChordsTimely(result, breakChordPoint, t - breakChordPoint);
                    }
                    continue;
                }
                bool hasAdded = false;
                tMelody++;
                if (timePt == -1 || tMelody >= time[timePt])
                {
                    timePt++;
                    tMelody = 0;
                    if (timePt == pitches.Length - 2)
                    {
                        string next = "G";
                        progress.Add(next);
                        result.Add(MakeChord(next, result, pitches[timePt]));
                        hasAdded = true;
                    }
                    else if (timePt == pitches.Length - 1)
                    {
                        string next = "C";
                        progress.Add(next);
                        result.Add(MakeChord(next, result, pitches[timePt]));
                        hasAdded = true;
                    }
                    else
                    {
                        ChordRegen r = new ChordRegen();
                        string next = happy >= sad ? Progress1(progress, pitches[timePt], time[timePt]).Item1 : Progress2(pitches[timePt]);
                        if (r.Regen(next)[pitches[timePt]] || (next.Length == 1 && Helper.RandomBoolean(happy)) || (next.Length == 2 && Helper.RandomBoolean(sad)))
                        {
                            progress.Add(next);
                            ModifyHappiness(ref next);
                            result.Add(MakeChord(next, result, pitches[timePt]));
                            if (sad > happy)
                            {
                                nextProgressCD = 6;
                                breakChordPoint = t;
                            }
                            hasAdded = true;
                        }
                    }
                }
                else if (Helper.RandomBoolean(happy) && timePt < pitches.Length - 2)
                {
                    ChordRegen r = new ChordRegen();
                    string next = Progress1(progress, pitches[timePt], time[timePt]).Item1;
                    if (r.Regen(next)[pitches[timePt]])
                    {
                        progress.Add(next);
                        ModifyHappiness(ref next);
                        result.Add(MakeChord(next, result, pitches[timePt]));
                        hasAdded = true;
                    }
                    else
                    {
                        next = Progress2(pitches[timePt]);
                        progress.Add(next);
                        ModifyHappiness(ref next);
                        result.Add(MakeChord(next, result, pitches[timePt]));
                        hasAdded = true;
                    }
                }
                if (!hasAdded)
                {
                    List<int> chord = new List<int>(result.Last());
                    chord.Sort();
                    chord[chord.Count - 1] = pitches[timePt];
                    result.Add(chord);
                }
            }
            System.Windows.Forms.MessageBox.Show(string.Join("-", progress));
            return result.ToArray();
        }

        public List<int> MakeChord(string name, List<List<int>> prev, int pitch)
        {
            ChordRegen r = new ChordRegen();
            NoteOmission o = new NoteOmission();
            bool[] full = r.Regen(name, highlimit: pitch);
            o.Omit(full, prev, NoteNotions.GetPlaceInOctave(name), happy > sad ? 3 : 4);
            full[pitch] = true;
            List<int> chord = new List<int>();
            for (int i = 0; i < full.Length; i++)
                if (full[i]) chord.Add(i);
            return chord;
        }

        public Tuple<string, int> Progress1(List<string> previous, int pitch, int time)
        {
            if (previous.Count == 0)
                return Tuple.Create("C", time);
            ChordRegen r = new ChordRegen();
            string last1 = previous.Last();
            if (previous.Count == 1)
            {
                if (last1 == "C")
                {
                    if (r.Regen("G")[pitch])
                        return Tuple.Create("G", time);
                    if (r.Regen("Am")[pitch])
                        return Tuple.Create("Am", time);
                }
                return Tuple.Create("F", (int)(time * 0.9f));
            }
            string last2 = previous[previous.Count - 2];
            if (last2 == "G" && last1 == "Am")
            {
                if (r.Regen("G")[pitch])
                    return Tuple.Create("G", time);
                if (r.Regen("Em")[pitch])
                    return Tuple.Create("Em", time);
                if (r.Regen("C")[pitch])
                    return Tuple.Create("C", time);
            }
            else if (last2 == "Am")
            {
                if (last1 == "F")
                {
                    if (r.Regen("G")[pitch])
                        return Tuple.Create("G", time);
                }
                else
                {
                    if (r.Regen("F")[pitch])
                        return Tuple.Create("F", time);
                }
            }
            else if (last1 == "F")
            {
                if ("G,Em,C".Contains(last2))
                {
                    if (r.Regen("Em")[pitch])
                        return Tuple.Create("Em", time);
                    if (r.Regen("C")[pitch])
                        return Tuple.Create("C", time);
                }
                if ("C,Em".Contains(last2))
                {
                    if (r.Regen("G")[pitch])
                        return Tuple.Create("G", time);
                }
            }
            else if (last1 == "Dm" && "C,Em".Contains(last2))
            {
                if (r.Regen("G")[pitch])
                    return Tuple.Create("G", time);
            }
            if (!last1.StartsWith("G") && !last1.StartsWith("F"))
                return Tuple.Create("F", (int)(time * 0.5f));
            else if (last1.StartsWith("F"))
                return Tuple.Create("G", (int)(time * 0.5f));
            else
                return Tuple.Create("C", time);
        }

        static readonly string[] naturalMajor = { "C", "Dm", "Em", "F", "G", "Am" };
        static readonly string[] naturalMinor = { "Am", "Dm", "C", "Dm", "Em", "F" };
        public string Progress2(int pitch)
        {
            ChordRegen r = new ChordRegen();
            if (Helper.RandomBoolean(happy > sad ? 0.8f : 0.2f))
            {
                for (int i = 0; i < 6; i++)
                    if (naturalMajor[i].Length == 1 && r.Regen(naturalMajor[i])[pitch])
                        return naturalMajor[i];
            }
            else
            {
                for (int i = 0; i < 6; i++)
                    if (naturalMajor[i].Length == 2 && r.Regen(naturalMajor[i])[pitch])
                        return naturalMajor[i];
            }
            return "G7";
        }

        public void ModifyHappiness(ref string chord)
        {
            if (sad > happy)
            {
                if ((Helper.RandomBoolean(sad - happy) && chord.Length == 1))
                {
                    for (int i = 0; i < 6; i++)
                        if (naturalMajor[i] == chord)
                            chord = naturalMinor[i];
                }
            }
        }
    }
}
