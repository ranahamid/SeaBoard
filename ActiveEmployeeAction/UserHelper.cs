using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveEmployeeAction
{
    public class UserHelper
    {

        [Flags]
        public enum UserFlags
        {
            Script = 1,                                     // 0x1
            AccountDisabled = 2,                            // 0x2
            HomeDirectoryRequired = 8,                      // 0x8
            AccountLockedOut = 16,                          // 0x10
            PasswordNotRequired = 32,                       // 0x20
            PasswordCannotChange = 64,                      // 0x40
            EncryptedTextPasswordAllowed = 128,             // 0x80
            TempDuplicateAccount = 256,                     // 0x100
            NormalAccount = 512,                            // 0x200
            InterDomainTrustAccount = 2048,                 // 0x800
            WorkstationTrustAccount = 4096,                 // 0x1000
            ServerTrustAccount = 8192,                      // 0x2000
            PasswordDoesNotExpire = 65536,                  // 0x10000 (Also 66048 )
            MnsLogonAccount = 131072,                       // 0x20000
            SmartCardRequired = 262144,                     // 0x40000
            TrustedForDelegation = 524288,                  // 0x80000
            AccountNotDelegated = 1048576,                  // 0x100000
            UseDesKeyOnly = 2097152,                        // 0x200000
            DontRequirePreauth = 4194304,                   // 0x400000
            PasswordExpired = 8388608,                      // 0x800000 (Applicable only in Window 2000 and Window Server 2003)
            TrustedToAuthenticateForDelegation = 16777216,  // 0x1000000
            NoAuthDataRequired = 33554432                   // 0x2000000
        }

        public static bool IsEmployeeActive(string sAMAccountName, string domain)
        {
            bool active = true;

            var principalContext = new PrincipalContext(ContextType.Domain, domain);
            var userPrincipal = UserPrincipal.FindByIdentity(principalContext, sAMAccountName);
            if (userPrincipal != null)
            {
                var dirEntry = userPrincipal.GetUnderlyingObject() as DirectoryEntry;
                active = !IsAccountDisabled(dirEntry);
            }
            else
            {
                // User with this sAMAccountName does not exist
                active = false;
            }

            return active;
        }

        private static bool IsAccountDisabled(DirectoryEntry user)
        {
            const string uac = "userAccountControl";
            if (user.NativeGuid == null) return false;

            if (user.Properties[uac] != null && user.Properties[uac].Value != null)
            {
                var userFlags = (UserFlags)user.Properties[uac].Value;

                bool flag_value = userFlags.HasFlag(UserFlags.AccountDisabled);

                return flag_value;
            }

            return false;
        }

    }

}
