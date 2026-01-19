using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVC.Models;
using Npgsql;

namespace MVC.BAL
{
    public class UserHelper
    {
        private readonly NpgsqlConnection _conn;

        public UserHelper(NpgsqlConnection connection)
        {
            _conn = connection;
        }


        // ---------------- Register ----------------
        public async Task<int> Register(t_registeruser data)
        {
            try
            {
                await _conn.OpenAsync();

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM t_registeruser WHERE c_email=@e", _conn))
                {
                    cmd.Parameters.AddWithValue("@e", data.c_email);
                    long count = (long)await cmd.ExecuteScalarAsync();
                    if (count > 0)
                        return -1;
                }

                using var insert = new NpgsqlCommand(@"
                    INSERT INTO t_registeruser
                    (c_username,c_email,c_password,c_cpassword,c_mobile,c_state,c_city,c_gender,c_image,c_date)
                    VALUES (@c_username,@c_email,@c_password,@c_cpassword,@c_mobile,@c_state,@c_city,@c_gender,@c_image,@c_date)
                ", _conn);

                insert.Parameters.AddWithValue("@c_username", data.c_username);
                insert.Parameters.AddWithValue("@c_email", data.c_email);
                insert.Parameters.AddWithValue("@c_password", data.c_password);
                insert.Parameters.AddWithValue("@c_cpassword", data.c_cpassword);
                insert.Parameters.AddWithValue("@c_mobile", data.c_mobile);
                insert.Parameters.AddWithValue("@c_state", data.c_state);
                insert.Parameters.AddWithValue("@c_city", data.c_city);
                insert.Parameters.AddWithValue("@c_gender", data.c_gender);
                insert.Parameters.AddWithValue("@c_date", data.c_date);
                insert.Parameters.AddWithValue("@c_image", data.c_image ?? "default.png");

                await insert.ExecuteNonQueryAsync();
                return 1;
            }
            finally { await _conn.CloseAsync(); }
        }

        // ---------------- LOGIN ----------------
        public async Task<t_registeruser?> Login(string email, string pwd)
        {
            t_registeruser? user = null;
            try
            {
                await _conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM t_registeruser WHERE c_email=@e AND c_password=@p", _conn);

                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@p", pwd);

                using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    string img = r["c_image"].ToString();
                    if (!img.StartsWith("/"))
                        img = "/Image/" + img;

                    user = new t_registeruser
                    {
                        c_userid = r.GetInt32(r.GetOrdinal("c_userid")),
                        c_username = r["c_username"].ToString(),
                        c_email = r["c_email"].ToString(),
                        c_password = r["c_password"].ToString(),
                        c_cpassword = r["c_cpassword"].ToString(),
                        c_mobile = r["c_mobile"].ToString(),
                        c_city = r["c_city"].ToString(),
                        c_state = r["c_state"].ToString(),
                        c_gender = r["c_gender"].ToString(),
                        c_date = r["c_date"].ToString(),
                        c_image = img,
                        c_role = r["c_role"].ToString()
                    };
                }
            }
            finally { await _conn.CloseAsync(); }

            return user;
        }

        // ---------- GET USER ----------
        public async Task<t_registeruser?> GetUserById(int id)
        {
            t_registeruser? user = null;

            try
            {
                await _conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM t_registeruser WHERE c_userid=@id", _conn);

                cmd.Parameters.AddWithValue("@id", id);

                using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    string img = r["c_image"]?.ToString();
                    img = string.IsNullOrEmpty(img) ? "/Image/default.png" : "/Image/" + img;

                    user = new t_registeruser
                    {
                        c_userid = r.GetInt32(r.GetOrdinal("c_userid")),
                        c_username = r["c_username"].ToString(),
                        c_email = r["c_email"].ToString(),
                        c_password = r["c_password"].ToString(),
                        c_cpassword = r["c_cpassword"].ToString(),
                        c_mobile = r["c_mobile"].ToString(),
                        c_city = r["c_city"].ToString(),
                        c_state = r["c_state"].ToString(),
                        c_gender = r["c_gender"].ToString(),
                        c_date = r["c_date"].ToString(),
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

        // ---------- UPDATE PROFILE ----------
        public async Task<bool> UpdateProfile(t_registeruser user)
        {
            try
            {
                await _conn.OpenAsync();

                using var cmd = new NpgsqlCommand(@"
                UPDATE t_registeruser SET
                    c_username=@name,
                    c_city=@city,
                    c_state=@state,
                    c_mobile=@mobile,
                    c_gender=@gender
                WHERE c_userid=@id
            ", _conn);

                cmd.Parameters.AddWithValue("@name", user.c_username);
                cmd.Parameters.AddWithValue("@city", user.c_city);
                cmd.Parameters.AddWithValue("@state", user.c_state);
                cmd.Parameters.AddWithValue("@mobile", user.c_mobile);
                cmd.Parameters.AddWithValue("@gender", user.c_gender);
                cmd.Parameters.AddWithValue("@id", user.c_userid);

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        // ---------- UPDATE IMAGE ----------
        public async Task<bool> UpdateProfilePicture(int id, string img)
        {
            try
            {
                await _conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "UPDATE t_registeruser SET c_image=@i WHERE c_userid=@id", _conn);

                cmd.Parameters.AddWithValue("@i", img);
                cmd.Parameters.AddWithValue("@id", id);

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        // ---------- UPDATE PASSWORD ----------
        public async Task<int> UpdatePassword(int id, string oldPass, string newPass)
        {
            try
            {
                await _conn.OpenAsync();

                string? dbPass;

                using (var cmd = new NpgsqlCommand(
                    "SELECT c_password FROM t_registeruser WHERE c_userid=@id", _conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    dbPass = (await cmd.ExecuteScalarAsync())?.ToString();
                }

                if (dbPass != oldPass)
                    return 0;

                using (var cmd = new NpgsqlCommand(
                    "UPDATE t_registeruser SET c_password=@p WHERE c_userid=@id", _conn))
                {
                    cmd.Parameters.AddWithValue("@p", newPass);
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }

                return 1;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

    }
}