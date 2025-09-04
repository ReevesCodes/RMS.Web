CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Categories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Categories" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NULL
);

CREATE TABLE "DeliveryAddresses" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DeliveryAddresses" PRIMARY KEY AUTOINCREMENT,
    "FullName" TEXT NULL,
    "AddressLine1" TEXT NULL,
    "AddressLine2" TEXT NULL,
    "City" TEXT NULL,
    "State" TEXT NULL,
    "PostalCode" TEXT NULL,
    "Country" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "UserId" TEXT NULL,
    "OrderId" INTEGER NOT NULL
);

CREATE TABLE "PaymentMethods" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PaymentMethods" PRIMARY KEY AUTOINCREMENT,
    "CardHolderName" TEXT NULL,
    "CardNumber" TEXT NULL,
    "ExpirationMonth" TEXT NULL,
    "ExpirationYear" TEXT NULL,
    "ExpirationDate" TEXT NULL,
    "CVV" TEXT NULL,
    "BillingAddress" TEXT NULL,
    "UserId" TEXT NULL,
    "OrderId" INTEGER NOT NULL
);

CREATE TABLE "SupportMessages" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SupportMessages" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Message" TEXT NOT NULL,
    "SubmittedAt" TEXT NOT NULL
);

CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NULL,
    "Email" TEXT NULL,
    "UserName" TEXT NULL,
    "Password" TEXT NULL,
    "PasswordHash" TEXT NULL,
    "Role" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL
);

CREATE TABLE "Products" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NULL,
    "Description" TEXT NULL,
    "Price" TEXT NOT NULL,
    "Category" TEXT NULL,
    "ImageUrl" TEXT NULL,
    "Stock" INTEGER NOT NULL,
    "CategoryId" INTEGER NULL,
    CONSTRAINT "FK_Products_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id")
);

CREATE TABLE "Orders" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NULL,
    "OrderDate" TEXT NOT NULL,
    "TotalAmount" REAL NOT NULL,
    "DeliveryAddressId" INTEGER NULL,
    "DeliveryAddressId1" INTEGER NULL,
    "PaymentMethodId" INTEGER NULL,
    "PaymentMethodId1" INTEGER NULL,
    "Status" TEXT NULL,
    CONSTRAINT "FK_Orders_DeliveryAddresses_DeliveryAddressId1" FOREIGN KEY ("DeliveryAddressId1") REFERENCES "DeliveryAddresses" ("Id"),
    CONSTRAINT "FK_Orders_PaymentMethods_PaymentMethodId1" FOREIGN KEY ("PaymentMethodId1") REFERENCES "PaymentMethods" ("Id")
);

CREATE TABLE "CartItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CartItems" PRIMARY KEY AUTOINCREMENT,
    "ProductId" INTEGER NOT NULL,
    "UserId" TEXT NULL,
    "Quantity" INTEGER NOT NULL,
    "Price" TEXT NOT NULL,
    CONSTRAINT "FK_CartItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Reviews" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Reviews" PRIMARY KEY AUTOINCREMENT,
    "Rating" INTEGER NOT NULL,
    "Comment" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UserId" INTEGER NOT NULL,
    "ProductId" INTEGER NOT NULL,
    CONSTRAINT "FK_Reviews_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Reviews_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "OrderItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderItems" PRIMARY KEY AUTOINCREMENT,
    "OrderId" INTEGER NOT NULL,
    "ProductId" INTEGER NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "PriceAtPurchase" TEXT NOT NULL,
    "UnitPrice" TEXT NOT NULL,
    CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_CartItems_ProductId" ON "CartItems" ("ProductId");

CREATE INDEX "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");

CREATE INDEX "IX_OrderItems_ProductId" ON "OrderItems" ("ProductId");

CREATE INDEX "IX_Orders_DeliveryAddressId1" ON "Orders" ("DeliveryAddressId1");

CREATE INDEX "IX_Orders_PaymentMethodId1" ON "Orders" ("PaymentMethodId1");

CREATE INDEX "IX_Products_CategoryId" ON "Products" ("CategoryId");

CREATE INDEX "IX_Reviews_ProductId" ON "Reviews" ("ProductId");

CREATE INDEX "IX_Reviews_UserId" ON "Reviews" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250727100214_InitialCreate', '9.0.7');

DROP INDEX "IX_Orders_DeliveryAddressId1";

DROP INDEX "IX_Orders_PaymentMethodId1";

CREATE INDEX "IX_Orders_DeliveryAddressId" ON "Orders" ("DeliveryAddressId");

CREATE INDEX "IX_Orders_PaymentMethodId" ON "Orders" ("PaymentMethodId");

CREATE TABLE "ef_temp_Orders" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY AUTOINCREMENT,
    "DeliveryAddressId" INTEGER NULL,
    "OrderDate" TEXT NOT NULL,
    "PaymentMethodId" INTEGER NULL,
    "Status" TEXT NULL,
    "TotalAmount" REAL NOT NULL,
    "UserId" TEXT NULL,
    CONSTRAINT "FK_Orders_DeliveryAddresses_DeliveryAddressId" FOREIGN KEY ("DeliveryAddressId") REFERENCES "DeliveryAddresses" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Orders_PaymentMethods_PaymentMethodId" FOREIGN KEY ("PaymentMethodId") REFERENCES "PaymentMethods" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_Orders" ("Id", "DeliveryAddressId", "OrderDate", "PaymentMethodId", "Status", "TotalAmount", "UserId")
SELECT "Id", "DeliveryAddressId", "OrderDate", "PaymentMethodId", "Status", "TotalAmount", "UserId"
FROM "Orders";

CREATE TABLE "ef_temp_PaymentMethods" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PaymentMethods" PRIMARY KEY AUTOINCREMENT,
    "BillingAddress" TEXT NULL,
    "CVV" TEXT NULL,
    "CardHolderName" TEXT NULL,
    "CardNumber" TEXT NULL,
    "ExpirationDate" TEXT NULL,
    "ExpirationMonth" TEXT NULL,
    "ExpirationYear" TEXT NULL,
    "UserId" TEXT NULL
);

INSERT INTO "ef_temp_PaymentMethods" ("Id", "BillingAddress", "CVV", "CardHolderName", "CardNumber", "ExpirationDate", "ExpirationMonth", "ExpirationYear", "UserId")
SELECT "Id", "BillingAddress", "CVV", "CardHolderName", "CardNumber", "ExpirationDate", "ExpirationMonth", "ExpirationYear", "UserId"
FROM "PaymentMethods";

CREATE TABLE "ef_temp_DeliveryAddresses" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DeliveryAddresses" PRIMARY KEY AUTOINCREMENT,
    "AddressLine1" TEXT NULL,
    "AddressLine2" TEXT NULL,
    "City" TEXT NULL,
    "Country" TEXT NULL,
    "FullName" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PostalCode" TEXT NULL,
    "State" TEXT NULL,
    "UserId" TEXT NULL
);

INSERT INTO "ef_temp_DeliveryAddresses" ("Id", "AddressLine1", "AddressLine2", "City", "Country", "FullName", "PhoneNumber", "PostalCode", "State", "UserId")
SELECT "Id", "AddressLine1", "AddressLine2", "City", "Country", "FullName", "PhoneNumber", "PostalCode", "State", "UserId"
FROM "DeliveryAddresses";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;
DROP TABLE "Orders";

ALTER TABLE "ef_temp_Orders" RENAME TO "Orders";

DROP TABLE "PaymentMethods";

ALTER TABLE "ef_temp_PaymentMethods" RENAME TO "PaymentMethods";

DROP TABLE "DeliveryAddresses";

ALTER TABLE "ef_temp_DeliveryAddresses" RENAME TO "DeliveryAddresses";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;
CREATE INDEX "IX_Orders_DeliveryAddressId" ON "Orders" ("DeliveryAddressId");

CREATE INDEX "IX_Orders_PaymentMethodId" ON "Orders" ("PaymentMethodId");

COMMIT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250727102258_FixOrderRelationships', '9.0.7');

BEGIN TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250727160746_AddCategory', '9.0.7');

COMMIT;

