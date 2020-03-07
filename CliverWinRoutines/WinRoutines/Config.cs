//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Cliver.Win
{
    abstract public class AppSettings : Settings
    {
        /// <summary>
        /// By defult Environment.SpecialFolder.CommonApplicationData does not have writting permission
        /// </summary>
        /// <param name="userIdentityName"></param>
        public static void AllowReadWriteConfig(string userIdentityName = null)
        {
            if (userIdentityName == null)
                userIdentityName = UserRoutines.GetCurrentUserName3();
            NTAccount a = new NTAccount(userIdentityName);
            allowReadWriteConfig((SecurityIdentifier)a.Translate(typeof(SecurityIdentifier)));
        }

        /// <summary>
        /// By defult Environment.SpecialFolder.CommonApplicationData does not have writting permission
        /// </summary>
        public static void AllowReadWriteConfigToEveryone()
        {
            allowReadWriteConfig(new SecurityIdentifier(WellKnownSidType.WorldSid, null));
        }

        static void allowReadWriteConfig(SecurityIdentifier securityIdentifier)
        {
            DirectoryInfo di = new DirectoryInfo(Cliver.AppSettings.StorageDir);
            DirectorySecurity ds = di.GetAccessControl();
            ds.AddAccessRule(new FileSystemAccessRule(
                    securityIdentifier,
                FileSystemRights.Modify,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow));
            di.SetAccessControl(ds);
        }
    }
}