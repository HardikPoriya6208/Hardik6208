using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Event.Models;
using Npgsql;

namespace Event.BAL
{
    public class EventHelper
    {
        private readonly NpgsqlConnection _conn;

        public EventHelper(NpgsqlConnection conn)
        {
            _conn = conn;
        }

        // ================== GET ALL TOURS ==================
        public async Task<List<t_event>> GetAllTours()
        {
            var list = new List<t_event>();
            await _conn.OpenAsync();

            string sql = @"
            SELECT c_eventId, c_eventname, c_eventdate, c_eventprice,c_eventseats,c_address
            FROM t_event
            ORDER BY c_eventId ASC";

            using var cmd = new NpgsqlCommand(sql, _conn);
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                list.Add(MapTour(dr));
            }

            await _conn.CloseAsync();
            return list;
        }

        private t_event MapTour(NpgsqlDataReader dr)
        {
            return new t_event
            {
                c_eventId = dr.GetInt32(dr.GetOrdinal("c_eventId")),
                c_eventname = dr.GetString(dr.GetOrdinal("c_eventname")),
                c_eventdate = dr.GetString(dr.GetOrdinal("c_eventdate")),
                c_eventprice = dr.GetInt32(dr.GetOrdinal("c_eventprice")),
                c_eventseats = dr.GetInt32(dr.GetOrdinal("c_eventseats")),
                c_address = dr.GetString(dr.GetOrdinal("c_address")),
            };
        }

        // ================== SAVE BOOKING ==================
        public async Task<int> SaveBooking(t_eventbooking data)
        {
            await _conn.OpenAsync();

            string sql = @"
            INSERT INTO t_eventbooking (c_eventid, c_userid, c_eventseats,c_eventprice,c_status,c_eventdate)
            VALUES (@c_eventid, @c_userid, @c_eventseats,@c_eventprice,@c_status,@c_eventdate)";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@c_userid", data.c_userid);
            cmd.Parameters.AddWithValue("@c_eventid", data.c_eventid);
            cmd.Parameters.AddWithValue("@c_eventseats", data.c_eventseats);
            cmd.Parameters.AddWithValue("@c_eventprice", data.c_eventprice);
            cmd.Parameters.AddWithValue("@c_status", data.c_status);
            cmd.Parameters.AddWithValue("@c_eventdate", data.c_eventdate);


            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

        // ================== GET ALL BOOKINGS ==================
        public async Task<List<t_eventbooking>> GetAllBookings()
        {
            var list = new List<t_eventbooking>();
            await _conn.OpenAsync();

            string sql = @"SELECT c_bookingid, c_userid, c_eventid, c_eventseats ,c_eventprice,c_status,c_eventdate
                       FROM t_eventbooking ORDER BY c_bookingid ASC";

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
        public async Task<List<t_eventbooking>> GetBookingsByUser(int userId)
        {
            var list = new List<t_eventbooking>();
            await _conn.OpenAsync();

            string sql = @"SELECT c_bookingid, c_userid, c_eventid, c_eventseats, c_eventprice,c_status,c_eventdate
                       FROM t_eventbooking WHERE c_userid=@uid ORDER BY c_bookingid ASC";

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
        public async Task<int> UpdateBooking(t_eventbooking data)
        {
            await _conn.OpenAsync();

            string sql = @"UPDATE t_eventbooking SET c_eventdate=@c_eventdate,c_eventseats=@c_eventseats WHERE c_bookingid=@c_bookingid";
            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@c_eventseats", data.c_eventseats);
            cmd.Parameters.AddWithValue("@c_eventdate", data.c_eventdate);
            cmd.Parameters.AddWithValue("@c_bookingid", data.c_bookingid);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

        // ================== Delete BOOKING ==================
        public async Task<int> DeleteBooking(int id)
        {
            await _conn.OpenAsync();

            string sql = @"DELETE FROM t_eventbooking WHERE c_bookingid=@id";
            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

        private t_eventbooking MapBooking(NpgsqlDataReader dr)
        {
            return new t_eventbooking
            {
                c_bookingid = dr.GetInt32(dr.GetOrdinal("c_bookingid")),
                c_userid = dr.GetInt32(dr.GetOrdinal("c_userid")),
                c_eventprice = dr.GetInt32(dr.GetOrdinal("c_eventprice")),
                c_eventseats = dr.GetInt32(dr.GetOrdinal("c_eventseats")),
                c_eventdate = dr.GetString(dr.GetOrdinal("c_eventdate")),
                c_status = dr.GetString(dr.GetOrdinal("c_status")),
                c_eventid = dr.GetInt32(dr.GetOrdinal("c_eventid")),
            };
        }



        //======================== Admin Side =======================

        // ================== ADMIN: ADD TOUR ==================
        public async Task<int> AdminAddTour(t_event tour)
        {
            await _conn.OpenAsync();

            string sql = @"
        INSERT INTO t_event (c_eventname, c_eventprice, c_eventdate,c_eventseats,c_address)
        VALUES (@c_eventname, @c_eventprice, @c_eventdate,@c_eventseats,@c_address)";


            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@c_eventname", tour.c_eventname);
            cmd.Parameters.AddWithValue("@c_eventdate", tour.c_eventdate);
            cmd.Parameters.AddWithValue("@c_eventprice", tour.c_eventprice);
            cmd.Parameters.AddWithValue("@c_eventseats", tour.c_eventseats);
            cmd.Parameters.AddWithValue("@c_address", tour.c_address);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();

            return row;
        }

        public async Task<List<AdminBookingView>> AdminGetAllBookings()
        {
            var list = new List<AdminBookingView>();
            await _conn.OpenAsync();

            string sql = @"
        SELECT 
            b.c_bookingid,
            b.c_userid,
            u.c_username,
            b.c_eventseats,
            b.c_eventprice,
            b.c_status,
            b.c_eventid,
            b.c_eventdate
        FROM t_eventbooking b
        JOIN t_eventuser u ON b.c_userid = u.c_userid;
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
        public async Task<t_eventbooking?> AdminGetBookingById(int id)
        {
            await _conn.OpenAsync();

            string sql = @"SELECT c_bookingid, c_eventid, c_eventseats, c_eventprice, c_userid,c_status,c_eventdate
                       FROM t_eventbooking WHERE c_bookingid=@id";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var dr = await cmd.ExecuteReaderAsync();
            t_eventbooking? booking = null;
            if (await dr.ReadAsync())
            {
                booking = MapBooking(dr);
            }

            await _conn.CloseAsync();
            return booking;
        }

        // =========== Admin: Update Tour =================
        public async Task<int> AdminUpdateTour(t_event tour)
        {
            await _conn.OpenAsync();

            string sql = @"UPDATE t_event 
                   SET c_eventname=@c_eventname, 
                    c_eventdate=@c_eventdate, 
                       c_eventprice=@c_eventprice,
                       c_eventseats=@c_eventseats,
                       c_address=@c_address
                   WHERE c_eventId=@id";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@c_eventname", tour.c_eventname);
            cmd.Parameters.AddWithValue("@c_eventdate", tour.c_eventdate);
            cmd.Parameters.AddWithValue("@c_eventprice", tour.c_eventprice);
            cmd.Parameters.AddWithValue("@c_eventseats", tour.c_eventseats);
            cmd.Parameters.AddWithValue("@c_address", tour.c_address);
            cmd.Parameters.AddWithValue("@id", tour.c_eventId);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

        // =========== Admin: Delet Tour =================
        public async Task<int> AdminDeleteTour(int id)
        {
            await _conn.OpenAsync();

            string sql = @"DELETE FROM t_event WHERE c_eventId=@id";
            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }


        private AdminBookingView MapAdminBooking(NpgsqlDataReader dr)
        {
            return new AdminBookingView
            {
                c_bookingid = dr.GetInt32(dr.GetOrdinal("c_bookingid")),
                c_userid = dr.GetInt32(dr.GetOrdinal("c_userid")),
                c_eventid = dr.GetInt32(dr.GetOrdinal("c_eventid")),
                c_eventseats = dr.GetInt32(dr.GetOrdinal("c_eventseats")),
                c_eventprice = dr.GetInt32(dr.GetOrdinal("c_eventprice")),
                c_status = dr.GetString(dr.GetOrdinal("c_status")),
                c_eventdate = dr.GetString(dr.GetOrdinal("c_eventdate")),
            };
        }


        //====================  Logic ========================
        // public TourReport GetTourReport(int tourid)
        // {
        //     var report = new TourReport();
        //     _conn.Open();

        //     string sql = @"
        //     SELECT 
        //         t.t_tourid,
        //         t.t_tourname,
        //         t.t_tourprice::int,
        //         COALESCE(e.totalexpense,0),
        //         COALESCE(b.totalbooking,0)
        //     FROM t_tour t
        //     LEFT JOIN (
        //         SELECT t_tourid, SUM(t_tourprice) totalexpense
        //         FROM t_tourexpense GROUP BY t_tourid
        //     ) e ON e.t_tourid=t.t_tourid
        //     LEFT JOIN (
        //         SELECT t_tourid, COUNT(*) totalbooking
        //         FROM t_tourbooking GROUP BY t_tourid
        //     ) b ON b.t_tourid=t.t_tourid
        //     WHERE t.t_tourid=@id";

        //     using var cmd = new NpgsqlCommand(sql, _conn);
        //     cmd.Parameters.AddWithValue("@id", tourid);

        //     using var dr = cmd.ExecuteReader();
        //     if (dr.Read())
        //     {
        //         report.t_tourid = dr.GetInt32(0);
        //         report.t_expensename = dr.GetString(1);
        //         report.t_tourprice = dr.GetInt32(2);
        //         report.totalExpense = dr.GetInt32(3);
        //         report.totalBooking = dr.GetInt32(4);
        //         report.totalIncome = report.t_tourprice * report.totalBooking;
        //         report.profit = report.totalIncome - report.totalExpense;
        //     }

        //     _conn.Close();
        //     return report;
        // }


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