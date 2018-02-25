using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordNew.Concord.DataStructure
{
    class Score
    {
        public const int MAXN = 1024;
        private Chord[] _sequence;
        public Chord this[int index]
        {
            get
            {
                return _sequence[index];
            }
            set
            {
                _sequence[index] = value;
            }
        }
        public Score()
        {
            _sequence = new Chord[MAXN];
            for (int i = 0; i < _sequence.Length; i++)
                _sequence[i] = new Chord();
        }
        public Score(int[] MainMelody)
        {
            _sequence = new Chord[MAXN];
            for (int i = 0; i < _sequence.Length; i++)
                _sequence[i] = new Chord();
            for (int i = 0; i < MainMelody.Length; i++)
                _sequence[i].Add(MainMelody[i]);
        }
        public Score SetData(Chord[] chords)
        {
            _sequence = new Chord[MAXN];
            for (int i = chords.Length; i < _sequence.Length; i++)
                _sequence[i] = new Chord();
            for (int i = 0; i < chords.Length; i++)
                _sequence[i] = chords[i];
            return this;
        }
    }
}
