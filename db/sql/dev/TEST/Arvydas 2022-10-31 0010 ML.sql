EXEC prcUiWordUpdate 'confirmChangeServer', N'Changing the server settings will immediately destroy the current login information, and you will have to log in again afterwards. Are you sure you want to change server?', N'Pakeitus serverio nustatymus bus iš karto sunaikinta dabartinio prisijungimo informacija, ir po to teks prisijungti iš naujo. Ar tikrai norite pakeisti serverį?', N'Изменение настроек сервера немедленно уничтожит текущую информацию для входа, и после этого вам придется снова войти в систему. Вы уверены, что хотите сменить сервер?'

EXEC prcUiWordUpdate 'public', N'Public', N'Viešas', N'Публичный'
EXEC prcUiWordUpdate 'private', N'Private', N'Privatus', N'Частный'
EXEC prcUiWordUpdate 'audience', N'Audience', N'Auditorija', N'Аудитория'

/*
Select * from tblUiWord where alias LIKE '%pub%'
Select * from tblUiWord where alias LIKE '%friend%'
Select * from tblUiWord where alias LIKE '%priv%'

Select * from tblUiWord where alias LIKE '%audi%'

*/