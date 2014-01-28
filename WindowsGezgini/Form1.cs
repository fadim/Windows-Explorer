using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
//Directory:Klasör
//DriveInfo:Sürücü Bilgisi
//FileInfo:Dosya Bilgisi
//Path:Yol
//File :Dosya

namespace WindowsGezgini
{
    public partial class Form1 : Form
    {
        ArrayList liste = new ArrayList();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DriveInfo[] surucu=DriveInfo.GetDrives();
            for (int i = 0; i < surucu.Length; i++)
            {
                TreeNode tn = new TreeNode();
                tn.Text = surucu[i].Name;
                treeView1.Nodes.Add(tn);
                if (surucu[i].IsReady)
                {
                    KlasorListele(tn);    
                }
                
            }
        }

        void KlasorListele(TreeNode tn)
        {
            string[] klasorler = Directory.GetDirectories(tn.Text);
            for (int i = 0; i < klasorler.Length; i++)
            {
                TreeNode tn1 = new TreeNode();
                tn1.Text = klasorler[i].Substring(3);
                tn.Nodes.Add(tn1);
            }
        }

        void DosyaListele(string yol, string filtre)
        {
            //
            try
            {
                string[] dosyalar = Directory.GetFiles(yol, filtre);

                for (int i = 0; i < dosyalar.Length; i++)
                {
                    ListViewItem li = new ListViewItem();
                    li.Text = Path.GetFileName(dosyalar[i]);
                    listView1.Items.Add(li);

                    FileInfo fi = new FileInfo(dosyalar[i]);

                    //li.SubItems.Add(fi.Length.ToString());

                    string uzunluk = "";
                    if (fi.Length>1024*1024*1024)
                    {
                        uzunluk = Convert.ToString(fi.Length / (1024 * 1024 * 1024)) +"GB";
                    }
                    else if (fi.Length > 1024 * 1024 )
                    {
                        uzunluk = Convert.ToString(fi.Length / (1024 * 1024)) + "MB";
                    }
                    else if (fi.Length > 1024 )
                    {
                        uzunluk = Convert.ToString(fi.Length / 1024) + "KB";
                    }
                    else
                    {
                        uzunluk = Convert.ToString(fi.Length) + "Bytes";
                    }
                    li.SubItems.Add(uzunluk);
                    li.SubItems.Add(fi.DirectoryName);
                    this.Refresh();
                    Application.DoEvents();//klavye ve mouse tıklamalarına izin verir
                    lblDurum.Text = fi.DirectoryName;
                }
            }
            catch { }
        
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            txtADRES.Text = e.Node.FullPath;
            listView1.Items.Clear();
            DosyaListele(txtADRES.Text, "*.*");
        }

        private void txtADRES_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                listView1.Items.Clear();
                DosyaListele(txtADRES.Text, "*.*");
            }
        }

        private void listeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.List;
        }

        private void detayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Details;
        }

        private void küçükResimToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.SmallIcon;
        }

        private void büyükResimToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.LargeIcon;
        }

        private void kopyalaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //kopyala
            liste.Clear();
            yapıştırToolStripMenuItem.Enabled = true;
            for (int i = 0; i < listView1.SelectedItems.Count; i++)
            {
                //Dosyaadı,Boyutu,Klasör
                string dosyaadi = listView1.SelectedItems[i].SubItems[0].Text;
                string klasor = listView1.SelectedItems[i].SubItems[2].Text;
                liste.Add(klasor + "/" + dosyaadi);
            }
        }

        private void yapıştırToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //yapıştır
            yapıştırToolStripMenuItem.Enabled = false;
            Form f1 = new Form();
            f1.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            f1.StartPosition = FormStartPosition.CenterScreen;
            f1.Height = 50;
            f1.Width= 150;
            f1.BackColor = Color.Coral;
            ProgressBar pb = new ProgressBar();
            pb.Maximum = liste.Count;
            pb.Minimum = 0;
            pb.Height = 30;
            pb.Width = 130;
            pb.Left = 10;
            pb.Top = 10;
            f1.Controls.Add(pb);
            f1.TopMost = true;
            f1.Show();

            for (int i = 0; i < liste.Count; i++)
            {
                pb.Value = i+1;
                pb.Refresh();
                string kaynak = liste[i].ToString();
                string hedef = txtADRES.Text + "" + Path.GetFileName(kaynak);
                File.Copy(kaynak, hedef, true);
            }
            listView1.Items.Clear();
            DosyaListele(txtADRES.Text, "*.*");
            f1.Close();
        }

        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (listView1.SelectedItems.Count>0)
            {   string dosyaadi = listView1.SelectedItems[0].SubItems[0].Text;
                string klasor = listView1.SelectedItems[0].SubItems[2].Text;
                File.Delete(klasor + "/" + dosyaadi);
                listView1.Items.RemoveAt(0);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string dosyaadi = listView1.FocusedItem.SubItems[0].Text;
            string klasor = listView1.FocusedItem.SubItems[2].Text;
            System.Diagnostics.Process.Start(klasor + "/" + dosyaadi);
        }
        bool AramaDurumu = false;
        private void toolStripButton1_Click(object sender, EventArgs e)
        {//ara
            AramaDurumu = !AramaDurumu;
            if (AramaDurumu==true)
            {
                listView1.Items.Clear();
                DosyaAra(txtADRES.Text, txtARA.Text);
                lblDurum.Text = "Arama işlemi bitti";
            }
        }

        void DosyaAra(string yol,string filtre)
        {
            try
            {
                if (AramaDurumu == false) return;
                DosyaListele(yol, filtre);
                string[] klasor = Directory.GetDirectories(yol);
                for (int i = 0; i < klasor.Length; i++)
                {
                    DosyaAra(klasor[i], filtre);
                    if (AramaDurumu == false) return;
                }
            }
            catch { }
        }
    }
}
