CREATE TABLE Kategooriad (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nimetus NVARCHAR(50)
);

CREATE TABLE Tooted (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Toodenimetus NVARCHAR(100),
    Kogus INT,
    Hind DECIMAL(10,2),
    Pilt NVARCHAR(255),
    Kategooria_Id INT,
    FOREIGN KEY (Kategooria_Id) REFERENCES Kategooriad(Id)
);