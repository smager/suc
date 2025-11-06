INSERT INTO Contents(ContentId, uniqueName,title,Body,Type)
VALUES
('BE9BEC34-5045-4353-A888-177E7F6FC2AA','SFUL','Free Use Non Modifiable','Free to use but cannot modify','html'),
('A45D469C-7854-4A92-BC83-0772B1F62190','SCL','Paid Non Modifiable','Paid license for commercial use','html');

INSERT INTO LicenseTypes (LicenseTypeId,ContentId)
VALUES
('44E6FDD5-A3F6-4BC2-9D6B-44E7A1F3D0A8','BE9BEC34-5045-4353-A888-177E7F6FC2AA'),
('3E92110A-C8EC-4AD7-9EE4-B6F3AB7A66E4','A45D469C-7854-4A92-BC83-0772B1F62190');



INSERT INTO Accounts (FirstName,LastName,ApiKey)
VALUES ('German','Fuentes','v0rqU9/59fLeUJtVdJ+6Pg==');

DECLARE @acc UNIQUEIDENTIFIER=(SELECT TOP 1 AccountId FROM Accounts);
INSERT INTO UserModules (AccountId,ModuleId) SELECT @acc,ModuleId FROM Modules WHERE Price=0;
