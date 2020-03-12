using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.BizTalk.ExplorerOM;
using System.Data;

namespace BTS_Services.BiztalkServerConfig
{
    class BTSHostUtil
    {
        public static string[] GetALLData()
        {
            string[] strSearch = null;

            try
            {
                DataTable dt = new DataTable();
                dt = GetHostNameDataTable();

                strSearch = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strSearch[i] = dt.Rows[i]["HostName"].ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return strSearch;
        }

        public static string[] GetSearchData(string search)
        {
            string[] strSearch = null;

            try
            {
                if (!string.IsNullOrEmpty(search))
                {
                    
                    DataRow[] arrRows = null;

                    DataTable dt = new DataTable();
                    dt = GetHostNameDataTable();

                    arrRows = dt.Select("HostName like '%" + search + "%'");
                    strSearch = new string[arrRows.Length];

                    for (int i = 0; i < arrRows.Length; i++)
                    {
                        strSearch[i] = arrRows[i]["HostName"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }

            return strSearch;
        }

        private static DataTable GetHostNameDataTable()
        {
            try
            {
                BtsCatalogExplorer explorer = CreateBtsCatalogExplorer();

                HostCollection hosts = explorer.Hosts;

                DataTable dt = new DataTable();
                DataColumn dc = new DataColumn();
                dc.ColumnName = "HostName";
                dt.Columns.Add(dc);

                foreach (Host item in hosts)
                {
                    DataRow dr = dt.NewRow();
                    dr["HostName"] = item.Name;
                    dt.Rows.Add(dr);
                }                

                return dt;
            }
            catch (Exception)
            {
                
                throw;
            }            
        }

        private static BtsCatalogExplorer CreateBtsCatalogExplorer()
        {
            try
            {
                BtsCatalogExplorer explorer = new BtsCatalogExplorer();
                explorer.ConnectionString = BtsConfigDbHelper.BtsConfigurationDatabase.GetConnectionString();
                return explorer;
            }
            catch (Exception ex)
            {
                
                throw;
            }            
        }
    }

}
