using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.GroupPolicy;


namespace SearchADGPO
{
    public partial class Form1 : Form
    {
        DateTime timeThreshold;
        public static string ReportURL = "";

        public Form1()
        {
            InitializeComponent();
            listBox1.DoubleClick += new EventHandler(listBox1_DoubleClick);
            listBox1.MouseDown += new MouseEventHandler(listBox1_MouseDown);
            numericUpDown1.Value = 120;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            String SearchString = textBox3.Text;
            if (SearchString.Length != 0)
            {
                listBox1.Items.Clear();

                // Show Searching and Disable Buttons
                label3.Visible = true;
                button1.Enabled = false;

                W7_CheckBox.Enabled = false;
                W10_CheckBox.Enabled = false;

                textBox3.Enabled = false;

                label5.Enabled = false;
                numericUpDown1.Enabled = false;

                // Setting Threshold
                timeThreshold = DateTime.Now.AddMinutes(Decimal.ToDouble(-numericUpDown1.Value));

                // Use current user’s domain 
                GPDomain domain = new GPDomain();

                Log("Searching Domain to get ALL GPO's.");
                GpoCollection gpos;
                GPSearchCriteria gPSearchCriteria = new GPSearchCriteria();

                // Generate PolicyFilter
                if (W10_CheckBox.Checked)
                {
                    gPSearchCriteria.Add(SearchProperty.GpoDisplayName, SearchOperator.Contains, "W10");
                }

                // Generate PolicyFilter
                if (W7_CheckBox.Checked)
                {
                    gPSearchCriteria.Add(SearchProperty.GpoDisplayName, SearchOperator.Contains, "W7");
                }


                if (gPSearchCriteria.ToString().Length != 0)
                {
                    // Search Policies with criteria
                    gpos = domain.SearchGpos(gPSearchCriteria);
                }
                else
                {
                    // Search all Policies
                    gpos = domain.GetAllGpos();
                }


                Log("We need to search " + gpos.Count.ToString() + " GPO's.");
                Log("Cached files older than : " + timeThreshold.ToString() + " will be renewed.");

                progressBar1.Maximum = gpos.Count;
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                progressBar1.Visible = true;
                label3.Visible = true;

                if (progressBar1.Visible && label3.Visible)
                {
                    foreach (Gpo gpo in gpos)
                    {

                        Log("Searching " + gpo.DisplayName);

                        //Checking if TempReport exists
                        string GPOFolder = Path.GetDirectoryName(Application.ExecutablePath) + @"\GPOs\XML";
                        if (!(Directory.Exists(GPOFolder)))
                        {
                            Directory.CreateDirectory(GPOFolder);
                        }
                        string GPOReportFile = GPOFolder + @"\" + gpo.Id.ToString() + ".xml";
                        string tempreport;
                        if (!(File.Exists(GPOReportFile)))
                        {
                            Log("Cache file doesn't exist. Creating.");
                            tempreport = gpo.GenerateReport(ReportType.Xml);
                            try
                            {
                                File.WriteAllText(GPOReportFile, tempreport);
                            }
                            catch (Exception ex)
                            {
                                Log("Error on " + gpo.DisplayName + ": " + ex.Message.ToString());
                            }

                        }
                        else if (timeThreshold >= File.GetLastWriteTime(GPOReportFile))
                        {

                            Log("Renew file due expired Cache.");
                            tempreport = gpo.GenerateReport(ReportType.Xml);
                            try
                            {
                                File.WriteAllText(GPOReportFile, tempreport);
                            }
                            catch (Exception ex)
                            {
                                Log("Error on " + gpo.DisplayName + ": " + ex.Message.ToString());
                            }

                        }
                        else
                        {
                            tempreport = File.ReadAllText(GPOReportFile);
                        }

                        if (tempreport.Contains(SearchString))
                        {
                            listBox1.Items.Add(gpo.DisplayName);
                        }
                        progressBar1.Value++;

                    }
                }

                int found = listBox1.Items.Count;

                Log("Found " + found.ToString() + " GPO's with " + SearchString);
            }
            else
            {
                Log("Our SearchString is empty. We are not serching.");
            }

            W7_CheckBox.Checked = false;
            W10_CheckBox.Checked = false;

            W7_CheckBox.Enabled = true;
            W10_CheckBox.Enabled = true;

            label5.Enabled = true;
            numericUpDown1.Enabled = true;

            textBox3.Text = "";
            textBox3.Enabled = true;
            textBox3.Focus();

            label3.Visible = false;
            progressBar1.Visible = false;
            button1.Enabled = true;
        }

        private void Log(string v)
        {
            var Datum = DateTime.Now.ToString();
            log.Text += Datum + ": " + v + System.Environment.NewLine;
            log.SelectionStart = log.TextLength;
            log.ScrollToCaret();
        }

        private void W10_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (W10_CheckBox.Checked)
            {
                if (W7_CheckBox.Checked)
                {
                    W7_CheckBox.Checked = false;
                }
            }

        }

        private void W7_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (W7_CheckBox.Checked)
            {
                if (W10_CheckBox.Checked)
                {
                    W10_CheckBox.Checked = false;
                }
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var index = listBox1.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    listBox1.SelectedIndex = index;
                }
                if (listBox1.SelectedItem != null && index != ListBox.NoMatches)
                {
                    Log("Getting selected GPO and display SubMenue");
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            gPOReportToolStripMenuItem_Click(sender, e);
        }

        private void editGPOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                Log("Getting selected GPO");
                // Use current user’s domain 
                GPDomain domain = new GPDomain();

                GpoCollection gpos;
                GPSearchCriteria gPSearchCriteria = new GPSearchCriteria();
                gPSearchCriteria.Add(SearchProperty.GpoDisplayName, SearchOperator.Equals, listBox1.SelectedItem.ToString());

                gpos = domain.SearchGpos(gPSearchCriteria);
                foreach (Gpo gpo in gpos)
                {
                    Log("Opening GPO " + gpo.DisplayName.ToString());
                    String Param = @"/gpobject:'LDAP://" + gpo.Path + '"';
                    Process.Start("gpedit.msc", Param);
                }
            }

        }

        private void gPOReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                Log("Getting selected GPO");
                // Use current user’s domain 
                GPDomain domain = new GPDomain();

                GpoCollection gpos;
                GPSearchCriteria gPSearchCriteria = new GPSearchCriteria();
                gPSearchCriteria.Add(SearchProperty.GpoDisplayName, SearchOperator.Equals, listBox1.SelectedItem.ToString());

                gpos = domain.SearchGpos(gPSearchCriteria);
                foreach (Gpo gpo in gpos)
                {
                    Log("Generate HTML Report for " + gpo.DisplayName.ToString());
                    //Checking if TempReport exists
                    string GPOFolder = Path.GetDirectoryName(Application.ExecutablePath) + @"\GPOs\HTML";
                    if (!(Directory.Exists(GPOFolder)))
                    {
                        Directory.CreateDirectory(GPOFolder);
                    }

                    string GPOReportFile = GPOFolder + @"\" + gpo.Id.ToString() + ".html";
                    try
                    {
                        listBox1.Enabled = false;
                        listBox1.BackColor = SystemColors.Control;
                        label6.Visible = true;
                        
                        gpo.GenerateReportToFile(ReportType.Html, GPOReportFile);
                        Application.DoEvents();
                        Log("Finished generation and starting Viewer.");
                        
                        label6.Visible = false;
                        listBox1.BackColor = SystemColors.Window;
                        listBox1.Enabled = true;
                        ReportURL = GPOReportFile;
                        GPOReportViewer gPOReportViewer = new GPOReportViewer();
                        gPOReportViewer.Show();
                        
                    }
                    catch
                    {
                        Log("Error on generating HTML Report for GPO " + gpo.Id.ToString() + " | " + gpo.DisplayName);
                    }

                }
            }
        }
    }
}