using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Eindopdracht
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {


            InitializeComponent();
            //ivm scaling
            this.MaximumSize = new Size(500, 500);
            this.Size = new Size(500, 500);
            
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {

        }

        Timer tmr;
        //Tijd dat splash weergegeven wordt
        private void SplashScreen_Shown(object sender, EventArgs e)
        {
            tmr = new Timer();

            //set time interval 3 sec

            tmr.Interval = 3000;

            //starts the timer

            tmr.Start();

            tmr.Tick += tmr_Tick;
        }

        void tmr_Tick(object sender, EventArgs e)

        {

            //after 3 sec stop the timer

            tmr.Stop();

            //display mainform

            Weerstation mf = new Weerstation();

            mf.Show();

            //hide this form

            this.Hide();

        }
    }
}
