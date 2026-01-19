using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Tax.Models;

namespace Tax.BAL
{
    public class TranHelper
    {
        private readonly NpgsqlConnection _conn;

        public TranHelper(NpgsqlConnection conn)
        {
            _conn = conn;
        }

        //============= Show Data ================
        public List<t_tran> Show(int userId)
        {
            List<t_tran> list = new();
            string q = "SELECT * FROM t_tran WHERE c_userid=@uid";

            _conn.Open();
            var cmd = new NpgsqlCommand(q, _conn);
            cmd.Parameters.AddWithValue("uid", userId);

            var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new t_tran
                {
                    t_tranid = (int)r["t_tranid"],
                    c_userid = (int)r["c_userid"],
                    t_tranname = r["t_tranname"].ToString(),
                    t_trantype = r["t_trantype"].ToString(),
                    t_taxable = r["t_taxable"].ToString(),
                    t_taxamount = (int)r["t_taxamount"]
                });
            }
            _conn.Close();
            return list;
        }

        //============== Add Data ===========================
        public bool Add(t_tran t)
        {
            string q = @"INSERT INTO t_tran
                        (c_userid,t_tranname,t_trantype,t_taxable,t_taxamount)
                        VALUES(@c_userid,@t_tranname,@t_trantype,@t_taxable,@t_taxamount)";
            _conn.Open();
            var cmd = new NpgsqlCommand(q, _conn);
            cmd.Parameters.AddWithValue("c_userid", t.c_userid);
            cmd.Parameters.AddWithValue("t_tranname", t.t_tranname);
            cmd.Parameters.AddWithValue("t_trantype", t.t_trantype);
            cmd.Parameters.AddWithValue("t_taxable", t.t_taxable);
            cmd.Parameters.AddWithValue("t_taxamount", t.t_taxamount);
            int r = cmd.ExecuteNonQuery();
            _conn.Close();
            return r > 0;
        }

        // =========== Update Data ================
        public bool Update(t_tran t)
        {
            string q = @"UPDATE t_tran SET
                        t_tranname=@t_tranname,
                        t_trantype=@t_trantype,
                        t_taxable=@t_taxable,
                        t_taxamount=@t_taxamount
                        WHERE t_tranid=@t_tranid";
            _conn.Open();
            var cmd = new NpgsqlCommand(q, _conn);
            cmd.Parameters.AddWithValue("t_tranname", t.t_tranname);
            cmd.Parameters.AddWithValue("t_trantype", t.t_trantype);
            cmd.Parameters.AddWithValue("t_taxable", t.t_taxable);
            cmd.Parameters.AddWithValue("t_taxamount", t.t_taxamount);
            cmd.Parameters.AddWithValue("t_tranid", t.t_tranid);
            int r = cmd.ExecuteNonQuery();
            _conn.Close();
            return r > 0;
        }

        //============= Delete Data =====================
        public bool Delete(int id)
        {
            _conn.Open();
            var cmd = new NpgsqlCommand(
                "DELETE FROM t_tran WHERE t_tranid=@id", _conn);
            cmd.Parameters.AddWithValue("id", id);
            int r = cmd.ExecuteNonQuery();
            _conn.Close();
            return r > 0;
        }

        // ============ Total Icome =============
        public int totalincome(int id)
        {
            string q = @"SELECT COALESCE(SUM(t_taxamount),0)
                         FROM t_tran WHERE c_userid=@id AND t_trantype='Income'";
            _conn.Open();
            var cmd = new NpgsqlCommand(q, _conn);
            cmd.Parameters.AddWithValue("id", id);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            _conn.Close();
            return r;
        }

        // =============== nonTax ==============
        public int nontax(int id)
        {
            string q = @"SELECT COALESCE(SUM(t_taxamount),0)
                 FROM t_tran
                 WHERE c_userid=@id 
                 AND t_trantype='Expense'
                 AND t_taxable='false'";
            _conn.Open();
            var cmd = new NpgsqlCommand(q, _conn);
            cmd.Parameters.AddWithValue("id", id);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            _conn.Close();
            return r;
        }

        //====================== Tax =======================
        public int tax(int id)
        {
            string q = @"SELECT COALESCE(SUM(t_taxamount),0)
                 FROM t_tran
                 WHERE c_userid=@id 
                 AND t_trantype='Expense'
                 AND t_taxable='true'";
            _conn.Open();
            var cmd = new NpgsqlCommand(q, _conn);
            cmd.Parameters.AddWithValue("id", id);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            _conn.Close();
            return r;
        }

    }
}