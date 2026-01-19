using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models;
using LIBRARY.Models;
using Npgsql;

namespace library.BAL
{
    public class libraryHelper
    {
        private readonly NpgsqlConnection _conn;

        public libraryHelper(NpgsqlConnection conn)
        {
            _conn = conn;
        }

        // ================== GET ALL TOURS ==================
        public async Task<List<t_book>> GetAllTours()
        {
            var list = new List<t_book>();
            await _conn.OpenAsync();

            string sql = @"
            SELECT c_bookingid, c_bookname, c_author, c_category,c_totalqty,c_availableqty,c_image
            FROM t_book
            ORDER BY c_bookingid ASC";

            using var cmd = new NpgsqlCommand(sql, _conn);
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                list.Add(MapTour(dr));
            }

            await _conn.CloseAsync();
            return list;
        }


        private t_book MapTour(NpgsqlDataReader dr)
        {
            return new t_book
            {
                c_bookingid = dr.GetInt32(dr.GetOrdinal("c_bookingid")),
                c_bookname = dr.GetString(dr.GetOrdinal("c_bookname")),
                c_author = dr.GetString(dr.GetOrdinal("c_author")),
                c_category = dr.GetString(dr.GetOrdinal("c_category")),
                c_totalqty = dr.GetInt32(dr.GetOrdinal("c_totalqty")),
                c_availableqty = dr.GetInt32(dr.GetOrdinal("c_availableqty")),
                c_image = dr.IsDBNull(dr.GetOrdinal("c_image"))
                    ? "default.png"
                    : dr.GetString(dr.GetOrdinal("c_image"))
            };
        }

        // ================= Save Data =======================
        public async Task<int> SaveBooking(t_BookIssue data)
        {
            await _conn.OpenAsync();

            string sql = @"
INSERT INTO t_BookIssue
(c_userid, c_bookingid, c_issueDate, c_dueDate, c_status,c_bookname)
VALUES
(@uid, @bid, @issue, @due, @status,@c_bookname)";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@uid", data.c_userid);
            cmd.Parameters.AddWithValue("@bid", data.c_bookingid);
            cmd.Parameters.AddWithValue("@issue", data.c_issueDate);
            cmd.Parameters.AddWithValue("@due", data.c_dueDate);
            cmd.Parameters.AddWithValue("@status", data.c_status);
            cmd.Parameters.AddWithValue("@c_bookname", data.c_bookname);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

        // ============ User =======================
        public async Task<List<t_BookIssue>> GetAllBookings()
        {
            var list = new List<t_BookIssue>();
            await _conn.OpenAsync();

            string sql = @"
SELECT bi.c_issueid, bi.c_userid, b.c_bookname,
       bi.c_issueDate, bi.c_dueDate, bi.c_status
FROM t_BookIssue bi
JOIN t_book b ON bi.c_bookingid = b.c_bookingid
ORDER BY bi.c_issueid";

            using var cmd = new NpgsqlCommand(sql, _conn);
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
                list.Add(MapBooking(dr));

            await _conn.CloseAsync();
            return list;
        }

        private t_BookIssue MapBooking(NpgsqlDataReader dr)
        {
            return new t_BookIssue
            {
                c_issueid = dr.GetInt32(dr.GetOrdinal("c_issueid")),
                c_userid = dr.GetInt32(dr.GetOrdinal("c_userid")),
                c_bookname = dr.GetString(dr.GetOrdinal("c_bookname")),
                c_bookingid = dr.GetInt32(dr.GetOrdinal("c_bookingid")),
                c_issueDate = dr.GetString(dr.GetOrdinal("c_issueDate")),
                c_dueDate = dr.GetString(dr.GetOrdinal("c_dueDate")),
                c_status = dr.GetString(dr.GetOrdinal("c_status"))
            };
        }

        // =============== Qty(-1) =======================
        public async Task<bool> DecreaseStock(int bookId)
        {
            await _conn.OpenAsync();

            string sql = @"
UPDATE t_book
SET c_availableqty = c_availableqty - 1
WHERE c_bookingid=@id AND c_availableqty > 0";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", bookId);

            int rows = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return rows > 0;
        }

        // =============== Qty(+1) =======================
        public async Task IncreaseStock(int bookId)
        {
            await _conn.OpenAsync();

            string sql = @"
UPDATE t_book
SET c_availableqty = c_availableqty + 1
WHERE c_bookingid=@id";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", bookId);

            await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
        }

        //   // =============== Alreday Book =======================
        public async Task<bool> IsBookAlreadyBooked(int userId, int bookId)
        {
            await _conn.OpenAsync();

            string sql = @"
SELECT COUNT(*) FROM t_BookIssue
WHERE c_userid=@uid AND c_bookingid=@bid";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@bid", bookId);

            int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            await _conn.CloseAsync();

            return count > 0;
        }


        // ================== GET BOOKINGS BY USER ==================
        public async Task<List<t_BookIssue>> GetBookingsByUser(int userId)
        {
            var list = new List<t_BookIssue>();
            await _conn.OpenAsync();

            string sql = @"SELECT c_issueid, c_userid, c_bookname, c_bookingid, c_issueDate, c_dueDate,c_status
                       FROM t_BookIssue WHERE c_userid=@uid ORDER BY c_bookingid ASC";

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

        public async Task<int?> GetBookIdByBooking(int issueId)
        {
            await _conn.OpenAsync();

            string sql = "SELECT c_bookingid FROM t_BookIssue WHERE c_issueid=@id";
            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", issueId);

            object result = await cmd.ExecuteScalarAsync();
            await _conn.CloseAsync();

            return result == null ? null : Convert.ToInt32(result);
        }


        // ================== Delete BOOKING ==================
        public async Task<int> DeleteBooking(int id)
        {
            await _conn.OpenAsync();

            string sql = "DELETE FROM t_BookIssue WHERE c_issueid=@id";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }


        //================== Admin Side ==================

        // ================== ADMIN: ADD TOUR ==================
        public async Task<int> AdminAddTour(t_book tour)
        {
            await _conn.OpenAsync();

            string sql = @"
        INSERT INTO t_book (c_bookname, c_author, c_totalqty,c_category,c_availableqty,c_image)
        VALUES (@c_bookname, @c_author, @c_totalqty,@c_category,@c_availableqty,@c_image)";


            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@c_bookname", tour.c_bookname);
            cmd.Parameters.AddWithValue("@c_author", tour.c_author);
            cmd.Parameters.AddWithValue("@c_totalqty", tour.c_totalqty);
            cmd.Parameters.AddWithValue("@c_category", tour.c_category);
            cmd.Parameters.AddWithValue("@c_availableqty", tour.c_availableqty);
            cmd.Parameters.AddWithValue("@c_image", tour.c_image ?? "default.png");

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
    bi.c_issueid,
    bi.c_userid,
    u.c_username,
    bi.c_bookname,
    bi.c_bookingid,
    bi.c_issueDate,
    bi.c_dueDate,
    bi.c_status
FROM t_BookIssue bi
JOIN t_libraryuser u 
    ON bi.c_userid = u.c_userid
JOIN t_book b
    ON bi.c_bookingid = b.c_bookingid;

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
        public async Task<t_BookIssue?> AdminGetBookingById(int id)
        {
            await _conn.OpenAsync();

            string sql = @"SELECT c_issueid, c_userid, c_bookname, c_bookingid, c_issueDate,c_dueDate,c_status
                       FROM t_BookIssue WHERE c_bookingid=@id";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var dr = await cmd.ExecuteReaderAsync();
            t_BookIssue? booking = null;
            if (await dr.ReadAsync())
            {
                booking = MapBooking(dr);
            }

            await _conn.CloseAsync();
            return booking;
        }

        // =========== Admin: Update Tour =================
        public async Task<int> AdminUpdateTour(t_book tour)
        {
            await _conn.OpenAsync();

            string sql = @"UPDATE t_book 
                   SET c_bookname=@c_bookname,
                       c_author=@c_author,
                       c_category=@c_category,
                       c_totalqty=@c_totalqty,
                       c_availableqty=@c_availableqty,
                       c_image=@c_image,
                   WHERE c_bookingid=@id";

            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@c_bookname", tour.c_bookname);
            cmd.Parameters.AddWithValue("@c_author", tour.c_author);
            cmd.Parameters.AddWithValue("@c_category", tour.c_category);
            cmd.Parameters.AddWithValue("@c_totalqty", tour.c_totalqty);
            cmd.Parameters.AddWithValue("@c_availableqty", tour.c_availableqty);
            cmd.Parameters.AddWithValue("@c_image", tour.c_image);

            int row = await cmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return row;
        }

        // =========== Admin: Delet Tour =================
        public async Task<int> AdminDeleteTour(int id)
        {
            await _conn.OpenAsync();

            string sql = @"DELETE FROM t_book WHERE c_bookingid=@id";
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
                c_issueid = dr.GetInt32(dr.GetOrdinal("c_issueid")),
                c_userid = dr.GetInt32(dr.GetOrdinal("c_userid")),
                c_bookname = dr.GetString(dr.GetOrdinal("c_bookname")),
                c_issueDate = dr.GetString(dr.GetOrdinal("c_issueDate")),
                c_dueDate = dr.GetString(dr.GetOrdinal("c_dueDate")),
                c_status = dr.GetString(dr.GetOrdinal("c_status")),
            };
        }
    }
}