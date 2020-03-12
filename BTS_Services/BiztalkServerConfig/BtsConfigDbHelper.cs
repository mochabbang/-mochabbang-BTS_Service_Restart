using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Management;
using Microsoft.Win32;

namespace BTS_Services.BiztalkServerConfig
{
    class BtsConfigDbHelper
    {
        /// <summary>
        /// Util class to get the connectionstring.
        /// Which is usefull if the BizTalk DB is installed on a different service.
        /// </summary>
        public class BtsConfigurationDatabase
        {
            private string _database;
            private string _server;
            private static BtsConfigurationDatabase _btsConfig = null;

            private BtsConfigurationDatabase()
            {
                _server = string.Empty;
                _database = string.Empty;
            }

            private bool GetUsingWMI()
            {
                bool regFounded = false;
                _server = string.Empty;
                _database = string.Empty;
                try
                {
                    using (ManagementObjectSearcher searcherGroupSetting = new ManagementObjectSearcher())
                    {
                        searcherGroupSetting.Scope = new ManagementScope(@"root\MicrosoftBizTalkServer");
                        searcherGroupSetting.Query = new SelectQuery("select * from MSBTS_GroupSetting");
                        using (
                            ManagementObjectCollection.ManagementObjectEnumerator enumGroupSetting =
                                searcherGroupSetting.Get().GetEnumerator())
                        {
                            while (enumGroupSetting.MoveNext())
                            {
                                ManagementObject groupSetting = (ManagementObject)enumGroupSetting.Current;
                                _server = groupSetting["MgmtDbServerName"] as string;
                                _database = groupSetting["MgmtDbName"] as string;
                                regFounded = true;
                                break;
                            }
                            return regFounded;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            private bool GetUsingRegistry()
            {
                bool regFounded = false;
                _server = string.Empty;
                _database = string.Empty;
                using (
                    RegistryKey keyBts =
                        Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration"))
                {
                    if (keyBts == null)
                    {
                        return regFounded;
                    }
                    _server = keyBts.GetValue("MgmtDBServer") as string;

                    if (_server == null)
                    {
                        _server = string.Empty;
                        _database = keyBts.GetValue("MgmtDBName") as string;
                    }
                    if (_database == null)
                    {
                        _database = string.Empty;
                    }
                    regFounded = true;
                }
                return regFounded;
            }

            /// <summary>
            /// Gets the connectionstring to the BizTalk SQL Server DB
            /// </summary>
            /// <returns></returns>
            public static string GetConnectionString()
            {
                if (_btsConfig == null)
                {
                    _btsConfig = new BtsConfigurationDatabase();
                }

                if (!_btsConfig.GetUsingWMI())
                {
                    _btsConfig.GetUsingRegistry();
                }

                if ((_btsConfig._server == string.Empty) || (_btsConfig._database == string.Empty))
                {
                    throw new ApplicationException("Error while trying to get the connection string!");
                }

                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
                sb.DataSource = _btsConfig._server;
                sb.InitialCatalog = _btsConfig._database;
                sb.IntegratedSecurity = true;
                return sb.ToString();
            }
        }
    }
}
