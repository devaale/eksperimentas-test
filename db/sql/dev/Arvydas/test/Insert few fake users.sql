INSERT INTO AspNetUsers (
	Id, Name, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, UserName
) VALUES 
	(NEWID(), 'Bolshevikas', 0, 0, 0, 0, 0, 'Bolshevikas@energus.eu' ),
	(NEWID(), 'Menshevikas', 0, 0, 0, 0, 0, 'Menshevikas@energus.eu' )