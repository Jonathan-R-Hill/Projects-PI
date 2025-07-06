USE Storage
GO
CREATE TABLE Users (
    ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Primary Key',
    CreatedDate DATE COMMENT 'CreatedDate' DEFAULT (CURRENT_DATE),
    `FirstName` NVARCHAR(255) NOT NULL COMMENT 'FirstName',
    `LastName` NVARCHAR(255) NOT NULL COMMENT 'Surname',
    `UserName` NVARCHAR(255) NOT NULL UNIQUE COMMENT 'UserName'
) COMMENT='Users';

CREATE TABLE Vocabulary (
    ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Primary Key',
    UserID INT NOT NULL COMMENT 'UserID',
    `KnownLanguage-Word` NVARCHAR(255) NOT NULL COMMENT 'Word',
    `TargetLanguage-Word` NVARCHAR(255) NOT NULL COMMENT 'Word',
    CreatedDate DATE COMMENT 'CreatedDate' DEFAULT (CURRENT_DATE),
    `Learnt` BOOLEAN NOT NULL DEFAULT 0 COMMENT 'Learnt',
    `LastPracticed` DATE COMMENT 'LastPracticed',
    `NextPractice` DATE COMMENT 'NextPractice',
    FOREIGN KEY (UserID) REFERENCES Users(ID)
) COMMENT='Vocabulary';

CREATE TABLE discord_vocab(  
    id int NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'Primary Key',
    userID VARCHAR(255) NOT NULL COMMENT 'User ID',
    english_word VARCHAR(255) NOT NULL COMMENT 'English word',
    korean_word VARCHAR(255) NOT NULL COMMENT 'Korean word',
    learned BOOLEAN NOT NULL DEFAULT FALSE COMMENT 'Whether the word has been learned',
    last_practiced TIMESTAMP NULL DEFAULT NULL COMMENT 'Last time the word was practiced'
) COMMENT 'Discord vocab practice table';
