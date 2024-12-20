using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ReportesWH.Models;
using System.Data.SqlClient;

public class COracle
{
    String m_server;
    String m_SID;
    private String m_user;
    private String m_pass;
    OracleConnection m_OracleDB;
    private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    public string Server { get => m_server; set => m_server = value; }
    public string SID { get => m_SID; set => m_SID = value; }
    public string conexion = "";

    public COracle(String serv, String Sid)
    {
        m_server = serv;
        m_SID = Sid;
        m_user = "APPS";
        m_pass = "apps";
        m_OracleDB = GetDBConnection();
        m_OracleDB.Open();
    }

    private OracleConnection GetDBConnection()
    {

        Console.WriteLine("Getting Connection ...");

        // 'Connection string' to connect directly to Oracle.
        string connString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = "
             + Server + ")(PORT = " + "1521" + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = "
             + SID + ")));Password=" + m_pass + ";User ID=" + m_user + ";Enlist=false;Pooling=true;Connection Timeout=10000;";

        OracleConnection conn = new OracleConnection();

        try
        {
            conn.ConnectionString = connString;
            conexion = conn.State.ToString();
        }
        catch (Exception ex)
        {
            conexion = conn.State.ToString();
            conn = null;
            logger.Error(ex, "Error al conectarse a base de datos de Oracle");
        }
        
        return conn;
    }
    public DataTable getReportSI(DateTime ds, DateTime de)
    {
        DataTable report = null;

        string sql = "SELECT 'PCK(TRAY)' AS DESCRIPTION,COUNT(*) as QTY FROM SIIXSEM.INCOMING_LOT_DETAILS WHERE CREATED_DT >= '01-" + ds.ToString("MM-yyyy") + "' AND RECEIPT_NUM<> 'OPEN-DO-001' AND CREATED_DT <= '01-" + de.ToString("MM-yyyy") + "' AND ITEM_NAME LIKE 'PCK%' " +
            "UNION " +
            "SELECT 'CHINA' AS DESCRIPTION, COUNT(*) as QTY FROM SIIXSEM.INCOMING_LOT_DETAILS WHERE CREATED_DT >= '01-" + ds.ToString("MM-yyyy") + "' AND RECEIPT_NUM<> 'OPEN-DO-001' AND CREATED_DT <= '01-" + de.ToString("MM-yyyy") + "' AND ITEM_NAME IN(SELECT ITEM_CD FROM SIIXSEM.M_ITEM_SPECIFICATIONS WHERE TRADE_FG_FLAG = 'Y') " +
            "UNION " +
            "SELECT 'RECEIVING' AS DESCRIPTION, COUNT(*) as QTY FROM SIIXSEM.INCOMING_LOT_DETAILS WHERE CREATED_DT >= '01-" + ds.ToString("MM-yyyy") + "' AND RECEIPT_NUM<> 'OPEN-DO-001' AND CREATED_DT <= '01-" + de.ToString("MM-yyyy") + "' AND ITEM_NAME NOT LIKE 'PCK%' AND ITEM_NAME NOT IN(SELECT ITEM_CD FROM SIIXSEM.M_ITEM_SPECIFICATIONS WHERE TRADE_FG_FLAG = 'Y')";
        try
        {
            // Create command.
            OracleCommand cmd = new OracleCommand();

            // Set connection for command.
            cmd.Connection = m_OracleDB;
            cmd.CommandText = sql;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    report = new DataTable();

                    ///LINKAGE_SEQ, ASSEMBLY_DESC,PAIR_FG_NAME, ASSEMBLY_NAME, BACKFLUSH_DIVIDER, DJ_QTY
                    report.Columns.Add("DESCRIPTION");
                    report.Columns.Add("QTY");
                    while (reader.Read())
                    {
                        try
                        {
                            report.Rows.Add(reader.GetString(0).ToUpper(), reader.GetInt32(1));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("arsehole");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[QuerySerial] Error ");
        }
        return report;
    }
    public DataTable getReportMO(DateTime ds, DateTime de)
    {
        DataTable report = null;

        string sql1 = "SELECT 'SMT Picking' AS DESCRIPTION, COUNT ( * ) AS QTY FROM SIIXSEM.DJ_MASTER_PICK_LIST WHERE CREATED_DT >= '01-" + ds.ToString("MM-yyyy") + "' AND CREATED_DT< '01-" + de.ToString("MM-yyyy") + "' AND PICKED_FLAG = 'Y' AND SUPPLY_SUBINV = 'SMT1'" +
            "UNION " +
            "SELECT 'ASSY Picking' AS DESCRIPTION, COUNT ( * ) AS QTY FROM SIIXSEM.DJ_MASTER_PICK_LIST WHERE CREATED_DT >= '01-" + ds.ToString("MM-yyyy") + "' AND CREATED_DT< '01-" + de.ToString("MM-yyyy") + "' AND PICKED_FLAG = 'Y' AND SUPPLY_SUBINV = 'ASSY'";
        try
        {
            // Create command.
            OracleCommand cmd = new OracleCommand();

            // Set connection for command.
            cmd.Connection = m_OracleDB;
            cmd.CommandText = sql1;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    report = new DataTable();

                    ///LINKAGE_SEQ, ASSEMBLY_DESC,PAIR_FG_NAME, ASSEMBLY_NAME, BACKFLUSH_DIVIDER, DJ_QTY
                    report.Columns.Add("DESCRIPTION");
                    report.Columns.Add("QTY");
                    while (reader.Read())
                    {
                        try
                        {
                            report.Rows.Add(reader.GetString(0).ToUpper(), reader.GetInt32(1));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("arsehole");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[QuerySerial] Error ");
        }
        return report;
    }
    public DataTable getReportFP(DateTime ds, DateTime de)
    {
        DataTable report = null;
        var db = new siixsem_wh_monthly_reportsEntities1();
        int dsm = int.Parse(ds.ToString("yyyyMM"));
        int dem = int.Parse(de.ToString("yyyyMM"));
        int id1 = 0;
        int id2 = 0;

        var ids = db.GetIDMaterial(dsm, dem).ToList();
        id1 = int.Parse(ids.First().ToString());

        if (ids.Count == 0 || ids.Count == 1)
        {
            string sqlid = "SELECT TRANSACTION_ID FROM INV.MTL_MATERIAL_TRANSACTIONS WHERE (CREATION_DATE >= '01-" + de.ToString("MM-yyyy") + "' AND CREATION_DATE < '02-" + de.ToString("MM-yyyy") + "') AND ROWNUM = 1";
            try
            {
                // Create command.
                OracleCommand icmd = new OracleCommand();

                // Set connection for command.
                icmd.Connection = m_OracleDB;
                icmd.CommandText = sqlid;

                using (DbDataReader reader = icmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        report = new DataTable();

                        ///LINKAGE_SEQ, ASSEMBLY_DESC,PAIR_FG_NAME, ASSEMBLY_NAME, BACKFLUSH_DIVIDER, DJ_QTY
                        report.Columns.Add("TRANSACTION_ID");
                        while (reader.Read())
                        {
                            try
                            {
                                id2 = reader.GetInt32(0);
                                var iid = db.InsertIDMaterial(dem,de.ToString("MM-yyyy"),id2);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("arsehole");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[QuerySerial] Error ");
            }
        }
        else if(ids.Count == 2)
        {
            id2 = int.Parse(ids.Last().ToString());
        }

        string sql1 = "SELECT 'Free Pick and SubInventory SMT' AS DESCRIPTION, COUNT(*) AS QTY FROM INV.MTL_MATERIAL_TRANSACTIONS WHERE(TRANSACTION_ID >= "+id1+ " AND TRANSACTION_ID < " + id2 + ") AND SUBINVENTORY_CODE = 'SMT1' AND(SOURCE_CODE = 'SUBINVENTORY TRANSFER' OR SOURCE_CODE LIKE 'FREE PICK%')" +
            "UNION " +
            "SELECT 'Free Pick and SubInventory ASSY' AS DESCRIPTION, COUNT(*) AS QTY FROM INV.MTL_MATERIAL_TRANSACTIONS WHERE(TRANSACTION_ID >= " + id1 + " AND TRANSACTION_ID < " + id2 + ") AND SUBINVENTORY_CODE = 'ASSY' AND(SOURCE_CODE = 'SUBINVENTORY TRANSFER' OR SOURCE_CODE LIKE 'FREE PICK%')";
        try
        {
            // Create command.
            OracleCommand cmd = new OracleCommand();

            // Set connection for command.
            cmd.Connection = m_OracleDB;
            cmd.CommandText = sql1;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    report = new DataTable();

                    ///LINKAGE_SEQ, ASSEMBLY_DESC,PAIR_FG_NAME, ASSEMBLY_NAME, BACKFLUSH_DIVIDER, DJ_QTY
                    report.Columns.Add("DESCRIPTION");
                    report.Columns.Add("QTY");
                    while (reader.Read())
                    {
                        try
                        {
                            report.Rows.Add(reader.GetString(0).ToUpper(), reader.GetInt32(1));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("arsehole");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[QuerySerial] Error ");
        }
        return report;
    }
    public DataTable getReportIV(DateTime ds, DateTime de)
    {
        DataTable report = null;
        var db = new siixsem_wh_monthly_reportsEntities1();
        int dsm = int.Parse(ds.ToString("yyyyMM"));
        int dem = int.Parse(de.ToString("yyyyMM"));

        var ids = db.GetIDMaterial(dsm, dem).ToList();
        int id1 = int.Parse(ids.First().ToString());
        int id2 = int.Parse(ids.Last().ToString());

        string sql2 = "SELECT COUNT ( * ) FROM INV.MTL_MATERIAL_TRANSACTIONS where TRANSACTION_ID >= " + id1 + " AND TRANSACTION_ID < " + id2 + " AND SUBINVENTORY_CODE = 'CONS' AND TRANSFER_SUBINVENTORY = 'IWH' AND SOURCE_CODE = 'SUBINVENTORY TRANSFER'";
        try
        {
            // Create command.
            OracleCommand cmd = new OracleCommand();

            // Set connection for command.
            cmd.Connection = m_OracleDB;
            cmd.CommandText = sql2;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    report = new DataTable();

                    ///LINKAGE_SEQ, ASSEMBLY_DESC,PAIR_FG_NAME, ASSEMBLY_NAME, BACKFLUSH_DIVIDER, DJ_QTY
                    report.Columns.Add("QTY");
                    while (reader.Read())
                    {
                        try
                        {
                            report.Rows.Add(reader.GetInt32(0));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("arsehole");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[QuerySerial] Error ");
        }
        return report;
    }
    public DataTable getReportSW(DateTime ds, DateTime de)
    {
        DataTable report = null;
        int dsm = int.Parse(ds.ToString("yyyyMM"));
        int dem = int.Parse(de.ToString("yyyyMM"));

        string sql2 = "SELECT COUNT(*) AS QTY  FROM SIIXSEM.RETURN_FROM_PRODUCTION WHERE FROM_SUBINV = 'SMT1' AND TO_SUBINV = 'IWH' AND TO_LOCATOR = 'RETURN' AND CREATED_DT >= '01-" + ds.ToString("MM-yyyy") + "' AND CREATED_DT <= '01-" + de.ToString("MM-yyyy") + "'";
        try
        {
            // Create command.
            OracleCommand cmd = new OracleCommand();

            // Set connection for command.
            cmd.Connection = m_OracleDB;
            cmd.CommandText = sql2;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    report = new DataTable();

                    ///LINKAGE_SEQ, ASSEMBLY_DESC,PAIR_FG_NAME, ASSEMBLY_NAME, BACKFLUSH_DIVIDER, DJ_QTY
                    report.Columns.Add("QTY");
                    while (reader.Read())
                    {
                        try
                        {
                            report.Rows.Add(reader.GetInt32(0));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("arsehole");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[QuerySerial] Error ");
        }
        return report;
    }
    public DataTable getReportFG(DateTime ds, DateTime de)
    {
        DataTable report = null;

        string sql2 = "SELECT COUNT ( * ) AS QTY FROM SIIXSEM.PACKING_HDR WHERE CREATED_DT >= '01-" + ds.ToString("MM-yyyy") + "' AND CREATED_DT < '01-" + de.ToString("MM-yyyy") + "'" +
        "UNION " +
        "SELECT COUNT( * ) AS QTY FROM SIIXSEM.SHIPPING_PALLET_DTL WHERE CREATED_DT >= '01-" + ds.ToString("MM-yyyy") + "' AND CREATED_DT< '01-" + de.ToString("MM-yyyy") + "'";
        try
        {
            // Create command.
            OracleCommand cmd = new OracleCommand();

            // Set connection for command.
            cmd.Connection = m_OracleDB;
            cmd.CommandText = sql2;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    report = new DataTable();

                    ///LINKAGE_SEQ, ASSEMBLY_DESC,PAIR_FG_NAME, ASSEMBLY_NAME, BACKFLUSH_DIVIDER, DJ_QTY
                    report.Columns.Add("QTY");
                    while (reader.Read())
                    {
                        try
                        {
                            report.Rows.Add(reader.GetInt32(0));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("arsehole");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[QuerySerial] Error ");
        }
        return report;
    }
    public void Close()
    {
        m_OracleDB.Dispose();
        m_OracleDB.Close();
        OracleConnection.ClearPool(m_OracleDB);
    }

}
