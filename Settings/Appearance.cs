//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************

namespace Cliver.PdfDocumentParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly AppearanceSettings Appearance;

        public class AppearanceSettings : Cliver.Settings
        {
            public System.Drawing.Color SelectionBoxColor = System.Drawing.Color.Red;
            public System.Drawing.Color AnchorBoxColor = System.Drawing.Color.Violet;
            public System.Drawing.Color AscendantAnchorBoxColor = System.Drawing.Color.BlueViolet;
            public System.Drawing.Color TableBoxColor = System.Drawing.Color.Blue;

            public float SelectionBoxBorderWidth = 1;//MS:You can access the unit of measure of the Graphics object using its PageUnit property. The unit of measure is typically pixels. A Width of 0 will result in the Pen drawing as if the Width were 1.
            public float AnchorBoxBorderWidth = 1;
            public float AscendantAnchorBoxBorderWidth = 1;
            public float TableBoxBorderWidth = 1;

            public override void Loaded()
            {
            }

            public override void Saving()
            {
            }
        }
    }
}