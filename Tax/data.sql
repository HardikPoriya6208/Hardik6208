CREATE TABLE t_taxuser(
	c_userid SERIAL PRIMARY KEY,
	c_username VARCHAR(100)not null,
	c_email VARCHAR(100)not null,
	c_password VARCHAR(100)not null,
	c_gender VARCHAR(100)not null,
	c_mobile VARCHAR(100)not null,
	c_state VARCHAR(100)not null,
	c_city VARCHAR(100)not null,
	c_image VARCHAR(500),
	c_type VARCHAR(100)not null,
	c_role varchar(20)not null DEFAULT 'User'
) 

select * from t_taxuser;

INSERT INTO t_taxuser(c_username,c_email,c_password,c_gender,c_mobile,c_state,c_city,c_image,c_type,c_role)
	VALUES('admin','admin123@gmail.com','Hardik@6208','Male','9106230614','Gujrat','Surat','HP.png','Salary','Admin');


INSERT INTO t_taxuser(c_username,c_email,c_password,c_gender,c_mobile,c_state,c_city,c_image,c_type,c_role)
	VALUES('hardik','hardik123@gmail.com','Hardik@6208','Male','9106230614','Gujrat','Surat','HP.png','Freelance','User');


CREATE TABLE t_tran(
	t_tranid SERIAL PRIMARY KEY,
	c_userid int not null,
	t_tranname VARCHAR(100)not null,
	t_trantype VARCHAR(100)not null,
	t_taxable VARCHAR(100)not null,
	t_taxamount int not null
)