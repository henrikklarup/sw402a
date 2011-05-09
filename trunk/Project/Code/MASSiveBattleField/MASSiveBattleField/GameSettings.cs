using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MASSiveBattleField
{
    public partial class GameSettings : Form
    {

        //Small = 13
        //Medium = 26
        //Large = 46

        public int gridSize = 26;

        public GameSettings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            if (radioButton1.Checked)
                gridSize = 13;
            else if (radioButton2.Checked)
                gridSize = 26;
            else if (radioButton3.Checked)
                gridSize = 46;
        }

        private void GameSettings_Load(object sender, EventArgs e)
        {
            radioButton2.Checked = true;
        }
    }
}
