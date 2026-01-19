using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LIBRARY.Models;
using Npgsql;

namespace library.BAL
{
    public class UserHelper
    {
        private readonly NpgsqlConnection _conn;

        public UserHelper(NpgsqlConnection connection)
        {
            _conn = connection;
        }


        // ============= Register ===========
        public async Task<int> Register(t_libraryuser data)
        {
            try
            {
                await _conn.OpenAsync();

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM t_libraryuser WHERE c_email=@e", _conn))
                {
                    cmd.Parameters.AddWithValue("@e", data.c_email);
                    long count = (long)await cmd.ExecuteScalarAsync();
                    if (count > 0)
                        return -1;
                }

                using var insert = new NpgsqlCommand(@"
                    INSERT INTO t_libraryuser
                    (c_username,c_email,c_password,c_department,c_mobile,c_status,c_image)
                    VALUES (@c_username,@c_email,@c_password,@c_department,@c_mobile,@c_status,@c_image)
                ", _conn);

                insert.Parameters.AddWithValue("@c_username", data.c_username);
                insert.Parameters.AddWithValue("@c_email", data.c_email);
                insert.Parameters.AddWithValue("@c_password", data.c_password);
                insert.Parameters.AddWithValue("@c_department", data.c_department);
                insert.Parameters.AddWithValue("@c_mobile", data.c_mobile);
                insert.Parameters.AddWithValue("@c_status", data.c_status);
                insert.Parameters.AddWithValue("@c_image", data.c_image ?? "default.png");

                await insert.ExecuteNonQueryAsync();
                return 1;
            }
            finally { await _conn.CloseAsync(); }
        }

        // ---------------- LOGIN ----------------
        public async Task<t_libraryuser?> Login(string email, string pwd)
        {
            t_libraryuser? user = null;
            try
            {
                await _conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM t_libraryuser WHERE c_email=@e AND c_password=@p", _conn);

                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@p", pwd);

                using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    string img = r["c_image"].ToString();
                    if (!img.StartsWith("/"))
                        img = "/Image/" + img;

                    user = new t_libraryuser
                    {
                        c_userid = r.GetInt32(r.GetOrdinal("c_userid")),
                        c_username = r["c_username"].ToString(),
                        c_email = r["c_email"].ToString(),
                        c_password = r["c_password"].ToString(),
                        c_department = r["c_department"].ToString(),
                        c_mobile = r["c_mobile"].ToString(),
                        c_status = r["c_status"].ToString(),
                        c_image = img,
                        c_role = r["c_role"].ToString()
                    };
                }
            }
            finally { await _conn.CloseAsync(); }

            return user;
        }

        // ---------- GET USER ----------
        public async Task<t_libraryuser?> GetUserById(int id)
        {
            t_libraryuser? user = null;

            try
            {
                await _conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM t_libraryuser WHERE c_userid=@id", _conn);

                cmd.Parameters.AddWithValue("@id", id);

                using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    string img = r["c_image"]?.ToString();
                    img = string.IsNullOrEmpty(img) ? "/Image/default.png" : "/Image/" + img;

                    user = new t_libraryuser
                    {
                        c_userid = r.GetInt32(0),
                        c_username = r["c_username"].ToString(),
                        c_password = r["c_password"].ToString(),
                        c_email = r["c_email"].ToString(),
                        c_status = r["c_status"].ToString(),
                        c_mobile = r["c_mobile"].ToString(),
                        c_department = r["c_department"].ToString(),
                        c_image = img
                    };
                }
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return user;
        }

    }
}