using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flight.Models;
using Npgsql;

namespace Flight.BAL
{
    public class FlightHelper
    {
        private readonly NpgsqlConnection _conn;

        public FlightHelper(NpgsqlConnection conn)
        {
            _conn = conn;
        }

        // ================== GET ALL TOURS ==================
        public async Task<List<t_flight>> GetAllTours()
        {
            var list = new List<t_flight>();
            await _conn.OpenAsync();

            string sql = @"
            SELECT t_flightId, t_flightno, t_departure, t_destination, t_date, t_totalSeats, t_price
            FROM t_flight
            ORDER BY t_flightId ASC";

            using var cmd = new NpgsqlCommand(sql, _conn);
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                list.Add(MapTour(dr));
            }

            await _conn.CloseAsync();
            return list;
        }


        private t_flight MapTour(NpgsqlDataReader dr)
        {
            return new t_flight
            {
                t_flightId = dr.GetInt32(dr.GetOrdinal("t_flightId")),
                t_flightno = dr.GetString(dr.GetOrdinal("t_flightno")),
                t_departure = dr.GetString(dr.GetOrdinal("t_departure")),
                t_destination = dr.GetString(dr.GetOrdinal("t_destination")),
                t_totalSeats = dr.GetInt32(dr.GetOrdinal("t_totalSeats")),
                t_price = dr.GetInt32(dr.GetOrdinal("t_price")),
                t_date = DateOnly.FromDateTime(dr.GetDateTime(dr.GetOrdinal("t_date")))
            };
        }



        // ================== SAVE BOOKING ==================
        public async Task<int> SaveBooking(t_flightbooking data)
        {
            await _conn.OpenAsync();

            string sql = @"
INSERT INTO t_flightbooking 
(c_userid, t_flightId, t_totalSeats, t_date,t_departure)
VALUES 
(@c_userid, @t_flightId, @t_totalSeats, @t_date,@t_departure)";


            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@c_userid", data.c_userid);
            cmd.Parameters.AddWithValue("@t_flightId", data.t_flightId);
            cmd.Parameters.AddWithValue("@t_departure", data.t_departure);
            cmd.Parameters.AddWithValue("@t_totalSeats", data.t_totalSeats);
            cmd.Parameters.AddWithValue("@t_date", data.t_date);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

        // ================== GET ALL BOOKINGS ==================
        public async Task<List<t_flightbooking>> GetAllBookings()
        {
            var list = new List<t_flightbooking>();
            await _conn.OpenAsync();

            string sql = @"SELECT t_bookingId, c_userid, t_flightId, t_totalSeats,t_date,t_departure
                       FROM t_flightbooking ORDER BY t_bookingId ASC";

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
        public async Task<List<t_flightbooking>> GetBookingsByUser(int userId)
        {
            var list = new List<t_flightbooking>();
            await _conn.OpenAsync();

            string sql = @"SELECT t_bookingId, c_userid, t_flightId, t_totalSeats,t_date,t_departure
                       FROM t_flightbooking WHERE c_userid=@uid ORDER BY t_bookingId ASC";

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

        // ================== Delete BOOKING ==================
        public async Task<int> DeleteBooking(int id)
        {
            await _conn.OpenAsync();

            string sql = @"DELETE FROM t_flightbooking WHERE t_bookingId=@id";
            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }



        private t_flightbooking MapBooking(NpgsqlDataReader dr)
        {
            return new t_flightbooking
            {
                t_bookingId = dr.GetInt32(dr.GetOrdinal("t_bookingId")),
                t_flightId = dr.GetInt32(dr.GetOrdinal("t_flightId")),
                t_departure = dr.GetString(dr.GetOrdinal("t_departure")),
                t_totalSeats = dr.GetInt32(dr.GetOrdinal("t_totalSeats")),
                t_date = DateOnly.FromDateTime(dr.GetDateTime(dr.GetOrdinal("t_date"))),
                c_userid = dr.GetInt32(dr.GetOrdinal("c_userid"))
            };
        }

        public async Task<int> UpdateBooking(t_flightbooking data)
        {
            await _conn.OpenAsync();

            string sql = @"UPDATE t_flightbooking SET t_date=@t_date,t_totalSeats=@t_totalSeats, t_departure=@t_departure WHERE t_bookingId=@t_bookingId";
            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@t_date", data.t_date);
            cmd.Parameters.AddWithValue("@t_totalSeats", data.t_totalSeats);
            cmd.Parameters.AddWithValue("@t_departure", data.t_departure);
            cmd.Parameters.AddWithValue("@t_bookingId", data.t_bookingId);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

    }
}