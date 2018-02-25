using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConcordV3
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
        }

        private void buttonTestLittleStar_Click(object sender, EventArgs e)
        {
            var gwmm = new Concord.GenerateWithMainMelody();
            var pitch = new int[] {
                    60, 60, 67, 67, 69, 69, 67, 65, 65, 64, 64, 62, 62, 60,
                    67, 67, 65, 65, 64, 64, 62, 67, 67, 65, 65, 64, 64, 62,
                    60, 60, 67, 67, 69, 69, 67, 65, 65, 64, 64, 62, 62, 60
                };
            var time = new int[] {
                    2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 4,
                    2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 4,
                    2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 4
                };
            for (int i = 0; i < time.Length; i++)
            {
                pitch[i] += 12;
            }
            var result = gwmm.Apply(
                pitch,
                time
            );
            textBox1.Text = Midi.OutputMidi.Score2String(result);
        }
    }
}
