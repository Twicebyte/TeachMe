using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeachMe
{
    public partial class TeachersHub : Form
    {
        public TeachersHub()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void TeachersHub_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.OpenForms[0].Show();
        }

        private void HideAll()
        {
            tabControl1.Hide();
            sourceTest.Filter = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HideAll();
            try
            {
                label5.Text = ((int)dataSet1.Test.Select("", "test desc")[0][0] + 1).ToString();
            }
            catch { label5.Text = "1"; }
            tabControl1.Show();
            dataSet1.Test.AddTestRow(Convert.ToInt32(label5.Text), 0, "Name", "Course " + label5.Text);
            sourceTest.Filter = "test = " + label5.Text;
            dataSet1.WriteXml("DataSet.xml");
            ComboReload();
        }

        private void ComboReload()
        {
            comboBox1.Items.Clear();
            foreach (DataRow dr in dataSet1.Test.Select("objectId=0"))
                comboBox1.Items.Add(dr[3].ToString());
        }

        private void ListReload()
        {
            listBox1.Items.Clear();
            listBox1.Items.Add("<All students>");
            foreach (DataRow dr in dataSet1.User.Select("id>=0", "lastName asc, firstName asc, secondName asc"))
                listBox1.Items.Add(dr[0].ToString() + ": " + dr[3].ToString() + " " + dr[1].ToString() + " " + dr[2].ToString());
        }

        BindingSource sourceTest = new BindingSource();
        private void TeachersHub_Load(object sender, EventArgs e)
        {
            dataSet1.ReadXml("DataSet.xml");
            ComboReload();
            ListReload();
            dataGridView1.AutoGenerateColumns = true;
            sourceTest.DataSource = dataSet1.Test;
            dataGridView1.DataSource = sourceTest;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAll();
            try
            {
                label5.Text = dataSet1.Test.Select("(value like '" + comboBox1.Text + "') and (objectId=0)")[0][0].ToString();
                tabControl1.Show();
                sourceTest.Filter = "test = " + label5.Text;
                ListReload();
                proceedStsts();
            }
            catch { }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            sourceTest.EndEdit();
            dataSet1.AcceptChanges();
            dataSet1.WriteXml("DataSet.xml");
            File.Encrypt("DataSet.xml");
            ComboReload();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HideAll();
            if (dataSet1.Test.Select("test = " + label5.Text).Length > 0)
            {
                foreach (DataRow TR in dataSet1.Test.Select("test = " + label5.Text))
                    dataSet1.Test.RemoveTestRow((Resources.DataSet.TestRow)TR);
                dataSet1.AcceptChanges();
                dataSet1.WriteXml("DataSet.xml");
                ComboReload();
            }
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            sourceTest.EndEdit();
            dataSet1.AcceptChanges();
            dataSet1.WriteXml("DataSet.xml");
            ComboReload();
        }

        private void proceedStsts()
        {
            int a = 0;
            foreach (DataRow dr in dataSet1.Test.Select("(test = " + label5.Text + ") and (objectId > 0) and (paramName like 'Points')", "objectId asc"))
                a += Convert.ToInt32(dr[3].ToString());
            label12.Text = a.ToString();
            dataGridView2.Rows.Clear();
            if ((listBox1.SelectedItems.Count>0) && !((string)listBox1.SelectedItem == "<All students>"))
            {
                a = 0;
                string objectId = ((string)listBox1.SelectedItem).Split(':')[0];

                foreach (DataRow dr in dataSet1.UserTest.Select("(uid = " + objectId + ") and (tid = " + label5.Text + ")"))
                {
                    if (dr[3].ToString() == dataSet1.Test.Select("(objectid = " + dr[2].ToString() + ") and (test = " + label5.Text + ") and (paramName = 'Answer')")[0][3].ToString())
                    {
                        a += Convert.ToInt32(dataSet1.Test.Select("(objectid = " + dr[2].ToString() + ") and (test = " + label5.Text + ") and (paramName = 'Points')")[0][3].ToString());
                    }
                }
                label11.Text = a.ToString();

                if (dataSet1.UserTest.Select("(uid = " + objectId + ") and (tid = " + label5.Text + ")").Length == dataSet1.Test.Select("(test = " + label5.Text + ") and (paramName = 'Answer')").Length)
                {
                    label14.Text = "1";
                }
                else
                {
                    label14.Text = "0";
                }

                foreach (DataRow dr in dataSet1.UserTest.Select("(uid = " + objectId + ") and (tid = " + label5.Text + ")"))
                {
                    DataRow udr = dataSet1.User.Select("id = " + dr[0].ToString())[0];
                    DataRow tdr = dataSet1.Test.Select("(test = " + label5.Text + ") and (paramName = 'Question') and (objectId = " + dr[2].ToString() + ")")[0];
                    dataGridView2.Rows.Add();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[0].Value = dr[2].ToString();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[1].Value = tdr[3].ToString();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[2].Value = udr[1].ToString();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[3].Value = udr[3].ToString();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[4].Value = dr[3].ToString();
                }

                chart1.Series[0].Points.Clear();
                foreach (DataRow dr in dataSet1.Test.Select("(paramName = 'Type') and ((value = 'Radio') or (value = 'Field')) and (test = " + label5.Text + ")"))
                {
                    chart1.Series[0].Points.AddXY(dr[1].ToString(), dataSet1.UserTest.Select("(uid = " + objectId + ") and (tid = " + label5.Text + ") and (oid = " + dr[1].ToString() + ")").Length);
                }

            }
            else
            {
                a = 0; int b = 0;
                foreach (DataRow pdr in dataSet1.User.Select("id>0"))
                {
                    string objectId = pdr[0].ToString();
                    foreach (DataRow dr in dataSet1.UserTest.Select("(uid = " + objectId + ") and (tid = " + label5.Text + ")"))
                    {
                        if (dr[3].ToString() == dataSet1.Test.Select("(objectid = " + dr[2].ToString() + ") and (test = " + label5.Text + ") and (paramName = 'Answer')")[0][3].ToString())
                        {
                            a += Convert.ToInt32(dataSet1.Test.Select("(objectid = " + dr[2].ToString() + ") and (test = " + label5.Text + ") and (paramName = 'Points')")[0][3].ToString());
                        }
                    }

                    if (dataSet1.UserTest.Select("(uid = " + objectId + ") and (tid = " + label5.Text + ")").Length == dataSet1.Test.Select("(test = " + label5.Text + ") and (paramName = 'Answer')").Length)
                    {
                        b++;
                    }

                }
                if (a > 0)
                    label11.Text = (a / dataSet1.User.Select("id>0").Length).ToString();
                else
                    label11.Text = "0";
                label14.Text = b.ToString();

                foreach (DataRow dr in dataSet1.UserTest.Select("tid = " + label5.Text))
                {
                    DataRow udr = dataSet1.User.Select("id = " + dr[0].ToString())[0];
                    DataRow tdr = dataSet1.Test.Select("(test = " + label5.Text + ") and (paramName = 'Question') and (objectId = " + dr[2].ToString() + ")")[0];
                    dataGridView2.Rows.Add();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[0].Value = dr[2].ToString();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[1].Value = tdr[3].ToString();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[2].Value = udr[1].ToString();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[3].Value = udr[3].ToString();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[4].Value = dr[3].ToString();
                }

                chart1.Series[0].Points.Clear();
                foreach (DataRow dr in dataSet1.Test.Select("(paramName = 'Type') and ((value = 'Radio') or (value = 'Field')) and (test = " + label5.Text + ")"))
                {
                    chart1.Series[0].Points.AddXY(dr[1].ToString(), dataSet1.UserTest.Select("(tid = " + label5.Text + ") and (oid = " + dr[1].ToString() + ")").Length);
                }

            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            proceedStsts();
        }

        private void label16_Click(object sender, EventArgs e)
        {
            Help h = new Help();
            h.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            UserChange UC = new UserChange();
            UC.label8.Text = label8.Text;
            UC.ShowDialog();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            UserChange UC = new UserChange();
            UC.label8.Text = label8.Text;
            UC.ShowDialog();
        }
    }
}
