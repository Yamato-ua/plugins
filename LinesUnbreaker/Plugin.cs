﻿using System.Windows.Forms;

namespace Nikse.SubtitleEdit.PluginLogic
{
    public class LinesUnbreaker : IPlugin
    {
        // Metadata
        string IPlugin.Name => "Lines Unbreaker";
        string IPlugin.Text => "Lines Unbreaker";
        decimal IPlugin.Version => 1.3M;
        string IPlugin.Description => "Helps unbreaking short-lines.";
        string IPlugin.ActionType => "tool";
        string IPlugin.Shortcut => string.Empty;

        string IPlugin.DoAction(Form parentForm, string subtitle, double frameRate, string listViewLineSeparatorString, string subtitleFileName, string videoFileName, string rawText)
        {
            if (string.IsNullOrWhiteSpace(subtitle))
            {
                MessageBox.Show("No subtitle loaded", parentForm.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(listViewLineSeparatorString))
                Configuration.ListViewLineSeparatorString = listViewLineSeparatorString;

            var list = subtitle.SplitToLines();

            var srt = new SubRip();
            var sub = new Subtitle(srt);
            srt.LoadSubtitle(sub, list, subtitleFileName);
            if (srt.ErrorCount > 0)
            {
                MessageBox.Show(srt.Errors + " Errors found while parsing .srt",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            using (var form = new PluginForm(sub))
            {
                if (form.ShowDialog(parentForm) == DialogResult.OK)
                    return form.FixedSubtitle;
            }
            return string.Empty;
        }
    }
}