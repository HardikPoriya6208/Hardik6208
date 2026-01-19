CREATE TABLE t_flightuser(
	c_userid SERIAL PRIMARY KEY,
	c_username VARCHAR(100)not null,
	c_email VARCHAR(100)not null,
	c_password VARCHAR(100)not null,
	c_mobile VARCHAR(100)not null,
	c_image VARCHAR(500),
	c_role VARCHAR(20)not null DEFAULT 'User'
)
SELECT * from t_flightuser;

insert INTO t_flightuser(c_username,c_email,c_password,c_mobile,c_image,c_role)
	VALUES('Hardik','hardik123@gmail.com','Hardik@6208','9106230614','HP.png','User')

CREATE TABLE t_flight(
		t_flightId SERIAL PRIMARY KEY,
		t_flightno VARCHAR(100) not null,
		t_departure VARCHAR(100)not null,
		t_destination VARCHAR(100)not null,
		t_date date not null,
		t_totalSeats int not null,
		t_price int not null
)
select  * from t_flight;

INSERT INTO t_flight
(t_flightno, t_departure, t_destination, t_date, t_totalSeats, t_price)
VALUES
('A101', 'New York', 'London', '2026-01-15', 180, 55000),
('A102', 'London', 'Paris', '2026-01-16', 150, 22000),
('A103', 'Mumbai', 'Dubai', '2026-01-18', 200, 18000),
('A104', 'Delhi', 'Singapore', '2026-01-20', 170, 42000),
('A105', 'Sydney', 'Tokyo', '2026-01-22', 190, 48000);


CREATE TABLE t_flightbooking(
	t_bookingId SERIAL PRIMARY KEY,
	c_userid int not null,
	t_flightId int not null,
	t_departure VARCHAR(100) not null,
	t_totalSeats int not null,
	t_date date not null
)
