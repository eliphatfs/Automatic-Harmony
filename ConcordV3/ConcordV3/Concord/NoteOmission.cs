using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord
{
    public class NoteOmission
    {
        // Store the Omitted Notes In Case There Are Not Enough Notes
        List<int> omitted = new List<int>();
        public void Omit(bool[] toOmit, List<List<int>> previous, int root, int count)
        {
            omitted.Clear();
            // Omit some Low Notes as to Make Loose the Lower Parts
            for (int i = root + 1; i < root + 9; i++)
                if (toOmit[i]) omitOne(toOmit, i);
            // Omit Pure 5th's
            for (int i = toOmit.Length - 7; i > root; i--)
            {
                if (toOmit[i] && toOmit[i + 7])
                {
                    omitOne(toOmit, i + 7);
                    // else omitOne(toOmit, i + 7);
                }
            }
            // Omit Pure 8th's
            for (int i = root + 1; i < toOmit.Length - 12; i++)
            {
                if (toOmit[i] && toOmit[i + 12])
                {
                    omitOne(toOmit, i);
                    // else omitOne(toOmit, i + 12);
                }
            }

            // Count Notes Left after Omission
            int notesLeft = 0;
            for (int i = 0; i < toOmit.Length; i++)
                if (toOmit[i]) notesLeft++;

            // Randomly Choose Omitted Notes to Match Wanted Count
            while (notesLeft < count)
            {
                int disomit = Helper.Rand.Next(omitted.Count);
                toOmit[omitted[disomit]] = true;
                omitted.RemoveAt(disomit);
                notesLeft++;
            }

            // Avoid Parallel 5th's and 8th's
            if (previous.Count > 0)
            {
                int noteid = 0;
                for (int i = 0; i < toOmit.Length - 12; i++)
                {
                    if (toOmit[i]) noteid++;
                    if (toOmit[i] && toOmit[i + 7])
                    {
                        if (previous.Last().Count > noteid + 1)
                            if (previous.Last()[noteid + 1] - previous.Last()[noteid] == 7)
                                if (Helper.RandomBoolean()) omitOne(toOmit, i);
                                else omitOne(toOmit, i + 7);
                    }
                    if (toOmit[i] && toOmit[i + 12])
                    {
                        if (previous.Last().Count > noteid + 1)
                            if (previous.Last()[noteid + 1] - previous.Last()[noteid] == 12)
                                if (Helper.RandomBoolean()) omitOne(toOmit, i);
                                else omitOne(toOmit, i + 12);
                    }
                }
            }
        }

        void omitOne(bool[] toModify, int toOmit)
        {
            toModify[toOmit] = false;
            omitted.Add(toOmit);
        }
    }
}
