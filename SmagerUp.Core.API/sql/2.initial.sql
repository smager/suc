INSERT INTO Licenses (Name,Type,Description,TermsUrl)
VALUES
('SFUL','FreeUseNonModifiable','Free to use but cannot modify','https://smagerup.com/licenses/sful.txt'),
('SCL','PaidNonModifiable','Paid license for commercial use','https://smagerup.com/licenses/scl.txt');

INSERT INTO Modules (Name,Version,Code,Description,LicenseId,Price)
VALUES
('grid','1.0.0','/* minified grid code */','Basic grid module',1,0),
('chart-advanced','2.0.0','/* minified chart code */','Advanced charts',2,99.0);

INSERT INTO Accounts (FirstName,LastName,ApiKey)
VALUES ('German','Fuentes','v0rqU9/59fLeUJtVdJ+6Pg==');

DECLARE @acc UNIQUEIDENTIFIER=(SELECT TOP 1 AccountId FROM Accounts);
INSERT INTO UserModules (AccountId,ModuleId) SELECT @acc,ModuleId FROM Modules WHERE Price=0;
