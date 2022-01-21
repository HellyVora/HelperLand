create table users (
userid int primary key,
mail varchar(100) Not null unique,
pswd varchar(16) Not null,
);

create table Customer(
cid int primary key,
userid int,
first_name varchar(50) not null,
last_name varchar(50) not null,
contact_no bigint,
dob date,
language varchar(20),
pet bit,
mail varchar(100) not null unique,
Foreign key(userid) references users(userid),
);

create table Addresses(
userid int not null,
house_no int not null,
street int not null,
city varchar(50) not null,
county varchar(50) not null,
landmark varchar(50),
portal int not null,
Foreign key(userid) references users(userid),

);
 
create table Administration(
	aid int primary key,
	userid int not null,
	mail varchar(50) not null,
	Foreign key(userid) references users(userid),	
);

alter table users
add type_of_user varchar(5);

create table Service_provider(
spid int primary key,
userid int not null,
first_name varchar(50) not null,
last_name varchar(50) not null,
contact_no bigint,
dob date,
lang varchar(10),
gender char(1),
nationality varchar(20),
tax_no bigint not null,
Acc_holder_name varchar(50) not null,
IBAN int not null,
mail varchar(50) not null,
Foreign key(userid) references users(userid),
);

create table Serves
(
serve_id int primary key,
cid int not null,
spid int not null,
date_of_service date not null,
start_time time not null,
end_time time not null,
payment bit not null,
rating int,
done bit not null,
Foreign key(cid) references Customer(cid),
Foreign key(spid) references Service_Provider(spid),
);

create table All_services_list(
service_id int primary key,
sname varchar(50) not null,
cost int not null,
);

alter table Serves
add service_id int not null foreign key(service_id) references  All_services_list(service_id);

create table Available(
spid int primary key foreign key(spid) references Service_Provider(spid),
mon time,
tue time,
wed time,
thrus time,
fri time,
sat time,
sun time,
);

create table Invoice(
serve_id int primary key foreign key(serve_id) references Serves(serve_id),
invoice_no int not null unique,
spid int foreign key(spid) references Service_provider(spid),
anount int not null,
);

create table Prefrences(
spid int foreign key(spid) references Service_provider(spid),
cid  int foreign key(cid) references Customer(cid),
preference bit not null,
);