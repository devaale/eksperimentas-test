EXEC prcUiWordUpdate 'noLicenseX', N'We are sorry, but you need a {0} to use the AI ​​Services.', N'Apgailestaujame, tačiau jums reikalinga {0}, kad naudotis dirbtinio intelekto paslaugomis.', N'К сожалению, для использования сервиса ИИ вам необходима {0}.'
EXEC prcUiWordUpdate 'weAreSorry', N'We are sorry', N'Atsiprašome', N'Нам очень жаль'

select * from tblUiWord where alias like '%lic%'