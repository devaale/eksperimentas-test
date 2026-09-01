EXEC prcUiWordUpdate 'virtualDatapoint', N'Virtual data point', N'Virtualus duomenų taškas', N'Виртуальная точка данных'
EXEC prcUiWordUpdate 'function', N'Function', N'Funkcija', N'Функция'
EXEC prcUiWordUpdate 'createAlarm', N'Create alarm', N'Sukurti aliarmą', N'Создать алярм'
EXEC prcUiWordUpdate 'aggregateBy', N'Aggregate by', N'Agreguoti pagal', N'Агрегировать по'
EXEC prcUiWordUpdate 'formulaParts', N'Parts of the formula', N'Formulės dalys', N'Части формулы'
EXEC prcUiWordUpdate 'formulaPart', N'Part of the formula', N'Formulės dalis', N'Часть формулы'
EXEC prcUiWordUpdate 'validation', N'Validation', N'Validavimas', N'Валидация'
EXEC prcUiWordUpdate 'errFormulaParts', N'The parts of the formula are messy. Each of these must be either a numerical value or a specified data point. You can remove unnecessary parts of the formula in cases where the number of formula members is not fixed.', N'Formulės dalys yra netvarkingos. Kiekvienai iš jų turi būti nurodyta arba skaitinė reikšmė, arba nurodytas duomenų taškas. Nereikalingas formulės dalis galite pašalinti, tais atvejais, kai formulės narių skaičius nėra fiksuotas.', N'Части формулы беспорядочны. Каждый из них должен быть либо числовым значением, либо определенной точкой данных. Вы можете удалить ненужные части формулы в тех случаях, когда количество членов формулы не фиксировано.'

DELETE FROM tblDatapointFormula

-- Daugyba
EXEC prcUiWordUpdate 'multiplication', N'Multiplication', N'Daugyba', N'Умножение'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (10, 'multiplication', 0, 0)

-- Dalyba
EXEC prcUiWordUpdate 'division', N'Division', N'Dalyba', N'Разделение'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (20, 'division', 0, 0)

-- Sudėtis
EXEC prcUiWordUpdate 'addition', N'Addition', N'Sudėtis', N'Сложение'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (30, 'addition', 0, 0)

-- Atimtis
EXEC prcUiWordUpdate 'subtraction', N'Subtraction', N'Atimtis', N'Вычитание'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (40, 'subtraction', 0, 0)

-- Skirtumas
EXEC prcUiWordUpdate 'difference', N'Difference', N'Skirtumas', N'Разница'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (50, 'difference', 0, 0)

-- Minimumas
EXEC prcUiWordUpdate 'minVal', N'Minimum value', N'Minimali reikšmė', N'Минимальное значение'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (60, 'minVal', 0, 1)

-- Vidurkis
EXEC prcUiWordUpdate 'avgVal', N'Average value', N'Vidurkis', N'Среднее значение'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (70, 'avgVal', 0, 1)

-- Maksimali reikšmė
EXEC prcUiWordUpdate 'maxVal', N'Average value', N'Maksimali reikšmė', N'Среднее значение'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (80, 'maxVal', 0, 1)

-- Suma
EXEC prcUiWordUpdate 'sum', N'Sum', N'Suma', N'Сумма'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (90, 'sum', 0, 1)

-- Kiekis
EXEC prcUiWordUpdate 'count', N'Count', N'Kiekis', N'Количество'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (100, 'count', 0, 1)

-- NOW Martynas' formulas...

-- Poveikis aplinkai
EXEC prcUiWordUpdate 'environmentalImpact', N'Environmental impact', N'Poveikis aplinkai', N'Воздействие на окружающую среду'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (1010, 'environmentalImpact', 1, 0)

-- Šiluminis komfortas
EXEC prcUiWordUpdate 'thermalComfort', N'Thermal comfort', N'Šiluminis komfortas', N'Тепловой комфорт'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (1020, 'thermalComfort', 1, 0)

--  Fanger PMV
EXEC prcUiWordUpdate 'fangerPmv', N'Fanger PMV', N'Fanger PMV', N'фангер ПМВ'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (1030, 'fangerPmv', 6, 0)


/*
EXEC prcUiWordUpdate 'efficiencyCalculation', N'Efficiency calculation', N'Efektyvumo skaičiavimas', N'Расчет эффективности'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (1, 'efficiencyCalculation', 0)

EXEC prcUiWordUpdate 'primaryEnergy', N'Primary energy', N'Pirminė energija', N'Первичная энергия'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (11, 'primaryEnergy', 0)


EXEC prcUiWordUpdate 'economicRationalitySummer', N'Economic rationality in the summer', N'Ekonominis racionalumas vasarą', N'Экономическая рациональность летом'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (13, 'economicRationalitySummer', 0)

EXEC prcUiWordUpdate 'economicRationalityWinter', N'Economic rationality in winter', N'Ekonominis racionalumas žiemą', N'Экономическая рациональность зимой'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (131, 'economicRationalityWinter', 0)

EXEC prcUiWordUpdate 'meanRadiantTemperature', N'Mean radiant temperature', N'Vidutinė spinduliavimo temperatūra', N'Средняя лучистая температура'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (1511, 'meanRadiantTemperature', 0)

EXEC prcUiWordUpdate 'relativeAirConstant', N'Relative air constant', N'Santykinė oro konstanta', N'Относительная воздушная постоянная'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (1512, 'relativeAirConstant', 0)

EXEC prcUiWordUpdate 'clothing', N'Clothing', N'Apranga', N'Одежда'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (1513, 'clothing', 0)

EXEC prcUiWordUpdate 'metabolicRateConstant', N'Metabolic rate constant', N'Metabolizmo greičio konstanta', N'Константа скорости метаболизма'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (1514, 'metabolicRateConstant', 0)

EXEC prcUiWordUpdate 'humidity', N'Humidity', N'Drėgmė', N'Влажность'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (1515, 'humidity', 0)

EXEC prcUiWordUpdate 'indoorTemperature', N'Indoor temperature', N'Vidaus temperatūra', N'Температура в помещении'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (1516, 'indoorTemperature', 0)

EXEC prcUiWordUpdate 'technologicalElectricity', N'Technological electricity', N'Technologinė elektra', N'Технологическое электричество'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (2, 'technologicalElectricity', 0)

EXEC prcUiWordUpdate 'technologicalElectricity', N'Technological electricity', N'Technologinė elektra', N'Технологическое электричество'
INSERT INTO tblDatapointFormula (Id, Alias, NumDatapoints, Aggregated) VALUES (2, 'technologicalElectricity', 0)

*/

--Select * from tblUiWord where alias like '%valid%'