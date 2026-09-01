Kad įdiegti back-end, bandyti sketi šiomis instrukcijomis: 
==========================================================
https://docs.microsoft.com/en-us/aspnet/core/tutorials/publish-to-iis?view=aspnetcore-6.0&tabs=visual-studio

	1.	Install the .NET Core Hosting Bundle on Windows Server. (Admin teisės)
		https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-6.0.2-windows-hosting-bundle-installer
		
	2. Tam dar reiks .NET Core SDK (greičiausiai ir taip bus, aš neperinstaliavau nes jau buvo)
		https://docs.microsoft.com/en-us/dotnet/core/sdk
		
	
Idėja tokia, kad IIS jeigu pats kuri website ir pridedi .net projektą, gaunasi kažkokia ahinėja... Užsiknisimas ir niekas neveikia.
Tačiau aš po to pašalinau IIS savo sukurtą virtualią direktoriją ir maždaug viską tvarkingai 
ASP.net core projekte nurodžiau ir folderis IIS konfigūracijoje savaime atsirado ir kaip ir viskas pasileido.

Tačiau irgi ne belekokia nuoroda, bet tik į labai konkretų Web page.
		
