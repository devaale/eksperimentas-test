Čia dedami skriptai, kuriuos jūs sukuriate ir kurie dar nepaleisti ant serverio. 

Juos paleidus ant serverio, jie turi būti iš čia perkelti į atitinkamo hosto, serverio katalogą. 

O čia jie nepaliekami po to dėl to, kad developeris netyčia dar kartą konkretaus skripto nepaleistų ant serverio, kas tam tikrais atvejais gali lemti neigiamas pasekmes.

SQL Skriptai, kurie jau yra atlikti ant serverio. Failų pavadinimų neimingas:

[Developerio vardas][tarpas]
[data yyyy-mm-dd][tarpas]
[eilės numeris konkrečią dieną][tarpas]
[SQL Subjekto pavadinimas].sql

Pvz. Arvydas 2020-01-30 0004 prcBlabla.sql

Eilės numeris reikalingas dėl to, kad per vieną dieną programuotojas gali padaryti keletą pakeitimų, iš kurių kai kuriuos gali tekti startuoti konkrečia eilės tvarka. 
