
-- Drop existing tables if any
DROP TABLE IF EXISTS "CategoryAttributeLinks";
DROP TABLE IF EXISTS "Products";
DROP TABLE IF EXISTS "Categories";
DROP TABLE IF EXISTS "Attributes";

-- Create Categories table
CREATE TABLE "Categories" (
    "Id" UUID PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "ParentId" UUID REFERENCES "Categories"("Id") ON DELETE SET NULL
);

-- Create Attributes table
CREATE TABLE "Attributes" (
    "Id" UUID PRIMARY KEY,
    "Name" TEXT NOT NULL
);

-- Create Products table
CREATE TABLE "Products" (
    "Id" UUID PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "CategoryId" UUID NOT NULL REFERENCES "Categories"("Id") ON DELETE CASCADE
);

-- Create CategoryAttributeLinks table
CREATE TABLE "CategoryAttributeLinks" (
    "Id" UUID PRIMARY KEY,
    "CategoryId" UUID NOT NULL REFERENCES "Categories"("Id") ON DELETE CASCADE,
    "AttributeId" UUID NOT NULL REFERENCES "Attributes"("Id") ON DELETE CASCADE,
    "LinkType" TEXT NOT NULL -- direct or inherited
);

-- Sample Categories
INSERT INTO "Categories" ("Id", "Name", "ParentId") VALUES
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Electronics', NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Home Appliances', NULL),
('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Laptops', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'),
('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Smartphones', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa');

-- Sample Attributes
INSERT INTO "Attributes" ("Id", "Name") VALUES
('11111111-1111-1111-1111-111111111111', 'Color'),
('22222222-2222-2222-2222-222222222222', 'Size'),
('33333333-3333-3333-3333-333333333333', 'Warranty');

-- Sample Products
INSERT INTO "Products" ("Id", "Name", "CategoryId") VALUES
('aaaa1111-1111-1111-1111-111111111111', 'Gaming Laptop', 'cccccccc-cccc-cccc-cccc-cccccccccccc'),
('aaaa2222-2222-2222-2222-222222222222', 'Budget Smartphone', 'dddddddd-dddd-dddd-dddd-dddddddddddd');

-- Sample CategoryAttributeLinks
INSERT INTO "CategoryAttributeLinks" ("Id", "CategoryId", "AttributeId", "LinkType") VALUES
('00011111-aaaa-bbbb-cccc-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 'direct'),
('00022222-aaaa-bbbb-cccc-222222222222', 'cccccccc-cccc-cccc-cccc-cccccccccccc', '22222222-2222-2222-2222-222222222222', 'direct'),
('00033333-aaaa-bbbb-cccc-333333333333', 'dddddddd-dddd-dddd-dddd-dddddddddddd', '33333333-3333-3333-3333-333333333333', 'inherited');
