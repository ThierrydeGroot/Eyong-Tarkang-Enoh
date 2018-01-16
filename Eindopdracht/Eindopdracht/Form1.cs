using System;
using System.Text;
using System.ComponentModel;
using System.Drawing;

using System.Windows.Forms;

using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Eindopdracht.localhost;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace Eindopdracht
{
    public partial class Weerstation : Form
    {
        string conSql = "server=localhost; database=weerstation; user=root; password=";
        private int _weatherTimer = 0;

        //Form is 900 bij 750 ivm hoge resolutie van mijn scherm ivm scaling

        public Weerstation()
        {
      


            InitializeComponent();
            this.MaximumSize = new Size(900, 750);
            this.Size = new Size(900, 750);
            tabControl1.MaximumSize = new Size(900, 750);
            tabControl1.Size = new Size(900, 750);
            timer1.Start();
            UpdateWeather();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);


        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        //Database Connectie en schrijven
        private void WriteToDatabase(DateTime date, double temp, string loc)
        {
            string dateQuery = date.ToString("yyyy-MM-dd H:mm:ss");
            string query = "INSERT INTO temperature(date, temp, location)VALUES('" + dateQuery + "','" + temp + "', '" + loc + "');";
            MySqlConnection conDatabase = new MySqlConnection(conSql);
            MySqlCommand cmdDatabase = new MySqlCommand(query, conDatabase);
            MySqlDataReader myReader;
            try
            {
                conDatabase.Open();
                myReader = cmdDatabase.ExecuteReader();
                while (myReader.Read())
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Database Connectie en ophalen

        private void GetFromDatabase(string loc)
        {
            List<String> columnData = new List<String>();

            string query = "SELECT temp FROM temperature WHERE location='" + loc + "' AND date >= DATE(NOW()) - INTERVAL 5 DAY ORDER BY date DESC LIMIT 5";
            MySqlConnection conDatabase = new MySqlConnection(conSql);
            MySqlCommand cmdDatabase = new MySqlCommand(query, conDatabase);
            MySqlDataReader myReader;
            try
            {
                conDatabase.Open();
                myReader = cmdDatabase.ExecuteReader();

                while (myReader.Read())
                {

                    string tempSQL = myReader["temp"].ToString();
                 

                        columnData.Add(myReader.GetString(0));

                    //File.WriteAllText("getDBData.txt", "temp: " + tempSQL + "\n date: ");


                }
                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                }
                chart1.Series["Temperatuur"].ChartType = SeriesChartType.Line;
                var increment = 1;
                columnData.Reverse();

                if (radioButton2.Checked)
                {
                    foreach (var v in columnData)
                    {
                        if (increment < 6)
                        {


                            chart1.Series["Temperatuur"].Points.AddXY(increment, Int32.Parse(v) * 1.8 +32);
                            increment++;
                        }
                    }
                }
                else { 
                    foreach (var v in columnData)
                    {
                        if (increment < 6) {


                            chart1.Series["Temperatuur"].Points.AddXY(increment, v);
                            increment++;
                        }
                    }

                }

                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
                 

        }


        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Weerstation_FormClosed(object sender, FormClosedEventArgs e)
        {

            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {
            
        }

        //Weer verversen met evt nieuwe info
        private void UpdateWeather()
        {

            localhost.WebService1 service = new localhost.WebService1();
            String[] cWeather = service.CurrentWeather(PlaatsLabel.Text);
          
            //F of C
            if (radioButton2.Checked)
            {
                double fahrint = Convert.ToInt32(double.Parse(cWeather[2]) * 1.8 + 32);
                tempLabel.Text = fahrint + " F";
            }
            if (radioButton1.Checked)
            {
                int celcius = Convert.ToInt32(double.Parse(cWeather[2]));
                tempLabel.Text = celcius + " °C";
            }

            //Waarden op eerste tab

            vochtigLabel.Text = cWeather[3] + " %";
            windLabel.Text = cWeather[4] + " km/h";
            weatherPic.ImageLocation = cWeather[5];
            label6.Text = "(Laatste update: " + DateTime.Now.ToLongTimeString() + ")";
            naastPlaatje.Text = cWeather[0];
            weertype.Text = cWeather[1];

            //Refresh timer
            _weatherTimer = 60;
           
            toolStripMenuItem1.Text = cWeather[0] + ": " + tempLabel.Text;

            //DB Schrijven

            WriteToDatabase(DateTime.Now, Convert.ToInt32(double.Parse(cWeather[2])), cWeather[0]);

            //Grafiek maken met database gegevens

            GetFromDatabase(cWeather[0]);

            

        }

        //Timer interval update
        private void timer1_Tick(object sender, EventArgs e)
        {
            _weatherTimer++;
            if (_weatherTimer == Int32.Parse(Interval.Text) * 10)
            {
                UpdateWeather();
            }
        }

        //Ok in opties menu
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }
            UpdateWeather();
            
        }

        //app sluiten vanuit toolbar
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //verversen in toolbar
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            UpdateWeather();
        }

        //opties toolbar
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if(Visible == false)
            {
                this.Visible = true;
            }
            tabControl1.SelectTab(tabPage3);
        }

        //openen via toolbar
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (Visible == false)
            {
                this.Visible = true;
            }
        }

        //over-venster openen in toolbar
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }

        private void chart1_Click(object sender, EventArgs e)
        {
           
        }
    }
}
