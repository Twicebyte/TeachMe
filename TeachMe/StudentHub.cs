using CefSharp;
using CefSharp.WinForms;
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
    public partial class StudentHub : Form
    {
        public StudentHub()
        {
            InitializeComponent();
            if (!Cef.IsInitialized) Cef.Initialize();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void StudentHub_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.OpenForms[0].Show();
        }
        ChromiumWebBrowser myBrowser = new ChromiumWebBrowser();

        private void StudentHub_Load(object sender, EventArgs e)
        {
            dataSet1.ReadXml("DataSet.xml");
            panel7.Controls.Add(myBrowser);
            myBrowser.Dock = DockStyle.Fill;
            HideAll();
            ComboReload();
        }

        private void HideAll()
        {
            myBrowser.Load("about:blank");
            panelField.Visible = false;
            panelRadio.Visible = false;
            panelContent.Visible = false;
        }

        private void VideoSlide(string heading, string sourceId)
        {
            label10.Text = heading;
            string text = "<html><head></head><body><div style=\"background: #000;  position: fixed; top: 0; right: 0; bottom: 0; left: 0;\">" +
                "<div style=\"position: absolute; top: 0; left: 0; width: 100%; height: 100%;\">" +
                "<iframe style=\"position: absolute; top: 0; left: 0; width: 100%; height: 100%;\" src=\"" +
                "https://www.youtube-nocookie.com/embed/" + sourceId + "?controls=1&showinfo=0&rel=0&autoplay=1&loop=1&playlist=W0LHTWG-UmQ" +
                "\"  frameborder=\"0\" allowfullscreen ></iframe></div></div></body></html>";
            File.WriteAllText("slide.html", text);
            Application.DoEvents();
            string curDir = Directory.GetCurrentDirectory();
            myBrowser.Load("file:///" + curDir + "/slide.html");
        }

        private void ImageSlide(string heading, string sourceId)
        {
            label10.Text = heading;
            string text = "<html><head></head><body><div style=\"background: #000;  position: fixed; top: 0; right: 0; bottom: 0; left: 0;\">" +
                "<div style=\"position: absolute; top: 0; left: 0; width: 100%; height: 100%;\">" +
                "<img style=\"position: absolute; top: 0; left: 0; right: 0; bottom: 0; margin: auto; max-width: 100%; max-height: 100%; object-fit: contain;\" src=\"" +
                sourceId +
                "\"  frameborder=\"0\"></img></div></div></body></html>";
            File.WriteAllText("slide.html", text);
            Application.DoEvents();
            string curDir = Directory.GetCurrentDirectory();
            myBrowser.Load("file:///" + curDir + "/slide.html");
        }

        private void TextSlide(string heading, string content)
        {
            label10.Text = heading;
            string text = "<html><head></head><body><div style=\"margin: 40px;\">" +
                content + "</div></body></html>";
            File.WriteAllText("slide.html", text);
            Application.DoEvents();
            string curDir = Directory.GetCurrentDirectory();
            myBrowser.Load("file:///" + curDir + "/slide.html");
        }

        private void loadContent()
        {
            int a = 0;
            foreach (DataRow dr in dataSet1.UserTest.Select("(uid = "+label8.Text+") and (tid = "+label14.Text+")"))
            {
                if (dr[3].ToString() == dataSet1.Test.Select("(objectid = " + dr[2].ToString() + ") and (test = " + label14.Text + ") and (paramName = 'Answer')")[0][3].ToString())
                {
                    a += Convert.ToInt32(dataSet1.Test.Select("(objectid = " + dr[2].ToString() + ") and (test = " + label14.Text + ") and (paramName = 'Points')")[0][3].ToString());
                }
            }
            label5.Text = a.ToString();

            if (listBox1.SelectedItems.Count > 0)
            {
                string objectId = ((string)listBox1.SelectedItem).Split(':')[0];
                string objectType = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Type')")[0][3].ToString();

                if (objectType == "Video")
                {
                    panelContent.Visible = true;
                    string objectHeading = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Heading')")[0][3].ToString();
                    string objectValue = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Value')")[0][3].ToString();
                    VideoSlide(objectHeading, objectValue);
                }


                if (objectType == "Image")
                {
                    string objectHeading = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Heading')")[0][3].ToString();
                    string objectValue = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Value')")[0][3].ToString();
                    panelContent.Visible = true;
                    ImageSlide(objectHeading, objectValue);
                }

                if (objectType == "Text")
                {
                    string objectHeading = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Heading')")[0][3].ToString();
                    string objectValue = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Value')")[0][3].ToString();
                    panelContent.Visible = true;
                    TextSlide(objectHeading, objectValue);
                }

                if (objectType == "Field")
                {
                    string objectQuestion = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Question')")[0][3].ToString();

                    panelField.Visible = true;
                    label13.Text = objectQuestion;

                    if (dataSet1.UserTest.Select("(uid = " + label8.Text + ") and (tid = " + label14.Text + ") and (oid = " + objectId + ")").Length > 0)
                    {
                        string objectAnswer = dataSet1.UserTest.Select("(uid = " + label8.Text + ") and (tid = " + label14.Text + ") and (oid = " + objectId + ")")[0][3].ToString();
                        textBox1.Text = objectAnswer;
                        string objectTruth = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Answer')")[0][3].ToString();
                        button2.Text = "Right Answer: " + objectTruth;
                        textBox1.Enabled = false;
                        button2.Enabled = false;
                    }
                    else
                    {
                        textBox1.Text = "";
                        button2.Text = "Send Answer";
                        textBox1.Enabled = true;
                        button2.Enabled = true;
                    }
                }

                if (objectType == "Radio")
                {
                    string objectQuestion = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Question')")[0][3].ToString();
                    panelRadio.Visible = true;
                    label12.Text = objectQuestion;
                    if (dataSet1.UserTest.Select("(uid = " + label8.Text + ") and (tid = " + label14.Text + ") and (oid = " + objectId + ")").Length > 0)
                    {
                        listBox2.Items.Clear();
                        string objectAnswer = dataSet1.UserTest.Select("(uid = " + label8.Text + ") and (tid = " + label14.Text + ") and (oid = " + objectId + ")")[0][3].ToString();
                        listBox2.Items.Add(objectAnswer);
                        string objectTruth = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Answer')")[0][3].ToString();
                        button1.Text = "Right Answer: " + objectTruth;
                        button1.Enabled = false;
                        listBox2.Enabled = false;
                    }
                    else
                    {
                        listBox2.Items.Clear();
                        foreach (DataRow dr in dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Variance')"))
                            listBox2.Items.Add(dr[3].ToString());
                        button1.Text = "Send Answer";
                        button1.Enabled = true;
                        listBox2.Enabled = true;
                    }
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAll();
            loadContent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAll();
            try
            {
                label14.Text = dataSet1.Test.Select("(value like '" + comboBox1.Text + "') and (objectId=0)")[0][0].ToString();
                ListReload();
                int a = 0;
                foreach (DataRow dr in dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId > 0) and (paramName like 'Points')", "objectId asc"))
                    a += Convert.ToInt32(dr[3].ToString());
                label6.Text = "/   " + a.ToString();
                loadContent();
            }
            catch { MessageBox.Show("Курс не найден."); }
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
            foreach (DataRow dr in dataSet1.Test.Select("(test = "+ label14.Text + ") and (objectId > 0) and (paramName like 'Name')", "objectId asc"))
                listBox1.Items.Add(dr[1].ToString() + ": " + dr[3].ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string objectId = ((string)listBox1.SelectedItem).Split(':')[0];
            string objectPoints = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Points')")[0][3].ToString();
            dataSet1.UserTest.AddUserTestRow((Resources.DataSet.UserRow)dataSet1.User.Select("id = " + label8.Text)[0], Convert.ToInt32(label14.Text), Convert.ToInt32(objectId), textBox1.Text);
            dataSet1.WriteXml("DataSet.xml");
            loadContent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string objectId = ((string)listBox1.SelectedItem).Split(':')[0];
            string objectPoints = dataSet1.Test.Select("(test = " + label14.Text + ") and (objectId = " + objectId + ") and (paramName like 'Points')")[0][3].ToString();
            dataSet1.UserTest.AddUserTestRow((Resources.DataSet.UserRow)dataSet1.User.Select("id = " + label8.Text)[0], Convert.ToInt32(label14.Text), Convert.ToInt32(objectId), (string)listBox2.SelectedItem);
            dataSet1.WriteXml("DataSet.xml");
            loadContent();
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
