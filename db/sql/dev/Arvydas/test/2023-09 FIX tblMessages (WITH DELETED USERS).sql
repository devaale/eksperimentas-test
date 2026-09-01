select * from AspNetUsers

select distinct m.ReceiverUserId, ru.Name, m.SenderUserId, su.Name
from tblMessage m
left join AspNetUsers ru on ru.Id = m.ReceiverUserId
left join AspNetUsers su on su.Id = m.SenderUserId


declare @oldUserId nvarchar(128), @newUserid nvarchar(128)
SET @oldUserId = 'a383d38b-1a87-49a8-96c8-e661e9ce618e'
SET @newUserid = '5D697D4E-A967-4506-BE09-352F1EB36D0A' -- Useris 1


update tblMessage
set  ReceiverUserId = @newUserid
where ReceiverUserId = @oldUserId

update tblMessage
set  SenderUserId = @newUserid
where SenderUserId = @oldUserId
