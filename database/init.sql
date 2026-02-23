CREATE DATABASE IF NOT EXISTS lumen_db;
USE lumen_db;

CREATE TABLE IF NOT EXISTS Users (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Email VARCHAR(255) NOT NULL,
  Name VARCHAR(255) NOT NULL,
  PasswordHash VARCHAR(255) NOT NULL,
  AvatarUrl VARCHAR(512) NULL,
  Balance DECIMAL(18,2) NOT NULL DEFAULT 1000.00,
  Experience INT NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS Payments (
    Id INT AUTO_INCREMENT PRIMARY KEY,

    Service VARCHAR(255) NOT NULL,
    Identifier VARCHAR(255) NOT NULL,

    Amount DECIMAL(18,2) NOT NULL,

    Status INT NOT NULL DEFAULT 0,

    CreatedAt DATETIME(6) NOT NULL,

    UserId INT NOT NULL,

    CONSTRAINT FK_Payments_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Templates (
    Id INT AUTO_INCREMENT PRIMARY KEY,

    Name VARCHAR(255) NOT NULL,
    Service VARCHAR(255) NOT NULL,
    Type VARCHAR(255) NOT NULL,
    Value VARCHAR(255) NOT NULL,

    UserId INT NOT NULL,

    CONSTRAINT FK_Templates_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS RepairsContent (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    
    CompletedTopicsJson TEXT NOT NULL,
    MaintenanceStateJson TEXT NOT NULL,
    EmergencyFormJson TEXT NOT NULL,

    CONSTRAINT FK_RepairsContent_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
        ON DELETE CASCADE,
        
    UNIQUE KEY UX_RepairsContent_UserId (UserId)
);
-- Optional (recommended): ensure emails are unique
-- CREATE UNIQUE INDEX idx_users_email ON Users (Email);


CREATE TABLE IF NOT EXISTS BudgetContent (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,

    CompletedSimulationsJson TEXT NOT NULL,

    CONSTRAINT FK_BudgetContent_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
        ON DELETE CASCADE,

    UNIQUE KEY UX_BudgetContent_UserId (UserId)
);