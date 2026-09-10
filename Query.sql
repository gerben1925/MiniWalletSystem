select * from dbo.Users


select * from dbo.Wallet
    
    
-- User 2
INSERT INTO [dbo].[Users] (Username, Password, Fullname)
VALUES ('testuser02', 'P@ssw0rd123', 'Jane Smith');

DECLARE @UserId2 INT = SCOPE_IDENTITY();

INSERT INTO [dbo].[Wallet] (UsersId, AccountNumber, Balance)
VALUES (@UserId2, 1000000002, 750.00);

-- User 3
INSERT INTO [dbo].[Users] (Username, Password, Fullname)
VALUES ('testuser03', 'P@ssw0rd123', 'Michael Reyes');

DECLARE @UserId3 INT = SCOPE_IDENTITY();

INSERT INTO [dbo].[Wallet] (UsersId, AccountNumber, Balance)
VALUES (@UserId3, 1000000003, 1200.50);

-- User 4
INSERT INTO [dbo].[Users] (Username, Password, Fullname)
VALUES ('testuser04', 'P@ssw0rd123', 'Ana Cruz');

DECLARE @UserId4 INT = SCOPE_IDENTITY();

INSERT INTO [dbo].[Wallet] (UsersId, AccountNumber, Balance)
VALUES (@UserId4, 1000000004, 300.75);