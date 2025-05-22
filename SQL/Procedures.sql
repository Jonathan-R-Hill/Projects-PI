USE `Storage`;

DELIMITER //
----------- User Management Procedures -----------
-- Add User Procedure
CREATE PROCEDURE `AddUser`(
    IN firstName NVARCHAR(255),
    IN lastName NVARCHAR(255),
    IN userName NVARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: Unable to add user. Username may already exist or data is invalid.';
    END;

    IF LENGTH(TRIM(firstName)) = 0 OR LENGTH(TRIM(lastName)) = 0 OR LENGTH(TRIM(userName)) = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: All fields (FirstName, LastName, UserName) are required.';
    END IF;

    IF LENGTH(userName) > 255 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: UserName exceeds maximum length.';
    END IF;

    INSERT INTO Users (FirstName, LastName, UserName)
    VALUES (TRIM(firstName), TRIM(lastName), TRIM(userName));

    SELECT LAST_INSERT_ID() AS NewUserID;
END //

-- Get User ID by UserName
CREATE PROCEDURE GetUserIdByUserName(IN in_userName NVARCHAR(255))
BEGIN
    DECLARE userId INT;

    SELECT ID INTO userId FROM Users WHERE UserName = in_userName LIMIT 1;

    IF userId IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: User not found';
    END IF;

    SELECT userId AS ID;
END;

CREATE PROCEDURE `DeleteUser`(IN userID INT)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: Failed to delete user.';
    END;

    DELETE FROM Users WHERE ID = userID;

    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: User not found.';
    ELSE
        SELECT 'Success: User deleted.' AS Status;
    END IF;
END;

----------- Vocab Management Procedures -----------
-- Get All Vocabulary
CREATE PROCEDURE `GetAllVocabulary`(
    IN userID INT
)
BEGIN
    SELECT * FROM Vocabulary WHERE UserID = userID;
END //

-- Get Vocabulary by ID
CREATE PROCEDURE `GetVocabularyByID`(IN userID INT, IN vocabID INT)
BEGIN
    SELECT * FROM Vocabulary WHERE UserID = userID AND ID = vocabID;
END //

-- Get Unknown Vocabulary
CREATE PROCEDURE `GetUnknownVocabulary`(
    IN userID INT
)
BEGIN
    SELECT * FROM Vocabulary 
    WHERE UserID = userID AND Learnt = 0;
END //

-- Get Known Vocabulary
CREATE PROCEDURE `GetKnownVocabulary`(
    IN userID INT
)
BEGIN
    SELECT * FROM Vocabulary 
    WHERE UserID = userID AND Learnt = 1;
END //

-- Add Vocabulary
CREATE PROCEDURE `AddVocabulary`(
    IN userID INT,
    IN knownWord NVARCHAR(255),
    IN targetWord NVARCHAR(255)
)
BEGIN
    -- Handle unexpected SQL errors (e.g., constraint violation)
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: Failed to insert vocabulary. It may already exist or the input is invalid.';
    END;

    -- Validation: Empty strings
    IF TRIM(knownWord) = '' OR TRIM(targetWord) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: Both KnownLanguage-Word and TargetLanguage-Word are required.';
    END IF;

    -- Validation: Length limit
    IF LENGTH(TRIM(knownWord)) > 255 OR LENGTH(TRIM(targetWord)) > 255 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: KnownLanguage-Word or TargetLanguage-Word exceeds maximum length.';
    END IF;

    -- Insert vocabulary
    INSERT INTO Vocabulary (UserID, `KnownLanguage-Word`, `TargetLanguage-Word`)
    VALUES (userID, TRIM(knownWord), TRIM(targetWord));

    -- Return the ID of the inserted row (optional)
    SELECT LAST_INSERT_ID() AS NewVocabID;
END //

-- Delete Vocabulary
CREATE PROCEDURE `DeleteVocabulary`(IN userID INT, IN vocabID INT)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: Failed to delete vocabulary.';
    END;

    DELETE FROM Vocabulary WHERE UserID = userID AND ID = vocabID;

    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'RAISEERROR: Vocabulary item not found.';
    ELSE
        SELECT 'Success: Vocabulary item deleted.' AS Status;
    END IF;
END //