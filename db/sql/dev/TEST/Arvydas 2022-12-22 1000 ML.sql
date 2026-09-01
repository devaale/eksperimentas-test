EXEC prcUiWordUpdate 'friendsAndBlocked', N'Friends and Blocked', N'Draugai ir užblokuoti', N'Друзья и заблокированные'

EXEC prcUiWordUpdate 'friends', N'Friends', N'Draugai', N'Друзья'
EXEC prcUiWordUpdate 'blocked', N'Blocked', N'Blokuoti', N'Блокированные'
EXEC prcUiWordUpdate 'operationSuccess', N'Operation successful!', N'Operacija sėkminga!', N'Операция успешна!'

EXEC prcUiWordUpdate 'tokenBallance', N'Your token balance: {0}.', N'Jūsų tokenų likutis: {0}.', N'Баланс ваших токен: {0}.'
EXEC prcUiWordUpdate 'giveToken', N'Give token', N'Duoti Tokeną', N'Дать токен'

EXEC prcUiWordUpdate 'sameSenderReceiver', N'Same sender and recipient!', N'Tas pats siuntėjas ir gavėjas!', N'Один и тот же отправитель и получатель!'

select * from tblUiWord where alias LIKE '%fail%'

--SELECT Id, Name, Tokens FROM AspNetUsers 
--SELECT * FROm tblTokenTransaction