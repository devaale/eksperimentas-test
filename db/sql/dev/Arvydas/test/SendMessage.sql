--SElect * from AspNetUsers

DECLARE @sender nvarchar(128)
SELECT @sender = N'a383d38b-1a87-49a8-96c8-e661e9ce618e'

DECLARE @receiver nvarchar(128)
SELECT @receiver = N'26b33240-b13e-406d-a2a3-b4e90af3c459'

INSERT INTO tblMessage (
	[SenderUserId], [ReceiverUserId], [Body]
)  VALUES (
	@sender, @receiver, 'TEXT'
)