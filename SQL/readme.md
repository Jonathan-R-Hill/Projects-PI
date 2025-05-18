# Vocabulary Management Stored Procedures

This repository contains MySQL stored procedures for managing users and vocabulary items in a language learning application. These procedures handle common operations like adding users, retrieving vocabulary lists, inserting new vocabulary, and deleting vocabulary entries with proper error handling.

---

## Stored Procedures Overview

### AddUser

Creates a new user.

```sql
CALL AddUser(firstName, lastName, userName);
```

- **Parameters:**
  - `firstName` (NVARCHAR(255)) - User's first name (required)
  - `lastName` (NVARCHAR(255)) - User's last name (required)
  - `userName` (NVARCHAR(255)) - Unique username (required, max length 255)

- **Returns:**
  - `NewUserID` - The ID of the newly created user

- **Errors:**
  - Raises error if required fields are missing or username already exists.

---

### GetUserIdByUserName

Retrieves the ID of a user given their username.

```sql
CALL GetUserIdByUserName(userName);
```

- **Parameters:**
  - `userName` (NVARCHAR(255)) - The username of the user.

- **Returns:**
  - `ID` (INT) - The ID of the user if found.

- **Errors:**
  - Raises error if the user is not found.

---

### GetAllVocabulary

Retrieves all vocabulary for a specific user.

```sql
CALL GetAllVocabulary(userID);
```

- **Parameters:**
  - `userID` (INT) - ID of the user whose vocabulary to fetch

- **Returns:**
  - All vocabulary records belonging to the user.

---

### GetVocabularyByID

Fetches a vocabulary entry by its ID for a user.

```sql
CALL GetVocabularyByID(userID, vocabID);
```

- **Parameters:**
  - `userID` (INT) - ID of the user
  - `vocabID` (INT) - ID of the vocabulary entry

- **Returns:**
  - The vocabulary record if found.

---

### GetUnknownVocabulary

Retrieves vocabulary items that are marked as *not learnt* (`Learnt = 0`).

```sql
CALL GetUnknownVocabulary(userID);
```

- **Parameters:**
  - `userID` (INT) - ID of the user

- **Returns:**
  - Vocabulary items where `Learnt` = 0.

---

### GetKnownVocabulary

Retrieves vocabulary items that are marked as *learnt* (`Learnt` = 1`).

```sql
CALL GetKnownVocabulary(userID);
```

- **Parameters:**
  - `userID` (INT) - ID of the user

- **Returns:**
  - Vocabulary items where `Learnt` = 1.

---

### AddVocabulary

Adds a new vocabulary entry for a user.

```sql
CALL AddVocabulary(userID, knownWord, targetWord);
```

- **Parameters:**
  - `userID` (INT) - ID of the user
  - `knownWord` (NVARCHAR(255)) - Word in the known language (required)
  - `targetWord` (NVARCHAR(255)) - Word in the target language (required)

- **Returns:**
  - `NewVocabID` - The ID of the newly inserted vocabulary entry.

- **Errors:**
  - Raises error if either word is missing or exceeds max length.
  - Raises error on insert failure (e.g., duplicate).

---

### DeleteVocabulary

Deletes a vocabulary entry for a user.

```sql
CALL DeleteVocabulary(userID, vocabID);
```

- **Parameters:**
  - `userID` (INT) - ID of the user
  - `vocabID` (INT) - ID of the vocabulary to delete

- **Returns:**
  - Success message on deletion (`Status: Success: Vocabulary item deleted.`).

- **Errors:**
  - Raises error if the vocabulary item does not exist or deletion fails.

---

## Usage Notes

- All procedures include error handling with descriptive messages using `SIGNAL SQLSTATE '45000'`.
- Input validation is performed to ensure required fields are present and meet length restrictions.
- Use the `CALL` statement to invoke these procedures in your MySQL client or application.
- Adjust permissions and grants as necessary for your database users to execute these procedures.

---

## Example

```sql
-- Add a new user
CALL AddUser('John', 'Doe', 'johndoe');

-- Get the ID of the new user
CALL GetUserIdByUserName('johndoe');

-- Add vocabulary for the new user (assuming userID=1)
CALL AddVocabulary(1, 'Hello', '안녕하세요');

-- Retrieve all vocabulary for user 1
CALL GetAllVocabulary(1);

-- Delete a vocabulary item (assuming vocabID=10)
CALL DeleteVocabulary(1, 10);
```