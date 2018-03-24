using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeachMe.Resources;

namespace TeachMe
{
    public partial class SignInForm : Form
    {
        public SignInForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                label7.Text = ((int)dataSet1.User.Select("", "id desc")[0][0]+1).ToString();
            }
            catch { label7.Text = "1"; }
            panel1.Visible = false; panel2.Visible = true;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panel1.Visible = true; panel2.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataSet1.User.AddUserRow(Convert.ToInt32(label7.Text), textBox4.Text, textBox5.Text, textBox6.Text, textBox3.Text);
            dataSet1.WriteXml("DataSet.xml");
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox3.Clear();
            panel1.Visible = true; panel2.Visible = false;
            textBox1.Text = label7.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (File.ReadAllText("DataSet.xml")[1] != '?')
                    File.WriteAllText("DataSet.xml", Altcode(File.ReadAllText("DataSet.xml")));
                dataSet1.ReadXml("DataSet.xml");
            }
            catch {
                dataSet1.WriteXml("DataSet.xml");
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") MessageBox.Show("ID must be stated. Please check and try again.");
            else if (dataSet1.User.Select("id=" + textBox1.Text + " and password like \'" + textBox2.Text + "\'").Count<DataRow>() > 0)
            {
                if (textBox1.Text[0] == '-')
                {
                    TeachersHub TH = new TeachersHub();
                    textBox2.Clear();
                    Hide();
                    TH.Show();
                    TH.label1.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["firstName"];
                    TH.label2.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["secondName"];
                    TH.label3.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["lastName"];
                    TH.label8.Text = textBox1.Text;
                }
                else
                {
                    StudentHub SH = new StudentHub();
                    textBox2.Clear();
                    Hide();
                    SH.Show();
                    SH.label1.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["firstName"];
                    SH.label2.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["secondName"];
                    SH.label3.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["lastName"];
                    SH.label8.Text = textBox1.Text;
                }
            }
            else
            {
                MessageBox.Show("ID - Password pair was not recognised. Please check and try again.");
            }
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == "") MessageBox.Show("ID must be stated. Please check and try again.");
                else if (dataSet1.User.Select("id=" + textBox1.Text + " and password like \'" + textBox2.Text + "\'").Count<DataRow>() > 0)
                {
                    if (textBox1.Text[0] == '-')
                    {
                        TeachersHub TH = new TeachersHub();
                        textBox2.Clear();
                        Hide();
                        TH.Show();
                        TH.label1.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["firstName"];
                        TH.label2.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["secondName"];
                        TH.label3.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["lastName"];
                        TH.label8.Text = textBox1.Text;
                    }
                    else
                    {
                        StudentHub SH = new StudentHub();
                        textBox2.Clear();
                        Hide();
                        SH.Show();
                        SH.label1.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["firstName"];
                        SH.label2.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["secondName"];
                        SH.label3.Text = (string)dataSet1.User.Select("id=" + textBox1.Text)[0]["lastName"];
                        SH.label8.Text = textBox1.Text;
                    }
                }
                else
                {
                    MessageBox.Show("ID - Password pair was not recognised. Please check and try again.");
                }
            }
        }
        

        public string Altcode(string text)
        {
            string newtext = "";
            for (int i = 0; i < text.Length; i++)
            {
                newtext += (char)(2000-text[i]);
            }
            return newtext;
        }

        private void SignInForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.Delete("slide.html");
            File.WriteAllText("DataSet.xml", Altcode(File.ReadAllText("DataSet.xml")));        
        }
    }
}
