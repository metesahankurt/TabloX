ALTER DATABASE CHARACTER SET utf8mb4;


CREATE TABLE `Artists` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Bio` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ProfileImageUrl` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Country` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Artists` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `AspNetRoles` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `AspNetUsers` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `DisplayUserName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FullName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `About` longtext CHARACTER SET utf8mb4 NULL,
    `ProfileImage` longtext CHARACTER SET utf8mb4 NULL,
    `FirstName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `LastName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
    `EmailConfirmed` tinyint(1) NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
    `SecurityStamp` longtext CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumberConfirmed` tinyint(1) NOT NULL,
    `TwoFactorEnabled` tinyint(1) NOT NULL,
    `LockoutEnd` datetime(6) NULL,
    `LockoutEnabled` tinyint(1) NOT NULL,
    `AccessFailedCount` int NOT NULL,
    CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Categories` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Categories` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `GiftCards` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Code` varchar(16) CHARACTER SET utf8mb4 NOT NULL,
    `Amount` decimal(65,30) NOT NULL,
    `IsActive` tinyint(1) NOT NULL,
    `CreatedDate` datetime(6) NOT NULL,
    `UsedDate` datetime(6) NULL,
    `CreatedBy` longtext CHARACTER SET utf8mb4 NULL,
    `UsedBy` longtext CHARACTER SET utf8mb4 NULL,
    `ExpiryDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_GiftCards` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Orders` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OrderDate` datetime(6) NOT NULL,
    `OrderNumber` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ShippingAddress` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ShippingStatus` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PaymentMethod` int NOT NULL,
    `InstallmentCount` int NOT NULL,
    `MaskedCardNumber` longtext CHARACTER SET utf8mb4 NULL,
    `BankAccountNumber` longtext CHARACTER SET utf8mb4 NULL,
    `CryptoWalletAddress` longtext CHARACTER SET utf8mb4 NULL,
    `GiftCardCode` longtext CHARACTER SET utf8mb4 NULL,
    `TotalAmount` decimal(65,30) NOT NULL,
    `DiscountAmount` decimal(65,30) NOT NULL,
    `FinalAmount` decimal(65,30) NOT NULL,
    CONSTRAINT `PK_Orders` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `AspNetRoleClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `AspNetUserClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `AspNetUserLogins` (
    `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderKey` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `AspNetUserRoles` (
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `AspNetUserTokens` (
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `Value` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `Artworks` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Title` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Description` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ImageUrl` longtext CHARACTER SET utf8mb4 NOT NULL,
    `HighResImageUrl` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Price` decimal(65,30) NOT NULL,
    `CategoryId` int NOT NULL,
    `ArtistId` int NOT NULL,
    CONSTRAINT `PK_Artworks` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Artworks_Artists_ArtistId` FOREIGN KEY (`ArtistId`) REFERENCES `Artists` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Artworks_Categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `CartItems` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ArtworkId` int NOT NULL,
    `Quantity` int NOT NULL,
    CONSTRAINT `PK_CartItems` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CartItems_Artworks_ArtworkId` FOREIGN KEY (`ArtworkId`) REFERENCES `Artworks` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `OrderItems` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ArtworkId` int NOT NULL,
    `Quantity` int NOT NULL,
    `OrderId` int NOT NULL,
    CONSTRAINT `PK_OrderItems` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_OrderItems_Artworks_ArtworkId` FOREIGN KEY (`ArtworkId`) REFERENCES `Artworks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_OrderItems_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE INDEX `IX_Artworks_ArtistId` ON `Artworks` (`ArtistId`);


CREATE INDEX `IX_Artworks_CategoryId` ON `Artworks` (`CategoryId`);


CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);


CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);


CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);


CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);


CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);


CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);


CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);


CREATE INDEX `IX_CartItems_ArtworkId` ON `CartItems` (`ArtworkId`);


CREATE INDEX `IX_OrderItems_ArtworkId` ON `OrderItems` (`ArtworkId`);


CREATE INDEX `IX_OrderItems_OrderId` ON `OrderItems` (`OrderId`);


