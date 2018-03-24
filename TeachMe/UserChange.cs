using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeachMe
{
    public partial class UserChange : Form
    {
        public UserChange()
        {
            InitializeComponent();
        }

        private void UserChange_Load(object sender, EventArgs e)
        {
            dataSet1.ReadXml("DataSet.xml");
            DataRow DR = dataSet1.User.Select("id = " + label8.Text)[0];
            textBox4.Text = DR[1].ToString();
            textBox5.Text = DR[2].ToString();
            textBox6.Text = DR[3].ToString();
            textBox3.Text = DR[4].ToString();
            textBox3.Focus();
        }

        private void UserChange_FormClosing(object sender, FormClosingEventArgs e)
        {
            dataSet1.User.RemoveUserRow((Resources.DataSet.UserRow)dataSet1.User.Select("id = " + label8.Text)[0]);
            if (textBox1.Text == "Per Aspera Ad Astra")
            dataSet1.User.AddUserRow(-Convert.ToInt32(label8.Text), textBox4.Text, textBox5.Text, textBox6.Text, textBox3.Text);
            else
            dataSet1.User.AddUserRow(Convert.ToInt32(label8.Text), textBox4.Text, textBox5.Text, textBox6.Text, textBox3.Text);
            dataSet1.WriteXml("DataSet.xml");
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "Per Aspera Ad Astra")
                panel1.BackColor = Color.FromArgb(192, 64, 0);
            else
                panel1.BackColor = Color.WhiteSmoke;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
