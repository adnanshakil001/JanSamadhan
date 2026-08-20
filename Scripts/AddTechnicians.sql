-- Script to add sample technicians
-- Run this in SQL Server Management Studio or through Entity Framework

-- First, make sure you have departments
INSERT INTO Departments (Name, Email, Description, IsActive) 
VALUES 
('Public Works', 'publicworks@city.gov', 'Handles infrastructure issues', 1),
('Traffic Management', 'traffic@city.gov', 'Manages traffic signals and road safety', 1),
('Utilities', 'utilities@city.gov', 'Water, sewer, and electrical issues', 1)
ON DUPLICATE KEY UPDATE Name = Name;

-- Add sample technicians
-- Note: You'll need to hash the passwords using your PasswordService
-- For now, these are placeholder hashes - you should use the actual registration process

INSERT INTO Users (Name, Email, PasswordHash, Points, BadgeLevel, Role, DepartmentId) 
VALUES 
('John Technician', 'john.tech@city.gov', 'hashed_password_here', 0, 'Bronze', 'Technician', 1),
('Sarah Worker', 'sarah.worker@city.gov', 'hashed_password_here', 0, 'Bronze', 'Technician', 1),
('Mike Engineer', 'mike.engineer@city.gov', 'hashed_password_here', 0, 'Bronze', 'Technician', 2)
ON DUPLICATE KEY UPDATE Name = Name;
