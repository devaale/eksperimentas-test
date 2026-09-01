EXEC prcUiWordUpdate 'fillFields', N'Fill in the fields', N'Užpildykite laukus', N'Заполните поля'
EXEC prcUiWordUpdate 'shortUsername1', N'Username must be at least {0} characters.', N'Vartotojo vardas turi būti ne trumpesnis nei {0} simboliai.', N'Имя пользователя должно содержать не менее {0} символов.'
EXEC prcUiWordUpdate 'passwordMismatch', N'Password mismatch.', N'Slaptažodžiai neatitinka.', N'Пароли не совпадают.'
EXEC prcUiWordUpdate 'enterValidEmail', N'Please enter a valid email address.', N'Prašome įvesti galiojantį elektroninio pašto adresą', N'Введите действительный адрес электронной почты.'
EXEC prcUiWordUpdate 'passwordReq', N'Password must be at least {0} characters long. It must contain at least one uppercase and lowercase letter, a number and a symbol.', N'Slaptažodis turi būti bent {0} simbolių ilgio. Jame turi būti bent viena didžioji ir mažoji raidė, skaičius ir simbolis.', N'Пароль должен содержать не менее {0} символов. Он должен содержать как минимум одну заглавную и строчную букву, цифру и символ.'

--select * from tblUiWord WHERE ALIAS LIKE '%email%' 