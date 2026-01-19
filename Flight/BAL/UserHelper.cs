using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flight.Models;
using Npgsql;

namespace Flight.BAL
{
    public class UserHelper
    {
        private readonly NpgsqlConnection _conn;


        public UserHelper(NpgsqlConnection connection)
        {
            _conn = connection;
        }


        // ============= Register ===========
        public async Task<int> Register(t_flightuser data)
        {
            try
            {
                await _conn.OpenAsync();

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM t_flightuser WHERE c_email=@e", _conn))
                {
                    cmd.Parameters.AddWithValue("@e", data.c_email);
                    long count = (long)await cmd.ExecuteScalarAsync();
                    if (count > 0)
                        return -1;
                }

                using var insert = new NpgsqlCommand(@"
                    INSERT INTO t_flightuser
                    (c_username,c_email,c_password,c_mobile,c_image)
                    VALUES (@c_username,@c_email,@c_password,@c_mobile,@c_image)
                ", _conn);

                insert.Parameters.AddWithValue("@c_username", data.c_username);
                insert.Parameters.AddWithValue("@c_email", data.c_email);
                insert.Parameters.AddWithValue("@c_password", data.c_password);
                insert.Parameters.AddWithValue("@c_mobile", data.c_mobile);
                insert.Parameters.AddWithValue("@c_image", data.c_image ?? "default.png");

                await insert.ExecuteNonQueryAsync();
                return 1;
            }
            finally { await _conn.CloseAsync(); }
        }

        // ---------------- LOGIN ----------------
        public async Task<t_flightuser?> Login(string email, string pwd)
        {
            t_flightuser? user = null;
            try
            {
                await _conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM t_flightuser WHERE c_email=@e AND c_password=@p", _conn);

                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@p", pwd);

                using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    string img = r["c_image"].ToString();
                    if (!img.StartsWith("/"))
                        img = "/Image/" + img;

                    user = new t_flightuser
                    {
                        c_userid = r.GetInt32(r.GetOrdinal("c_userid")),
                        c_username = r["c_username"].ToString(),
                        c_email = r["c_email"].ToString(),
                        c_password = r["c_password"].ToString(),
                        c_mobile = r["c_mobile"].ToString(),
                        c_image = img,
                        c_role = r["c_role"].ToString()
                    };
                }
            }
            finally { await _conn.CloseAsync(); }

            return user;
        }

    }
}