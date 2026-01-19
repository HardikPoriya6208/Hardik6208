create TABLE t_libraryuser(
	c_userid SERIAL PRIMARY KEY,
	c_username VARCHAR(100)not null,
	c_email VARCHAR(100)not null,
	c_password VARCHAR(100)not null,
	c_mobile VARCHAR(100)not null,
	c_department VARCHAR(100)not null,
	c_image VARCHAR(500),
	c_status VARCHAR(20)not null,
	c_role varchar(20)not null DEFAULT 'User'
);

select * from t_libraryuser;

insert into t_libraryuser(c_username,c_email,c_password,c_mobile,c_department,c_image,c_status,c_role)
	VALUES('admin','admin123@gmail.com','Hardik@6208','9106230614','admin','HP.png','Active','Admin');
insert into t_libraryuser(c_username,c_email,c_password,c_mobile,c_department,c_image,c_status,c_role)
	VALUES('hardik','hardik123@gmail.com','Hardik@6208','9106230614','user','HP.png','Active','User');


CREATE TABLE t_book(
			c_bookingid SERIAL PRIMARY KEY,
			c_bookname VARCHAR(100)not null,
			c_author VARCHAR(100)not null,
			c_category VARCHAR(100)not null,
			c_totalqty int not null,
			c_availableqty int not null,
			c_image VARCHAR(100)
)

select * from t_book;

INSERT INTO t_book
(c_bookname, c_author, c_category, c_totalqty, c_availableqty, c_image)
VALUES
('Clean Code', 'Robert C. Martin', 'Programming', 10, 7, 'cleancode.jpg'),
('The Pragmatic Programmer', 'Andrew Hunt', 'Programming', 8, 5, 'pragmatic.jpg'),
('Introduction to Algorithms', 'Thomas H. Cormen', 'Computer Science', 6, 4, 'algo.jpg'),
('Database System Concepts', 'Abraham Silberschatz', 'Database', 12, 9, 'dbms.jpg'),
('Design Patterns', 'Erich Gamma', 'Software Engineering', 5, 3, 'designpatterns.jpg');


CREATE TABLE t_BookIssue(
	c_issueid SERIAL PRIMARY KEY,
	c_userid int not null,
	c_bookname VARCHAR(100) not null,
	c_bookingid int not null,
	c_issueDate VARCHAR(100)not null,
	c_dueDate VARCHAR(100)not null,
	c_status VARCHAR(100)not null
)
