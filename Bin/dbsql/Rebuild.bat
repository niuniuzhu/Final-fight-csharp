@echo off  
:begin
@echo ----------1, create all game database------------ 
mysql -uroot -pron159753<createdb.sql
mysql -uroot -pron159753<grantuser.sql 

mysql -uroot -pron159753<fball_accountdb.sql
mysql -uroot -pron159753<fball_chargedb.sql
mysql -uroot -pron159753<fball_robedb.sql

mysql -uroot -pron159753 fball_accountdb < fball_accountdb.sql
mysql -uroot -pron159753 fball_chargedb < fball_chargedb.sql
mysql -uroot -pron159753 fball_robedb < fball_robedb.sql

mysql -uroot -pron159753 fball_gamedb_1 < fball_gamedb.sql
mysql -uroot -pron159753 fball_gamedb_2 < fball_gamedb.sql
mysql -uroot -pron159753 fball_gamedb_3 < fball_gamedb.sql 

mysql -uroot -pron159753 fball_logdb_1 < fball_logdb.sql
mysql -uroot -pron159753 fball_logdb_2 < fball_logdb.sql
mysql -uroot -pron159753 fball_logdb_3 < fball_logdb.sql 

