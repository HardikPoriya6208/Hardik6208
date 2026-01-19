CREATE TABLE t_registeruser(
	c_userid SERIAL PRIMARY KEY,
	c_username VARCHAR(100)not null,
	c_email VARCHAR(100)not null,
	c_password VARCHAR(100)not null,
	c_cpassword VARCHAR(100)not null,
	c_date VARCHAR(100)not null,
	c_state VARCHAR(100)not null,
	c_city VARCHAR(100)not null,
	c_gender VARCHAR(100)not null,
	c_mobile VARCHAR(100)not null,
	c_image VARCHAR(500),
	c_role varchar(20)not null DEFAULT 'User'
);

select * from t_registeruser;

INSERT INTO t_registeruser(c_username,c_email,c_password,c_cpassword,c_state,c_city,c_gender,c_mobile,c_image,c_role,c_date)
	VALUES('Admin','admin123@gmail.com','Hardik@6208','Hardik@6208','Gujrat','Surat','Male','9106230614','HP.png','Admin','2020-12-01');
INSERT INTO t_registeruser(c_username,c_email,c_password,c_cpassword,c_state,c_city,c_gender,c_mobile,c_image,c_role,c_date)
	VALUES('Hardik','hardik123@gmail.com','Hardik@6208','Hardik@6208','Gujrat','Surat','Male','9106230615','HP.png','User','2025-01-01');


CREATE TABLE t_tour(
	t_tourid SERIAL PRIMARY KEY,
	t_tourname VARCHAR(100)not null,
	t_tourprice int not null,
	t_tourdate VARCHAR(100)not null
);

select * from t_tour;

INSERT INTO t_tour (t_tourname, t_tourprice, t_tourdate) VALUES
('Goa Beach Tour', '15000', '2026-02-10'),
('Manali Hill Tour', '18000', '2026-03-05'),
('Rajasthan Heritage Tour', '22000', '2026-04-15'),
('Kerala Backwater Tour', '20000', '2026-05-20'),
('Kashmir Snow Tour', '25000', '2026-01-30');

CREATE TABLE t_tourbooking(
	t_bookingid SERIAL PRIMARY KEY,
	t_tourname VARCHAR(100)not null,
	c_userid int not null,
	t_tourid int not null,
	t_tourdate VARCHAR(100)not null
);

CREATE TABLE t_tourexpense(
			t_expenseid SERIAL PRIMARY KEY,
			t_tourid int not null,
			t_expensename VARCHAR(100) not null,
			t_tourdate date not null,
			t_tourprice int not null
)
