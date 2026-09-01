exec prcUiWordUpdate 'home', N'Home', N'Pradžia', N'Начало'
exec prcUiWordUpdate 'hello', N'Hello', N'Sveiki', N'Превед'
exec prcUiWordUpdate 'user-settings', N'User''s Settings', N'Vartotojo nustatymai', N'Пользовательские настройки'

select * from tblUiWord where alias = 'user-settings'