//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************

namespace Cliver.SampleParser
{
    public partial class Settings
    {
        public static GeneralSettings General;
    }

    public class GeneralSettings : Cliver.UserSettings
    {
        public string InputFolder;
        public string OutputFolder;

        protected override void Loaded()
        {
            if (string.IsNullOrWhiteSpace(InputFolder))
                InputFolder = ProgramRoutines.GetAppDirectory();
            if (string.IsNullOrWhiteSpace(OutputFolder))
                OutputFolder = ProgramRoutines.GetAppDirectory();
        }

        protected override void Saving()
        {
        }
    }
}
