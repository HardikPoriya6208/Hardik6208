using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Tax.Models;

namespace Tax.BAL
{
    public class UserHelper
    {
        private readonly NpgsqlConnection _conn;

        public UserHelper(NpgsqlConnection connection)
        {
            _conn = connection;
        }


        // ---------------- Register ----------------
        public async Task<int> Register(t_taxuser data)
        {
            try
            {
                await _conn.OpenAsync();

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM t_taxuser WHERE c_email=@e", _conn))
                {
                    cmd.Parameters.AddWithValue("@e", data.c_email);
                    long count = (long)await cmd.ExecuteScalarAsync();
                    if (count > 0)
                        return -1;
                }

                using var insert = new NpgsqlCommand(@"
                    INSERT INTO t_taxuser
                    (c_username,c_email,c_password,c_mobile,c_state,c_city,c_gender,c_image,c_type)
                    VALUES (@c_username,@c_email,@c_password,@c_mobile,@c_state,@c_city,@c_gender,@c_image,@c_type)
                ", _conn);

                insert.Parameters.AddWithValue("@c_username", data.c_username);
                insert.Parameters.AddWithValue("@c_email", data.c_email);
                insert.Parameters.AddWithValue("@c_password", data.c_password);
                insert.Parameters.AddWithValue("@c_mobile", data.c_mobile);
                insert.Parameters.AddWithValue("@c_state", data.c_state);
                insert.Parameters.AddWithValue("@c_city", data.c_city);
                insert.Parameters.AddWithValue("@c_gender", data.c_gender);
                insert.Parameters.AddWithValue("@c_type", data.c_type);
                insert.Parameters.AddWithValue("@c_image", data.c_image ?? "default.png");

                await insert.ExecuteNonQueryAsync();
                return 1;
            }
            finally { await _conn.CloseAsync(); }
        }

        // ---------------- LOGIN ----------------
        public async Task<t_taxuser?> Login(string email, string pwd)
        {
            t_taxuser? user = null;
            try
            {
                await _conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM t_taxuser WHERE c_email=@e AND c_password=@p", _conn);

                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@p", pwd);

                using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    string img = r["c_image"].ToString();
                    if (!img.StartsWith("/"))
                        img = "/Image/" + img;

                    user = new t_taxuser
                    {
                        c_userid = r.GetInt32(r.GetOrdinal("c_userid")),
                        c_username = r["c_username"].ToString(),
                        c_email = r["c_email"].ToString(),
                        c_password = r["c_password"].ToString(),
                        c_mobile = r["c_mobile"].ToString(),
                        c_city = r["c_city"].ToString(),
                        c_state = r["c_state"].ToString(),
                        c_gender = r["c_gender"].ToString(),
                        c_type = r["c_type"].ToString(),
                        c_image = img,
                        c_role = r["c_role"].ToString()
                    };
                }
            }
            finally { await _conn.CloseAsync(); }

            return user;
        }

        // ---------- GET USER ----------
        public async Task<t_taxuser?> GetUserById(int id)
        {
            t_taxuser? user = null;

            try
            {
                await _conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM t_taxuser WHERE c_userid=@id", _conn);

                cmd.Parameters.AddWithValue("@id", id);

                using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    string img = r["c_image"]?.ToString();
                    img = string.IsNullOrEmpty(img) ? "/Image/default.png" : "/Image/" + img;

                    user = new t_taxuser
                    {
                        c_userid = r.GetInt32(r.GetOrdinal("c_userid")),
                        c_username = r["c_username"].ToString(),
                        c_email = r["c_email"].ToString(),
                        c_password = r["c_password"].ToString(),
                        c_mobile = r["c_mobile"].ToString(),
                        c_city = r["c_city"].ToString(),
                        c_state = r["c_state"].ToString(),
                        c_gender = r["c_gender"].ToString(),
                        c_type = r["c_type"].ToString(),
                        c_image = img,
                        c_role = r["c_role"].ToString()
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