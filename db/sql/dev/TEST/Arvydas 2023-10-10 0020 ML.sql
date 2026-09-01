EXEC prcUiWordUpdate 'objectId', N'Object Id', N'Objekto Id', N'Идентификатор объекта'
EXEC prcUiWordUpdate 'normal', N'Normal', N'Normalus', N'Нормальный'
EXEC prcUiWordUpdate 'virtual', N'Virtual', N'Virtualus', N'Виртуальный'

select * from tblUiWord where alias = 'virtual'