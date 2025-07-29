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
	email varchar(255) not null unique check (email like '%@%'),
	brTel varchar(10) not null unique
)

insert into klijenti values('Aron','Irwin',458697188,'1981-03-26','aron.irwin@gmail.com',1766057452)
insert into klijenti values('Elisha','Cruz',273097286,'1988-11-21','elisha.cruz@gmail.com',3805678730)
insert into klijenti values('Kira','Estes',658533684,'1992-06-01','kira.estes@gmail.com',9299083450)
insert into klijenti values('Leslie','Benton',978808053,'1984-11-22','leslie.benton@gmail.com',7446288454)
insert into klijenti values('Morgan','Love',327137633,'1971-11-22','morgan.love@gmail.com',4961158296)
insert into klijenti values('Heather','Kent',426259240,'1973-07-24','heather.kent@gmail.com',7531997334)
insert into klijenti values('Katerina','Bernard',334108342,'1973-02-01','katerina.bernard@gmail.com',6225916960)
insert into klijenti values('Husna','Wong',130427521,'1995-11-02','husna.wong@gmail.com',1576590966)
insert into klijenti values('Joseph','Brandt',477283903,'1989-10-09','joseph.brandt@gmail.com',3029860794)
insert into klijenti values('Rose','Calderon',260803037,'1994-05-03','rose.calderon@gmail.com',1253387152)

---------------------------------------------------------------------------------------
create table paketi
(
	idPaketa int IDENTITY(1,1) primary key,
	nazivPak varchar(50) not null,
	cena float not null,
	vrstaPak varchar(15) not null check(vrstaPak in('more','planina','ekskurzija','krstarenje')),
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

insert into paketi(nazivPak,cena,vrstaPak,destinacija,tipPrevoza,vrstaSmestaja) values('Grcka Letovanje',35233.76,'more','Halkidiki','Bus','Hotel')
insert into paketi(nazivPak,cena,vrstaPak,destinacija,tipPrevoza,vrstaSmestaja) values('Grcka Letovanje',20233.76,'more','Halkidiki','Bus','Hostel')
insert into paketi(nazivPak,cena,vrstaPak,destinacija,tipPrevoza,vrstaSmestaja,dodatneAktivnosti) values('Zlatibor Zimovanje',40031.33,'planina','Zlatibor','Bus','Apartman',null)
insert into paketi(nazivPak,cena,vrstaPak,destinacija,tipPrevoza,vodic,trajanje) values('Italija',37330.22,'ekskurzija','Rim','Bus','da','05:30:00')
insert into paketi(nazivPak,cena,vrstaPak,ruta,datumPolaska,tipKabine) values('Krstarenje do SAD',320000,'krstarenje','Genoa,Italija-Barselona,Spanija-Miami,SAD-Kartahena,Kolumbija-San Diego,Kalifornija,SAD','2025-05-25','Prozorska')
insert into paketi(nazivPak,cena,vrstaPak,destinacija,tipPrevoza,vrstaSmestaja) values('Turska',25733.40,'more','Kusadasi','Avion','Pansion')
insert into paketi(nazivPak,cena,vrstaPak,destinacija,tipPrevoza,vrstaSmestaja,dodatneAktivnosti) values('Himalaji',89999.99,'planina','Himalaji','Avion','Hotel','Skijanje')

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

insert into rezervacije values(1,1,2,'2025-06-30','Mini-bar;Budjenje')
insert into rezervacije values(3,5,4,'2025-05-25','Budjenje')

---------------------------------------------------------------------------------------

--Enkripcija Broja Pasosa
CREATE MASTER KEY ENCRYPTION BY   
PASSWORD = 'dizajniranjesoftvera';

CREATE CERTIFICATE AgencijaSertifikat
   WITH SUBJECT = 'Broj Pasosa Klijenata';  
GO  

CREATE SYMMETRIC KEY Pasos_Key_01 
    WITH ALGORITHM = AES_256  
    ENCRYPTION BY CERTIFICATE AgencijaSertifikat;  
GO  

USE agencija;  
GO  
  
ALTER TABLE klijenti
    ADD EncryptedPasos varbinary(128);   
GO  


OPEN SYMMETRIC KEY Pasos_Key_01
   DECRYPTION BY CERTIFICATE AgencijaSertifikat;  

UPDATE klijenti 
SET EncryptedPasos = EncryptByKey(Key_GUID('Pasos_Key_01'), CAST(brPasosa as varchar(10)));  
GO  
 
OPEN SYMMETRIC KEY Pasos_Key_01 
   DECRYPTION BY CERTIFICATE AgencijaSertifikat;  
GO  

SELECT brPasosa, EncryptedPasos  
    AS 'Enkriptovan Broj Pasosa',  
    CONVERT(varchar, DecryptByKey(EncryptedPasos))   
    AS 'Dekriptovan Broj Pasosa'  
    FROM klijenti;  
GO

CLOSE SYMMETRIC KEY Pasos_Key_01 
---------------------------------------------------------------------------------------

--prikazivanje liste svih klijenata
select *
from klijenti k

--prikazivanje liste svih paketa razdvojeno po tipovima

--more
select idPaketa,nazivPak,cena,vrstaPak,destinacija,tipPrevoza,vrstaSmestaja
from paketi
where vrstaPak = 'more'

--planina
select idPaketa,nazivPak,cena,vrstaPak,destinacija,vrstaSmestaja,dodatneAktivnosti
from paketi
where vrstaPak = 'planina'

--ekskurzija
select idPaketa,nazivPak,cena,vrstaPak,destinacija,tipPrevoza,vodic,trajanje
from paketi
where vrstaPak = 'ekskurzija'

--krstarenje
select idPaketa,nazivPak,cena,vrstaPak,ruta,datumPolaska,tipKabine
from paketi
where vrstaPak = 'krstarenje'

--prikazivanje liste svih rezervacija

select *
from rezervacije

--prikazivanje liste svih destinacija

select distinct destinacija
from paketi
where destinacija is not null