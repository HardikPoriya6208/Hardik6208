CREATE TABLE t_eventuser(
	c_userid SERIAL PRIMARY KEY,
	c_username VARCHAR(100)not null,
	c_email VARCHAR(100)not null,
	c_password VARCHAR(100)not null,
	c_mobile VARCHAR(100)not null,
	c_address VARCHAR(100)not null,
	c_image VARCHAR(500),
	c_role VARCHAR(20)not null DEFAULT 'User'
)

select * from t_eventuser;

INSERT INTO t_eventuser(c_username,c_email,c_password,c_mobile,c_address,c_image,c_role)
	VALUES('admin','admin123@gmail.com','Hardik@6208','9106230614','Surat','HP.png','Admin')
	
INSERT INTO t_eventuser(c_username,c_email,c_password,c_mobile,c_address,c_image,c_role)
	VALUES('Hardik','hardik123@gmail.com','Hardik@6208','9106230614','Surat','HP.png','User')


CREATE TABLE t_event(
	c_eventId SERIAL PRIMARY KEY,
	c_eventname VARCHAR(100)not null,
	c_eventdate VARCHAR(100)not null,
	c_eventprice int not null,
	c_eventseats int not null,
	c_address VARCHAR(100)not null
)

select * from t_event;

INSERT INTO t_event 
(c_eventname, c_eventdate, c_eventprice, c_eventseats, c_address)
VALUES
('Music Concert', '2026-02-10', 1500, 200, 'Ahmedabad'),
('Tech Conference', '2026-03-05', 2500, 150, 'Gandhinagar'),
('Food Festival', '2026-01-25', 500, 300, 'Surat'),
('Startup Meetup', '2026-02-18', 800, 100, 'Vadodara'),
('Art Exhibition', '2026-03-01', 600, 120, 'Rajkot');

CREATE TABLE t_eventbooking(
	c_bookingid SERIAL PRIMARY KEY,
	c_userid int not null,
	c_eventid int not null,
	c_eventseats int not null,
	c_eventprice int not null,
	c_status VARCHAR(100)not null,
	c_eventdate VARCHAR(100)not null
)