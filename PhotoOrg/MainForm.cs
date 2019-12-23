﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;

namespace PhotoOrg
{


    public partial class MainForm : Form
    {
        public MainForm()
        {

            InitializeComponent();

            // Check for the jobs file, if it's there, add all the recorded jobs to the tracker.

            int count = 0;
            if (File.Exists("jobs.txt"))
            {
                string[] log = File.ReadAllLines("jobs.txt");
                foreach (string file in log)
                {
                    jobsCheckedListBox.Items.Add(log[count]);
                    count++;
                }
            }

            // Themes. Because that's what this needed. This checks for a settings file, and checks what the current theme set is,
            // and reads the specified filename for color data. I spent an hour on this. Why.

            if (File.Exists("settings/settings.cfg"))
            {

                string[] settingsFile = File.ReadAllLines("settings/settings.cfg");
                string theme = settingsFile[3];
                string themepath = "settings/" + theme + ".ini";

                if (File.Exists(themepath))
                {
                    string[] settings = File.ReadAllLines(themepath);
                    formTitleLabel.Text = settingsFile[1];

                    Color logoCol = Color.FromArgb(Convert.ToInt32(settings[2]), Convert.ToInt32(settings[3]), Convert.ToInt32(settings[4]));
                    Color bgCol = Color.FromArgb(Convert.ToInt32(settings[6]), Convert.ToInt32(settings[7]), Convert.ToInt32(settings[8]));
                    Color boxTextCol = Color.FromArgb(Convert.ToInt32(settings[10]), Convert.ToInt32(settings[11]), Convert.ToInt32(settings[12]));
                    Color boxCol = Color.FromArgb(Convert.ToInt32(settings[14]), Convert.ToInt32(settings[15]), Convert.ToInt32(settings[16]));

                    formTitleLabel.ForeColor = logoCol;

                    this.BackColor = bgCol;

                    if (settings[18] != "none")
                    {
                        Image BG = new Bitmap(@"settings/img/" + settings[18] + ".jpg");
                        this.BackgroundImage = BG;
                    }

                    nameInput.ForeColor = boxTextCol;
                    phoneNumberInput.ForeColor = boxTextCol;
                    orderNumberInput.ForeColor = boxTextCol;
                    addressInput.ForeColor = boxTextCol;
                    emailButton.ForeColor = boxTextCol;
                    browseButton.ForeColor = boxTextCol;
                    jobsToggleCheckbox.ForeColor = boxTextCol;
                    saveButton.ForeColor = boxTextCol;
                    notesInput.ForeColor = boxTextCol;
                    openJobFolderButton.ForeColor = boxTextCol;
                    jobsCheckedListBox.ForeColor = boxTextCol;

                    nameInput.BackColor = boxCol;
                    notesInput.BackColor = boxCol;
                    openJobFolderButton.BackColor = boxCol;
                    phoneNumberInput.BackColor = boxCol;
                    orderNumberInput.BackColor = boxCol;
                    addressInput.BackColor = boxCol;
                    emailButton.BackColor = boxCol;
                    browseButton.BackColor = boxCol;
                    jobsToggleCheckbox.BackColor = boxCol;
                    saveButton.BackColor = boxCol;
                    jobsCheckedListBox.BackColor = boxCol;
                    
                }
            }
        }

        // Magic 'grab the window anywhere to drag' code I nabbed. Don't know how it works, but it does. 

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)(HT_CAPTION);
        }

        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;

        // Pressing the 'Save' button

        private void button2_Click(object sender, EventArgs e)
        {
            int files = 0;
            string savedir = "";

            // Looking to see if an alternate save directory was chosen

            if (File.Exists("settings/settings.cfg") && File.ReadAllLines("settings/settings.cfg")[5] != "default") {
                string[] SettingsFile = File.ReadAllLines("settings/settings.cfg");
                savedir = @SettingsFile[5];
                savedir = savedir + @"\";
            }

            // Check to see if the Job # + Name matches any existing folder. If it doesn't proceed to copy
            // and rename based on user input

            foreach (string file in addFilesOpenFileDialog.FileNames)
            {
                if (!Directory.Exists(savedir + orderNumberInput.Text + " - " + nameInput.Text))
                    Directory.CreateDirectory(savedir + orderNumberInput.Text + " - " + nameInput.Text);
                    int filesexist = Directory.GetFiles(savedir + orderNumberInput.Text + " - " + nameInput.Text).Length;

                if (!File.Exists(savedir + orderNumberInput.Text + " - " + nameInput.Text + "/" + orderNumberInput.Text + " - " + nameInput.Text + " - " + (filesexist + 1) + ".jpg"))
                    File.Copy(addFilesOpenFileDialog.FileNames[files], savedir + orderNumberInput.Text + " - " + nameInput.Text + "/" + nameInput.Text + " - " + (filesexist + 1) + ".jpg");
                files++;
            }

            // The retarded amount of effort involved in saving all of that info to a text file / adding it to the job tracker

            string job = orderNumberInput.Text + " - " + nameInput.Text;
            List<string> jobs = new List<string>();

            if (!jobsCheckedListBox.Items.Contains(job))
            {
                jobs.Add(job);
                string[] info = { nameInput.Text, phoneNumberInput.Text, orderNumberInput.Text, emailButton.Text, addressInput.Text, notesInput.Text };
                File.WriteAllLines(savedir + orderNumberInput.Text + " - " + nameInput.Text + "/" + "Order info for " + " " + nameInput.Text + ".txt", info);
                jobs.ToArray();
                jobsCheckedListBox.Items.Add(job);
                File.AppendAllLines("jobs.txt", jobs);
            }else{
                jobs.Add(job);
                string[] info = { nameInput.Text, phoneNumberInput.Text, orderNumberInput.Text, emailButton.Text, addressInput.Text, notesInput.Text };
                File.WriteAllLines(savedir + orderNumberInput.Text + " - " + nameInput.Text + "/" + "Order info for " + " " + nameInput.Text + ".txt", info);
            }
            // Copying finished, show confirmation, and reset all parameters to get ready for the next job

            MessageBox.Show("Done!");
            addFilesOpenFileDialog.Reset();
            addFilesOpenFileDialog.Multiselect = true;
            browseButton.Text = "browse";
            nameInput.Text = "name";
            addressInput.Text = "address";
            emailButton.Text = "email";
            phoneNumberInput.Text = "phone";
            notesInput.Text = "notes";

            if (Regex.IsMatch(orderNumberInput.Text, @"^\d+$"))
            {

                int orderNum = int.Parse(orderNumberInput.Text);
                orderNum++;
                orderNumberInput.Text = orderNum.ToString();
            }
            else { orderNumberInput.Text = "order number"; }
        }

        // Apply theme settings when you click the logo. Because I was too dumb to figure out a real way.

        private void logo_Click(object sender, EventArgs e)
        {
            if (File.Exists("settings/settings.cfg"))
            {

                string[] settingsFile = File.ReadAllLines("settings/settings.cfg");
                string theme = settingsFile[3];
                string themepath = "settings/" + theme + ".ini";

                if (File.Exists(themepath))
                {
                    string[] settings = File.ReadAllLines(themepath);
                    formTitleLabel.Text = settingsFile[1];

                    Color logoCol = Color.FromArgb(Convert.ToInt32(settings[2]), Convert.ToInt32(settings[3]), Convert.ToInt32(settings[4]));
                    Color bgCol = Color.FromArgb(Convert.ToInt32(settings[6]), Convert.ToInt32(settings[7]), Convert.ToInt32(settings[8]));
                    Color boxTextCol = Color.FromArgb(Convert.ToInt32(settings[10]), Convert.ToInt32(settings[11]), Convert.ToInt32(settings[12]));
                    Color boxCol = Color.FromArgb(Convert.ToInt32(settings[14]), Convert.ToInt32(settings[15]), Convert.ToInt32(settings[16]));

                    formTitleLabel.ForeColor = logoCol;

                    this.BackColor = bgCol;

                    if (settings[18] != "none")
                    {
                        Image BG = new Bitmap(@"settings/img/" + settings[18] + ".jpg");
                        this.BackgroundImage = BG;
                    }
                    else {
                        this.BackgroundImage = null;
                    }

                    nameInput.ForeColor = boxTextCol;
                    phoneNumberInput.ForeColor = boxTextCol;
                    orderNumberInput.ForeColor = boxTextCol;
                    addressInput.ForeColor = boxTextCol;
                    emailButton.ForeColor = boxTextCol;
                    browseButton.ForeColor = boxTextCol;
                    jobsToggleCheckbox.ForeColor = boxTextCol;
                    saveButton.ForeColor = boxTextCol;
                    notesInput.ForeColor = boxTextCol;
                    openJobFolderButton.ForeColor = boxTextCol;
                    jobsCheckedListBox.ForeColor = boxTextCol;

                    nameInput.BackColor = boxCol;
                    notesInput.BackColor = boxCol;
                    phoneNumberInput.BackColor = boxCol;
                    orderNumberInput.BackColor = boxCol;
                    addressInput.BackColor = boxCol;
                    emailButton.BackColor = boxCol;
                    browseButton.BackColor = boxCol;
                    jobsToggleCheckbox.BackColor = boxCol;
                    saveButton.BackColor = boxCol;
                    openJobFolderButton.BackColor = boxCol;
                    jobsCheckedListBox.BackColor = boxCol;
                }
            }
        }

        // Stuff to show the 'X Files Loaded!' message based on how many files are loaded

        private void browse_Click(object sender, EventArgs e)
        {
            if (addFilesOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int files = 0;
                foreach (string file in addFilesOpenFileDialog.FileNames) { files++; }
                if (files == 1) { browseButton.Text = files + " file loaded!"; }
                else { browseButton.Text = files + " files loaded!"; }
                saveButton.Enabled = true;
            }
        }

        // I'll make my own exit button, with blackjack, and overused jokes

        private void exit_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        // Gay Code Jail

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        // For showing / hiding the job list

        private void mail_CheckedChanged(object sender, EventArgs e)
        {
            if (jobsToggleCheckbox.Checked) {
                ActiveForm.Width = 1377;
            } else {
                ActiveForm.Width = 472;
            }

        }

        // oopsie whoopsie looks like you cant remove these

        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }

        private void settingsOpen_Click(object sender, EventArgs e)
        {
            if (File.Exists("settings/settings.cfg"))
            {
                SettingsForm settings = new SettingsForm();
                settings.Show();
            }
            else
            {
                MessageBox.Show("Settings file missing!");
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void PhotoOrg_Load(object sender, EventArgs e)
        {

        }

        private List<PictureBox> PictureBoxes = new List<PictureBox>();
        private const int ThumbWidth = 200;
        private const int ThumbHeight = 200;
        private const int exifOrientationID = 0x112; //274

        // Reset all parameters to the info for a previous job if clicked on in the list

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string savedir = "";

            if (File.Exists("settings/settings.cfg") && File.ReadAllLines("settings/settings.cfg")[5] != "default")
            {
                string[] SettingsFile = File.ReadAllLines("settings/settings.cfg");
                savedir = @SettingsFile[5];
                savedir = savedir + @"\";
                string Select = (sender as CheckedListBox).SelectedItem.ToString();
                string cwd = savedir + Select;
                MessageBox.Show(cwd);
                Process.Start("explorer.exe", cwd);
            }
           else
            {   // Open previous job
                string Select = (sender as CheckedListBox).SelectedItem.ToString();
                string[] chop = Select.Split('-');
                string dice = chop[chop.Length - 1];
                string Fname = dice.Remove(0, 1);

                string cwinfo = Directory.GetCurrentDirectory() + @"\" + Select + @"\Order info for  " + Fname + ".txt";
                string cwd = Directory.GetCurrentDirectory() + @"\" + Select;
                int filesexist = Directory.GetFiles(cwd).Length;
                if (File.Exists(cwinfo))
                {
                    string[] info = File.ReadAllLines(cwinfo);

                    // loading Image thumbnail stuff

                    foreach (PictureBox pic in PictureBoxes) {
                        pic.DoubleClick -= PictureBox_DoubleClick;
                        pic.Dispose();
                    }

                    imagePreviewPanel.Controls.Clear();
                    PictureBoxes = new List<PictureBox>();

                    List<string> filenames = new List<string>();
                    string[] patterns = { "*.png", "*.gif", "*.jpg", "*.bmp", "*.tif" };
                    foreach (string pattern in patterns) {
                        filenames.AddRange(Directory.GetFiles(cwd, pattern, SearchOption.TopDirectoryOnly));
                    }
                    filenames.Sort();

                    foreach (string filename in filenames) {
                        PictureBox pic = new PictureBox();
                        pic.ClientSize = new Size(ThumbWidth, ThumbHeight);
                        pic.Image = new Bitmap(filename);

                        if ((pic.Image.Width > ThumbWidth) || (pic.Image.Height > ThumbHeight))
                        {
                            pic.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else {
                            pic.SizeMode = PictureBoxSizeMode.CenterImage;
                        }

                        // Check if image has Rotation flag from Exif data

                        if (pic.Image.PropertyIdList.Contains(exifOrientationID))
                        {
                            var prop = pic.Image.GetPropertyItem(exifOrientationID);
                            int val = BitConverter.ToUInt16(prop.Value, 0);
                            var rot = RotateFlipType.RotateNoneFlipNone;
                            if (val == 3 || val == 4)
                                rot = RotateFlipType.Rotate180FlipNone;
                            else if (val == 5 || val == 6)
                                rot = RotateFlipType.Rotate90FlipNone;
                            else if (val == 7 || val == 8)
                                rot = RotateFlipType.Rotate270FlipNone;
                            if (val == 2 || val == 4 || val == 5 || val == 7)
                                rot |= RotateFlipType.RotateNoneFlipX;
                            if (rot != RotateFlipType.RotateNoneFlipNone)
                                pic.Image.RotateFlip(rot);
                        }

                        pic.DoubleClick += PictureBox_DoubleClick;
                        FileInfo file_info = new FileInfo(filename);
                        //tipPicture.SetToolTip(pic, file_info.Name + "/nCreated " + file_info.CreationTime.ToShortDateString() + "\n(" + pic.Image.Width + " x " + pic.Image.Height + ") " + ToFileSizeApi(file_info.Length));
                        pic.Tag = file_info;

                        
                        pic.Parent = imagePreviewPanel;
                    }

                    // Repopulate the text boxes with saved info

                    addFilesOpenFileDialog.Reset();
                    addFilesOpenFileDialog.Multiselect = true;
                    nameInput.Text = info[0];
                    addressInput.Text = info[4];
                    notesInput.Text = info[5];
                    emailButton.Text = info[3];
                    orderNumberInput.Text = info[2];
                    phoneNumberInput.Text = info[1];
                    fileCountLabel.Text = "files: " + filesexist;
                    saveButton.Enabled = true;
                }
                else {
                    MessageBox.Show(cwinfo + Fname);
                }
            }
        }

        // Open up the folder for the job you have clicked

        private void OpenJobFolderButton_Click(object sender, EventArgs e)
        {
            string savedir = "";

            if (File.Exists("settings/settings.cfg") && File.ReadAllLines("settings/settings.cfg")[5] != "default")
            {
                // Handle some custom save directory stuff for opening jobs

                string[] SettingsFile = File.ReadAllLines("settings/settings.cfg");
                savedir = @SettingsFile[5];
                savedir = savedir + @"\";
                string Select = (sender as CheckedListBox).SelectedItem.ToString();
                string cwd = savedir + Select;
                MessageBox.Show(cwd);
                Process.Start("explorer.exe", cwd);
            }
            else
            {
                string Select = jobsCheckedListBox.SelectedItem.ToString();
                string cwd = Directory.GetCurrentDirectory() + @"\" + Select;
                Process.Start("explorer.exe", cwd);
            }
        }

        private void notes_TextChanged(object sender, EventArgs e)
        {

        }

        // Deletes job

        private void delete_Click(object sender, EventArgs e)
        {
            DialogResult delmes = MessageBox.Show("Are you sure you would like to remove entry? Photos will remain","Delete Job",MessageBoxButtons.YesNo);
            if (delmes == DialogResult.Yes)
            {
                string killme = jobsCheckedListBox.SelectedItem.ToString();

                string[] Lines = File.ReadAllLines("jobs.txt");
                File.Delete("jobs.txt");// Deleting the file
                using (StreamWriter sw = File.AppendText("jobs.txt"))

                {
                    foreach (string line in Lines)
                    {
                        if (line.IndexOf(killme) >= 0)
                        {
                            continue;
                        }
                        else
                        {
                            sw.WriteLine(line);
                        }
                    }
                }

                if (jobsCheckedListBox.SelectedIndex > 0)
                    jobsCheckedListBox.SelectedIndex = jobsCheckedListBox.SelectedIndex - 1;
                
                if (jobsCheckedListBox.SelectedIndex < jobsCheckedListBox.SelectedIndex - 0)
                    jobsCheckedListBox.SelectedIndex = jobsCheckedListBox.SelectedIndex + 1;

                jobsCheckedListBox.Items.Remove(killme);
            }
        }

        private void Thumbnail_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PictureBox_DoubleClick(object sender, EventArgs e)
        {
            // Get the file's information.
            PictureBox pic = sender as PictureBox;
            FileInfo file_into = pic.Tag as FileInfo;

            // "Start" the file.
            Process.Start(file_into.FullName);
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}