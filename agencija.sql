create database agencija

go

use agencija

---------------------------------------------------------------------------------------
create table klijenti
(
	idKlijenta int IDENTITY(1,1) primary key,
	ime varchar(20) not null,
	prezime varchar(20) not null,
	brPasosa int not null unique check (brPasosa between 100000000 and 999999999),
	datumRodjenja date not null,
	email varchar(255) null check (email like '%@%'),
	brTel varchar(10) not null
)

---------------------------------------------------------------------------------------
create table paketi
(
	idPaketa int IDENTITY(1,1) primary key,
	nazivPak varchar(50) not null,
	cena float not null,
	vrstaPak varchar(15) not null,
	destinacija varchar(20) null,
	tipPrevoza varchar(10) null,
	vrstaSmestaja varchar(20) null,
	dodatneAktivnosti varchar(100) null,
	vodic varchar(2) null check(vodic in('da','ne')),
	trajanje time null,
	brod varchar(30) null,
	ruta varchar(100) null,
	datumPolaska date null,
	tipKabine varchar(20) null
)
---------------------------------------------------------------------------------------
create table rezervacije
(
	idKlijenta int,
	idPaketa int,
	brPutnika int,
	datum date,
	dodatneUsluge varchar(100) null,
	primary key(idKlijenta,idPaketa),
	foreign key(idKlijenta) references klijenti(idKlijenta) on delete cascade,
	foreign key(idPaketa) references paketi(idPaketa) on delete cascade
)