EXEC prcUiWordUpdate 'newEntry', N'New entry', N'Naujas įrašas', N'Новая запись'

EXEC prcUiWordUpdate 'objectInfo', N'Object information', N'Objekto informacija', N'Информация об объекте'
EXEC prcUiWordUpdate 'objectShareToFriends', N'Give access to the object to friends:', N'Suteikti prieigą prie objekto draugams:', N'Дать доступ к объекту друзьям:'

EXEC prcUiWordUpdate 'sureDelete1', N'Are you sure you want to delete {0}?', N'Ar tikrai norite ištrinti {0}?', N'Вы уверены, что хотите удалить {0}?'

EXEC prcUiWordUpdate 'addImage', N'Add image', N'Pridėti paveikslėlį', N'Добавить картинку'
EXEC prcUiWordUpdate 'publish', N'Publish', N'Paskelbti', N'Публиковать'

EXEC prcUiWordUpdate 'alarms', N'Alarms', N'Aliarmai', N'Сигналы тревоги'
EXEC prcUiWordUpdate 'newContent', N'New content', N'Naujas turinys', N'Новый контент'

select * from tblUiWord where alias LIKE '%p%'