/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        sergey.stoyan@hotmail.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/

using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Cliver.Win
{
    /// <summary>
    /// Inheritor of this class is stored in CommonApplicationData folder.
    /// It is a Windows adapted version of AppSettings which provides necessary routines.
    /// </summary>
    public class AppSettings : Cliver.AppSettings
    {
        /// <summary>
        /// By default Environment.SpecialFolder.CommonApplicationData does not have writting permission.
        /// Use this method to set it up.
        /// </summary>
        /// <param name="userIdentityName"></param>
        public static void AllowReadWriteConfigDir(string userIdentityName = null)
        {
            if (userIdentityName == null)
                userIdentityName = UserRoutines.GetCurrentUserName3();
            NTAccount a = new NTAccount(userIdentityName);
            allowReadWriteConfigDir((SecurityIdentifier)a.Translate(typeof(SecurityIdentifier)));
        }

        /// <summary>
        /// By default Environment.SpecialFolder.CommonApplicationData does not have writting permission.
        /// Use this method to set it up.
        /// </summary>
        public static void AllowReadWriteConfigDirToEveryone()
        {
            allowReadWriteConfigDir(new SecurityIdentifier(WellKnownSidType.WorldSid, null));
        }

        static void allowReadWriteConfigDir(SecurityIdentifier securityIdentifier)
        {
            DirectoryInfo di = new DirectoryInfo(AppSettings.StorageDir);
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