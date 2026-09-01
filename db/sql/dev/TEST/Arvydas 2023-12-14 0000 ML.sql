-- Devices
EXEC prcUiWordUpdate 'clientUsername',				N'Client user name',				N'Kliento vartotojo vardas',			N'Имя пользователя клиента'
EXEC prcUiWordUpdate 'clientPassword',				N'Client password',					N'Kliento slaptažodis',					N'Пароль клиента'
EXEC prcUiWordUpdate 'unit-id',						N'Unit / Instance Id',				N'Vieneto / egzemplioriaus ID',			N'Идентификатор объекта/экземпляра'

-- Datapoints
EXEC prcUiWordUpdate 'instance',					N'Instance',						N'Egzempliorius',						N'Экземпляр'

-- BACnetObjectType
EXEC prcUiWordUpdate 'objectType',					N'Object type',						N'Objekto tipas',						N'Тип объекта'

EXEC prcUiWordUpdate 'analogInputAI',				N'Analog Input (AI)',				N'Analoginė įvestis (AI)',				N'Аналоговый вход (AI)'
EXEC prcUiWordUpdate 'analogOutputAO',				N'Analog Output (AO)',				N'Analoginis išėjimas (AO)',			N'Аналоговый выход (AO)'
EXEC prcUiWordUpdate 'binaryInputBI',				N'Binary Input (BI)',				N'Dvejetainė įvestis (BI)',				N'Двоичный вход (BI)'
EXEC prcUiWordUpdate 'binaryOutputBO',				N'Binary Output (BO)',				N'Dvejetainė išvestis (BO)',			N'Двоичный выход (BO)'
EXEC prcUiWordUpdate 'multistateInputMI',			N'Multi-state Input (MI)',			N'Kelių būsenų įvestis (MI)',			N'Многопозиционный вход (MI)'
EXEC prcUiWordUpdate 'multistateOutputMO',			N'Multi-state Output (MO)',			N'Kelių būsenų išvestis (MO)',			N'Многоуровневый выход (MO)'
EXEC prcUiWordUpdate 'calendar',					N'Calendar',						N'Kalendorius',							N'Календарь'
EXEC prcUiWordUpdate 'trendLog',					N'Trend Log',						N'Tendencijų žurnalas',					N'Журнал трендов'


EXEC prcUiWordUpdate 'propertyId',					N'Property Id',						N'Savybės Id',							N'Идентификатор свойства'

EXEC prcUiWordUpdate 'presentValue85',				N'Present Value (85)',				N'Dabartinė vertė (85)',				N'Текущая стоимость (85)'
EXEC prcUiWordUpdate 'statusFlags111',				N'Status Flags (111)',				N'Būsenos vėliavėlės (111)',			N'Флаги статуса (111)'
EXEC prcUiWordUpdate 'objectName77',				N'Object Name (77)',				N'Objekto pavadinimas (77)',			N'Имя объекта (77)'
EXEC prcUiWordUpdate 'highLimit56',					N'High Limit (56)',					N'Aukščiausia riba (56)',				N'Высокий лимит (56)'
EXEC prcUiWordUpdate 'lowlimit54',					N'Low limit (54)',					N'Žemiausia riba (54)',					N'Нижний предел (54)'
EXEC prcUiWordUpdate 'description28',				N'Description (28)',				N'Aprašymas (28)',						N'Описание (28)'
EXEC prcUiWordUpdate 'eventState23',				N'Event State (23)',				N'Įvykio būsena (23)',					N'Состояние события (23)'
EXEC prcUiWordUpdate 'lifeSafetyAlarm121',			N'Life Safety Alarm (121)',			N'Gyvybės saugos signalizacija (121)',	N'Сигнализация безопасности жизни (121)'
EXEC prcUiWordUpdate 'alarmValue101',				N'Alarm Value (101)',				N'Signalo reikšmė (101)',				N'Значение тревоги (101)'
EXEC prcUiWordUpdate 'priorityArray87',				N'Priority Array (87)',				N'Prioritetinis masyvas (87)',			N'Приоритетный массив (87)'
EXEC prcUiWordUpdate 'units19',						N'Units (19)',						N'Vienetai (19)',						N'Единицы (19)'
EXEC prcUiWordUpdate 'reliability65',				N'Reliability (65)',				N'Patikimumas (65)',					N'Надежность (65)'
EXEC prcUiWordUpdate 'resolution118',				N'Resolution (118)',				N'Rezoliucija (118)',					N'Резолюция (118)'

EXEC prcUiWordUpdate 'dataType',					N'Data type',						N'Duomenų tipas',						N'Тип данных'

EXEC prcUiWordUpdate 'readProperty0x0C',			N'0x0C - Read Property',			N'0x0C – skaityti ypatybę',				N'0x0C — чтение свойства'
EXEC prcUiWordUpdate 'writeProperty0x0F',			N'0x0F - Write Property',			N'0x0F – įrašyti ypatybę',				N'0x0F — запись свойства'
EXEC prcUiWordUpdate 'whoIs0x01',					N'0x01 – Who-Is',					N'0x01 – kas yra',						N'0x01 – Кто есть'
EXEC prcUiWordUpdate 'iAm0x02',						N'0x02 – I-Am',						N'0x02 – Aš esu',						N'0x02 – Я-есть'
EXEC prcUiWordUpdate 'readPropertyMultiple0x10',	N'0x10 - Read Property Multiple',	N'0x10 – Skaityti keletą savybių',		N'0x10 — чтение нескольких свойств'
EXEC prcUiWordUpdate 'writePropertyMultiple0x12',	N'0x12 - Write Property Multiple',	N'0x12 – Įrašyti keletą savybių',		N'0x12 — запись нескольких свойств'

EXEC prcUiWordUpdate 'resourceUri',					N'Resource Uri',					N'Resurso Uri',							N'Uri Ресурса'
EXEC prcUiWordUpdate 'payload',						N'Payload',							N'Naudinga apkrova',					N'Полезная нагрузка'


select * from tblUiWord where alias like '%res%'

--delete from tblUiWord where alias = 'clientName'