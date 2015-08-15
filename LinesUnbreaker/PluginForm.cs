﻿using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
namespace Nikse.SubtitleEdit.PluginLogic
{
    internal partial class PluginForm : Form
    {
        public string FixedSubtitle { get; set; }
        //private string path = Path.Combine("Plugins", "SeLinesUnbreaker.xml");
        private Subtitle _subtitle;
        private XElement _xmlSetting = null;
        private bool _allowFixes;
        private int _totalFixed;
        private int _maxLineLength;

        public PluginForm()
        {
            InitializeComponent();
        }

        public PluginForm(Subtitle subtitle, string name, string description, Form parentForm)
            : this()
        {
            // TODO: Complete member initialization
            _subtitle = subtitle;
            /*
            Resize += delegate
            {
                listView1.Columns[listView1.Columns.Count - 1].Width = -2;
                //this.listViewFixes.Columns.Count -1
            };*/
            FormClosing += delegate
            {
                LoadSettingsIfThereIs(false); // the setting will be stored in xml file.
            };
            LoadSettingsIfThereIs(true);
            FindLines();
        }

        private void LoadSettingsIfThereIs(bool load)
        {
            string path = GetSettingsFileName();
            if (!File.Exists(path))
                return;
            try
            {
                if (load)
                {
                    if (File.Exists(path))
                    {
                        _xmlSetting = XElement.Load(path);
                        int val;
                        int.TryParse(_xmlSetting.Element("Shorterthan").Value, out val);
                        if (val > 0)
                            numericUpDown1.Value = val;

                        checkBoxSkipDialog.Checked = bool.Parse(_xmlSetting.Element("SkipDialog").Value);
                        checkBoxSkipNarrator.Checked = bool.Parse(_xmlSetting.Element("SkipNarrator").Value);
                        checkBoxMoods.Checked = bool.Parse(_xmlSetting.Element("SkipMoods").Value);
                    }
                    else
                    {
                        _xmlSetting = new XElement("SeLinesUnbreaker",
                            new XElement("Shorterthan", numericUpDown1.Value),
                            new XElement("SkipDialog", true),
                            new XElement("SkipNarrator", true),
                            new XElement("SkipMoods", false)
                            );
                        _xmlSetting.Save(path);
                    }
                }
                else
                {
                    if (_xmlSetting == null)
                        return;
                    _xmlSetting.Element("Shorterthan").Value = numericUpDown1.Value.ToString();
                    _xmlSetting.Element("SkipMoods").Value = checkBoxMoods.Checked.ToString();
                    _xmlSetting.Element("SkipNarrator").Value = checkBoxSkipNarrator.Checked.ToString();
                    _xmlSetting.Element("SkipDialog").Value = checkBoxSkipDialog.Checked.ToString();
                    _xmlSetting.Save(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string GetSettingsFileName()
        {
            // "C:\Users\Ivandrofly\Desktop\SubtitleEdit\Plugins\SeLinesUnbreaker.xml"
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (path.StartsWith("file:\\", StringComparison.Ordinal))
                path = path.Remove(0, 6);
            path = Path.Combine(path, "Plugins");
            if (!Directory.Exists(path))
                path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Subtitle Edit"), "Plugins");
            return Path.Combine(path, "SeLinesUnbreaker.xml");
        }

        private void FindLines()
        {
            _totalFixed = 0;
            listView1.BeginUpdate();
            _maxLineLength = (int)numericUpDown1.Value;
            foreach (Paragraph p in _subtitle.Paragraphs)
            {
                if (p.NumberOfLines < 2)
                    continue;
                string oldText = p.Text;
                string text = p.Text;

                text = UnbreakLines(text);
                if (text != oldText)
                {
                    text = Regex.Replace(text, " +" + Environment.NewLine, Environment.NewLine).Trim();
                    text = Regex.Replace(text, Environment.NewLine + " +", Environment.NewLine).Trim();

                    if (AllowFix(p))
                    {
                        p.Text = text;
                    }
                    else
                    {
                        if (!_allowFixes)
                        {
                            oldText = Utilities.RemoveHtmlTags(oldText);
                            text = Utilities.RemoveHtmlTags(text);
                            AddFixToListView(p, oldText, text, text.Length.ToString());
                            _totalFixed++;
                        }
                    }
                }
            }

            if (!_allowFixes)
            {
                labelTotal.Text = string.Format("Total: {0}", _totalFixed);
                labelTotal.ForeColor = _totalFixed < 1 ? Color.Red : Color.Green;
            }
            listView1.EndUpdate();
            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private bool AllowFix(Paragraph p)
        {
            if (!_allowFixes)
                return false;
            string ln = p.Number.ToString();
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[1].Text == ln)
                    return item.Checked;
            }
            return false;
        }

        private void AddFixToListView(Paragraph p, string before, string after, string lineLength)
        {
            var item = new ListViewItem() { Checked = true, UseItemStyleForSubItems = true, Tag = p };
            item.SubItems.Add(p.Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
            item.SubItems.Add(lineLength);
            item.SubItems.Add(before.Replace(Environment.NewLine, Configuration.ListViewLineSeparatorString));
            item.SubItems.Add(after.Replace(Environment.NewLine, Configuration.ListViewLineSeparatorString));
            listView1.Items.Add(item);
        }

        private string UnbreakLines(string s)
        {
            var temp = Utilities.RemoveHtmlTags(s, true);
            temp = temp.Replace("♪", string.Empty).Replace("♫", string.Empty);
            temp = temp.Replace("  ", " ").Trim();

            // TODO: move these methods in Utilities's helper method
            if ((temp.StartsWith('-') || temp.Contains("\r\n-")) && checkBoxSkipDialog.Checked)
            {
                return s;
            }
            if ((temp.Contains('[') || temp.Contains('(')) && checkBoxMoods.Checked)
            {
                return s;
            }
            if (Regex.IsMatch(temp, ":\\B") && checkBoxSkipNarrator.Checked)
            {
                return s;
            }

            temp = Utilities.UnbreakLine(temp);
            if (temp.Length < _maxLineLength)
            {
                s = Utilities.UnbreakLine(s);
            }
            return s;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            _totalFixed = 0;
            buttonUpdate.Enabled = false;
            FindLines();
            buttonUpdate.Enabled = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _allowFixes = true;
            FindLines();
            FixedSubtitle = _subtitle.ToText(new SubRip());
            DialogResult = DialogResult.OK;
        }

        private void PluginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.Cancel;
        }

        private void SelectionHandler(object sender, EventArgs e)
        {
            if (listView1.Items.Count <= 0)
                return;
            listView1.BeginUpdate();
            if (sender == buttonCheckAll)
                foreach (ListViewItem item in this.listView1.Items)
                    item.Checked = true;
            else
                foreach (ListViewItem item in this.listView1.Items)
                    item.Checked = !item.Checked;
            listView1.EndUpdate();
        }

        private void PluginForm_Resize(object sender, EventArgs e)
        {
            var totalWidth = 0;
            for (int i = 0; i < 3; i++)
            {
                totalWidth += listView1.Columns[i].Width;
            }
            var half = (listView1.Width - totalWidth) >> 1;
            listView1.Columns[3].Width = half;
            listView1.Columns[4].Width = half;
        }
    }
}