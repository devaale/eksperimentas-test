EXEC prcUiWordUpdate 'contact', N'Contact', N'Susisiekti', N'Связаться'

EXEC prcUiWordUpdate 'userProfile', N'User profile', N'Vartotojo profilis', N'Профиль пользователя'

EXEC prcUiWordUpdate 'addFriend', N'Add as friend', N'Pridėti kaip draugą', N'Добавить в друзья'
EXEC prcUiWordUpdate 'addFriendConfirm', N'Are you sure you want to add this user as a friend? This user will then be able to see your devices.', N'Ar tikrai norite pridėti šį vartotoją kaip draugą? Šis vartotojas po to galės matyti jūsų įrenginius.', N'Вы уверены, что хотите добавить этого пользователя в друзья? После этого этот пользователь сможет видеть ваши устройства.'
EXEC prcUiWordUpdate 'addFriendDone', N'The user has been added to friends!', N'Vartotojas buvo įtrauktas į draugus!', N'Пользователь добавлен в друзья!'

EXEC prcUiWordUpdate 'unfriend', N'Unfriend', N'Pašalinti iš draugų', N'Удалить из друзей'
EXEC prcUiWordUpdate 'unfriendConfirm', N'Are you sure you want to remove this user from friends?', N'Ar tikrai norite pašalinti šį vartotoją iš draugų?', N'Вы уверены, что хотите удалить этого пользователя из друзей?'
EXEC prcUiWordUpdate 'unfriendDone', N'User has been removed from friends!', N'Vartotojas pašalintas iš draugų!', N'Пользователь удален из друзей!'

EXEC prcUiWordUpdate 'block', N'Block', N'Blokuoti', N'Блокировать'
EXEC prcUiWordUpdate 'blockConfirm', N'Are you sure you want to block this user?', N'Ar tikrai norite užblokuoti šį vartotoją?', N'Вы уверены, что хотите заблокировать этого пользователя?'
EXEC prcUiWordUpdate 'blockDone', N'User has been blocked!', N'Vartotojas buvo užblokuotas!', N'Пользователь заблокирован!'

EXEC prcUiWordUpdate 'unblock', N'Unblock', N'Atblokuoti', N'Разблокировать'
EXEC prcUiWordUpdate 'unblockConfirm', N'Are you sure you want to unblock this user?', N'Ar tikrai norite atblokuoti šį vartotoją?', N'Вы уверены, что хотите разблокировать этого пользователя?'
EXEC prcUiWordUpdate 'unblockDone', N'User has been unblocked!', N'Vartotojas buvo atblokuotas!', N'Пользователь разблокирован!'

EXEC prcUiWordUpdate 'yourProfile', N'This is your profile.', N'Tai jūsų profilis.', N'Это ваш профиль.'

EXEC prcUiWordUpdate 'operationFailed', N'Operation failed.', N'Operacija nepavyko.', N'Операция не удалась.'

-- Conversations
EXEC prcUiWordUpdate 'chat', N'Chat', N'Pokalbiai', N'Чат'
EXEC prcUiWordUpdate 'conversations', N'Conversations', N'Pokalbiai', N'Разговоры'
EXEC prcUiWordUpdate 'send', N'Send', N'Siųsti', N'Отправить'
EXEC prcUiWordUpdate 'messageText', N'Message text', N'Žinutės tekstas', N'Текст сообщения'

EXEC prcUiWordUpdate 'new_M', N'New', N'Naujas', N'Новый'
EXEC prcUiWordUpdate 'new_F', N'Nauja', N'Naujas', N'Новая'

EXEC prcUiWordUpdate 'searchText', N'Search text', N'Paieškos tekstas', N'Текст поиска'
EXEC prcUiWordUpdate 'clear', N'Clear', N'Išvalyti', N'Очистить'



select * from tblUiWord where alias like '%clear%'