//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            this.Icon = Win.AssemblyRoutines.GetAppIcon();
            Text = Program.FullName + ": Settings";

            load_settings();
        }

        void load_settings()
        {
            AnchorBoxColor.ForeColor = Settings.Appearance.AnchorBoxColor;
            AscendantAnchorBoxColor.ForeColor = Settings.Appearance.AscendantAnchorBoxColor;
            SelectionBoxColor.ForeColor = Settings.Appearance.SelectionBoxColor;
            TableBoxColor.ForeColor = Settings.Appearance.TableBoxColor;
            SelectionBoxBorderWidth.Value = (decimal)Settings.Appearance.SelectionBoxBorderWidth;
            AnchorBoxBorderWidth.Value = (decimal)Settings.Appearance.AnchorBoxBorderWidth;
            AscendantAnchorBoxBorderWidth.Value = (decimal)Settings.Appearance.SelectionBoxBorderWidth;
            TableBoxBorderWidth.Value = (decimal)Settings.Appearance.TableBoxBorderWidth;

            PdfPageImageResolution.Value = Settings.Constants.PdfPageImageResolution;
            CoordinateDeviationMargin.Value = (decimal)Settings.Constants.CoordinateDeviationMargin;
            OcrConfig.Text = Settings.Constants.OcrConfig.ToStringByJson();

            InitialSearchRectangleMargin.Value = (decimal)Settings.Constants.InitialSearchRectangleMargin;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            try
            {
                AppearanceSettings appearance = Settings.Appearance.CreateClone();
                appearance.AnchorBoxColor = AnchorBoxColor.ForeColor;
                appearance.AscendantAnchorBoxColor = AscendantAnchorBoxColor.ForeColor;
                appearance.SelectionBoxColor = SelectionBoxColor.ForeColor;
                appearance.TableBoxColor = TableBoxColor.ForeColor;
                appearance.SelectionBoxBorderWidth = (float)SelectionBoxBorderWidth.Value;
                appearance.AnchorBoxBorderWidth = (float)AnchorBoxBorderWidth.Value;
                appearance.AscendantAnchorBoxBorderWidth = (float)AscendantAnchorBoxBorderWidth.Value;
                appearance.TableBoxBorderWidth = (float)TableBoxBorderWidth.Value;

                ConstantsSettings constants = Settings.Constants.CreateClone();
                constants.PdfPageImageResolution = (int)PdfPageImageResolution.Value;
                constants.CoordinateDeviationMargin = (float)CoordinateDeviationMargin.Value;
                //constants.OcrConfig = Serialization.Json.Deserialize<Ocr.Config>(OcrConfig.Text);
                constants.InitialSearchRectangleMargin = (int)InitialSearchRectangleMargin.Value;

                Settings.Appearance = appearance;
                Settings.Appearance.Save();
                Settings.Constants = constants;
                Settings.Constants.Save();
                Message.Warning("Some settings may require restarting the application in order to come into effect.", this);
                Close();
            }
            catch (Exception ex)
            {
                Message.Error2(ex, this);
            }
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            Settings.Appearance.Reset();
            Settings.Constants.Reset();
            load_settings();
        }

        private void About_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void SelectionBoxColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = Settings.Appearance.SelectionBoxColor;
            if (cd.ShowDialog() == DialogResult.OK)
                SelectionBoxColor.ForeColor = cd.Color;
        }

        private void AnchorMasterBoxColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = Settings.Appearance.AnchorBoxColor;
            if (cd.ShowDialog() == DialogResult.OK)
                AnchorBoxColor.ForeColor = cd.Color;
        }

        private void AnchorSecondaryBoxColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = Settings.Appearance.AscendantAnchorBoxColor;
            if (cd.ShowDialog() == DialogResult.OK)
                AscendantAnchorBoxColor.ForeColor = cd.Color;
        }

        private void TableBoxColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = Settings.Appearance.TableBoxColor;
            if (cd.ShowDialog() == DialogResult.OK)
                TableBoxColor.ForeColor = cd.Color;
        }
    }
}