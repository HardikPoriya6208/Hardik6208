using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVC.Models;
using Npgsql;

namespace MVC.BAL
{
    public class TourHelper
    {
        private readonly NpgsqlConnection _conn;

        public TourHelper(NpgsqlConnection conn)
        {
            _conn = conn;
        }

        // ================== GET ALL TOURS ==================
        public async Task<List<t_tour>> GetAllTours()
        {
            var list = new List<t_tour>();
            await _conn.OpenAsync();

            string sql = @"
            SELECT t_tourid, t_tourname, t_tourprice, t_tourdate
            FROM t_tour
            ORDER BY t_tourid ASC";

            using var cmd = new NpgsqlCommand(sql, _conn);
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                list.Add(MapTour(dr));
            }

            await _conn.CloseAsync();
            return list;
        }


        private t_tour MapTour(NpgsqlDataReader dr)
        {
            return new t_tour
            {
                t_tourid = dr.GetInt32(dr.GetOrdinal("t_tourid")),
                t_tourname = dr.GetString(dr.GetOrdinal("t_tourname")),
                t_tourprice = dr.GetInt32(dr.GetOrdinal("t_tourprice")),
                // t_tourstourdate = DateOnly.FromDateTime(dr.GetDateTime(dr.GetOrdinal("t_tourstourdate")))
                t_tourdate = dr.GetString(dr.GetOrdinal("t_tourdate"))
            };
        }

        // ================== SAVE BOOKING ==================
        public async Task<int> SaveBooking(t_tourbooking data)
        {
            await _conn.OpenAsync();

            string sql = @"
            INSERT INTO t_tourbooking (t_tourname, c_userid, t_tourid,t_tourdate)
            VALUES (@t_tourname, @c_userid, @t_tourid,@t_tourdate)";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@t_tourname", data.t_tourname);
            cmd.Parameters.AddWithValue("@c_userid", data.c_userid);
            cmd.Parameters.AddWithValue("@t_tourid", data.t_tourid);
            cmd.Parameters.AddWithValue("@t_tourdate", data.t_tourdate);


            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

        // ================== GET ALL BOOKINGS ==================
        public async Task<List<t_tourbooking>> GetAllBookings()
        {
            var list = new List<t_tourbooking>();
            await _conn.OpenAsync();

            string sql = @"SELECT t_bookingid, t_tourname, c_userid, t_tourid ,t_tourdate
                       FROM t_tourbooking ORDER BY t_bookingid ASC";

            using var cmd = new NpgsqlCommand(sql, _conn);
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                list.Add(MapBooking(dr));
            }

            await _conn.CloseAsync();
            return list;
        }

        // ================== GET BOOKINGS BY USER ==================
        public async Task<List<t_tourbooking>> GetBookingsByUser(int userId)
        {
            var list = new List<t_tourbooking>();
            await _conn.OpenAsync();

            string sql = @"SELECT t_bookingid, t_tourname, c_userid, t_tourid, t_tourdate
                       FROM t_tourbooking WHERE c_userid=@uid ORDER BY t_bookingid ASC";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@uid", userId);

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                list.Add(MapBooking(dr));
            }

            await _conn.CloseAsync();
            return list;
        }

        // ================== UPDATE BOOKING ==================
        public async Task<int> UpdateBooking(t_tourbooking data)
        {
            await _conn.OpenAsync();

            // string sql = @"UPDATE t_tourbooking SET t_tourdate=@t_tourdate,t_toursprice=@t_toursprice WHERE t_bookingId=@t_bookingId";
            string sql = @"UPDATE t_tourbooking SET t_tourdate=@t_tourdate WHERE t_bookingid=@t_bookingid";
            using var cmd = new NpgsqlCommand(sql, _conn);
            // cmd.Parameters.AddWithValue("@t_toursprice", data.t_toursprice);
            cmd.Parameters.AddWithValue("@t_tourdate", data.t_tourdate);
            cmd.Parameters.AddWithValue("@t_bookingid", data.t_bookingid);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

        // ================== Delete BOOKING ==================
        public async Task<int> DeleteBooking(int id)
        {
            await _conn.OpenAsync();

            string sql = @"DELETE FROM t_tourbooking WHERE t_bookingid=@id";
            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

        private t_tourbooking MapBooking(NpgsqlDataReader dr)
        {
            return new t_tourbooking
            {
                t_bookingid = dr.GetInt32(dr.GetOrdinal("t_bookingid")),
                t_tourname = dr.GetString(dr.GetOrdinal("t_tourname")),
                c_userid = dr.GetInt32(dr.GetOrdinal("c_userid")),
                // t_tourstourdate = DateOnly.FromDateTime(dr.GetDateTime(dr.GetOrdinal("t_tourstourdate"))),
                t_tourdate = dr.GetString(dr.GetOrdinal("t_tourdate")),
                t_tourid = dr.GetInt32(dr.GetOrdinal("t_tourid")),
            };
        }




        //=================== Admin Side =======================


        // ================== ADMIN: ADD TOUR ==================
        public async Task<int> AdminAddTour(t_tour tour)
        {
            await _conn.OpenAsync();

            string sql = @"
        INSERT INTO t_tour (t_tourname, t_tourprice, t_tourdate)
        VALUES (@t_tourname, @t_tourprice, @t_tourdate)";


            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@t_tourname", tour.t_tourname);
            cmd.Parameters.AddWithValue("@t_tourprice", tour.t_tourprice);
            cmd.Parameters.AddWithValue("@t_tourdate", tour.t_tourdate);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();

            return row;
        }

        public async Task<List<AdminBookinView>> AdminGetAllBookings()
        {
            var list = new List<AdminBookinView>();
            await _conn.OpenAsync();

            string sql = @"
        SELECT 
            b.t_bookingid,
            b.c_userid,
            u.c_username,
            b.t_tourname,
            b.t_tourid,
            b.t_tourdate
        FROM t_tourbooking b
        JOIN t_registeruser u ON b.c_userid = u.c_userid;
    ";

            using var cmd = new NpgsqlCommand(sql, _conn);
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                list.Add(MapAdminBooking(dr));
            }

            await _conn.CloseAsync();
            return list;
        }


        // ================== ADMIN: GET BOOKING BY ID ==================
        public async Task<t_tourbooking?> AdminGetBookingById(int id)
        {
            await _conn.OpenAsync();

            string sql = @"SELECT t_bookingid, t_tourid, t_tourname, t_tourdate, c_userid
                       FROM t_tourbooking WHERE t_bookingid=@id";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var dr = await cmd.ExecuteReaderAsync();
            t_tourbooking? booking = null;
            if (await dr.ReadAsync())
            {
                booking = MapBooking(dr);
            }

            await _conn.CloseAsync();
            return booking;
        }

        // =========== Admin: Update Tour =================
         public async Task<int> AdminUpdateTour(t_tour tour)
        {
            await _conn.OpenAsync();

            string sql = @"UPDATE t_tour 
                   SET t_tourname=@name,
                       t_tourprice=@price,
                       t_tourdate=@date
                   WHERE t_tourid=@id";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@name", tour.t_tourname);
            cmd.Parameters.AddWithValue("@price", tour.t_tourprice);
            cmd.Parameters.AddWithValue("@date", tour.t_tourdate);
            cmd.Parameters.AddWithValue("@id", tour.t_tourid);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

         // =========== Admin: Delet Tour =================
        public async Task<int> AdminDeleteTour(int id)
        {
            await _conn.OpenAsync();

            string sql = @"DELETE FROM t_tour WHERE t_tourid=@id";
            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }


        private AdminBookinView MapAdminBooking(NpgsqlDataReader dr)
        {
            return new AdminBookinView
            {
                t_bookingid = dr.GetInt32(dr.GetOrdinal("t_bookingid")),
                c_userid = dr.GetInt32(dr.GetOrdinal("c_userid")),
                t_tourname = dr.GetString(dr.GetOrdinal("t_tourname")),
                t_tourid = dr.GetInt32(dr.GetOrdinal("t_tourid")),
                t_tourdate = dr.GetString(dr.GetOrdinal("t_tourdate")),
            };
        }


        //====================  Logic ========================
        public TourReport GetTourReport(int tourid)
        {
            var report = new TourReport();
            _conn.Open();

            string sql = @"
            SELECT 
                t.t_tourid,
                t.t_tourname,
                t.t_tourprice::int,
                COALESCE(e.totalexpense,0),
                COALESCE(b.totalbooking,0)
            FROM t_tour t
            LEFT JOIN (
                SELECT t_tourid, SUM(t_tourprice) totalexpense
                FROM t_tourexpense GROUP BY t_tourid
            ) e ON e.t_tourid=t.t_tourid
            LEFT JOIN (
                SELECT t_tourid, COUNT(*) totalbooking
                FROM t_tourbooking GROUP BY t_tourid
            ) b ON b.t_tourid=t.t_tourid
            WHERE t.t_tourid=@id";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", tourid);

            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                report.t_tourid = dr.GetInt32(0);
                report.t_expensename = dr.GetString(1);
                report.t_tourprice = dr.GetInt32(2);
                report.totalExpense = dr.GetInt32(3);
                report.totalBooking = dr.GetInt32(4);
                report.totalIncome = report.t_tourprice * report.totalBooking;
                report.profit = report.totalIncome - report.totalExpense;
            }

            _conn.Close();
            return report;
        }



        // ================== ADMIN: UPDATE BOOKING STATUS ==================

        // public async Task<int> AdminUpdateBookingStatus(int bookingId, string status)
        // {
        //     if (string.IsNullOrWhiteSpace(status))
        //         throw new ArgumentException("Status cannot be null");

        //     await _conn.OpenAsync();

        //     string sql = @"
        // UPDATE t_eventbooking
        // SET t_status = @t_status
        // WHERE t_bookingId = @t_bookingId";

        //     using var cmd = new NpgsqlCommand(sql, _conn);
        //     cmd.Parameters.Add("@t_bookingId", NpgsqlTypes.NpgsqlDbType.Integer).Value = bookingId;
        //     cmd.Parameters.Add("@t_status", NpgsqlTypes.NpgsqlDbType.Varchar).Value = status;

        //     int row = await cmd.ExecuteNonQueryAsync();
        //     await _conn.CloseAsync();

        //     return row;
        // }


       

    }
}