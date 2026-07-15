 -- ============================================
-- IT Help Desk & Ticketing Management System
-- Database Schema (Week 1 Deliverable)
-- Dialect: SQL Server style (IDENTITY).
-- For PostgreSQL: replace "INT IDENTITY(1,1)" with "SERIAL"
-- ============================================

-- Lookup tables first (no dependencies)

CREATE TABLE Roles (
    roleId      INT IDENTITY(1,1) PRIMARY KEY,
    roleName    VARCHAR(50) NOT NULL UNIQUE   -- Admin, IT Support Agent, Employee, Manager
);

CREATE TABLE Categories (
    categoryId      INT IDENTITY(1,1) PRIMARY KEY,
    categoryName    VARCHAR(50) NOT NULL UNIQUE -- Hardware, Software, Network, Email, Access Request, Other
);

CREATE TABLE Priorities (
    priorityId      INT IDENTITY(1,1) PRIMARY KEY,
    priorityName    VARCHAR(20) NOT NULL UNIQUE -- Low, Medium, High, Critical
);

CREATE TABLE Statuses (
    statusId      INT IDENTITY(1,1) PRIMARY KEY,
    statusName    VARCHAR(20) NOT NULL UNIQUE -- Open, In Progress, Pending, Resolved, Closed
);

-- Users depends on Roles

CREATE TABLE Users (
    userId          INT IDENTITY(1,1) PRIMARY KEY,
    roleId          INT NOT NULL,
    fullName        VARCHAR(100) NOT NULL,
    email           VARCHAR(150) NOT NULL UNIQUE,
    passwordHash    VARCHAR(255) NOT NULL,
    isActive        BIT NOT NULL DEFAULT 1,
    createdAt       DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FKUsersRoles FOREIGN KEY (roleId) REFERENCES Roles(roleId)
);

-- Tickets depends on Users, Categories, Priorities, Statuses

CREATE TABLE Tickets (
    ticketId            INT IDENTITY(1,1) PRIMARY KEY,
    ticketReference     VARCHAR(20) NOT NULL UNIQUE,     -- e.g. "TCK-2026-00042"
    title                VARCHAR(150) NOT NULL,
    description          VARCHAR(MAX) NOT NULL,
    categoryId           INT NOT NULL,
    priorityId           INT NOT NULL,
    statusId             INT NOT NULL,
    createdBy            INT NOT NULL,                    -- Employee who submitted it
    assignedTo           INT NULL,                         -- Support agent (nullable until assigned)
    createdAt            DATETIME NOT NULL DEFAULT GETDATE(),
    updatedAt            DATETIME NULL,
    resolvedAt           DATETIME NULL,
    CONSTRAINT FKTicketsCategories FOREIGN KEY (categoryId) REFERENCES Categories(categoryId),
    CONSTRAINT FKTicketsPriorities FOREIGN KEY (priorityId) REFERENCES Priorities(priorityId),
    CONSTRAINT FKTicketsStatuses FOREIGN KEY (statusId) REFERENCES Statuses(statusId),
    CONSTRAINT FKTicketsCreatedBy FOREIGN KEY (createdBy) REFERENCES Users(userId),
    CONSTRAINT FKTicketsAssignedTo FOREIGN KEY (assignedTo) REFERENCES Users(userId)
);

-- TicketComments depends on Tickets, Users

CREATE TABLE TicketComments (
    commentId       INT IDENTITY(1,1) PRIMARY KEY,
    ticketId        INT NOT NULL,
    userId          INT NOT NULL,
    commentText     VARCHAR(MAX) NOT NULL,
    isInternal      BIT NOT NULL DEFAULT 0,   -- internal note (agents only) vs visible to employee
    createdAt       DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FKCommentsTickets FOREIGN KEY (ticketId) REFERENCES Tickets(ticketId),
    CONSTRAINT FKCommentsUsers FOREIGN KEY (userId) REFERENCES Users(userId)
);

-- TicketAttachments depends on Tickets, Users

CREATE TABLE TicketAttachments (
    attachmentId    INT IDENTITY(1,1) PRIMARY KEY,
    ticketId        INT NOT NULL,
    uploadedBy      INT NOT NULL,
    fileName        VARCHAR(255) NOT NULL,
    filePath        VARCHAR(500) NOT NULL,
    fileSize        INT NOT NULL,        -- in bytes
    fileType        VARCHAR(50) NOT NULL,
    uploadedAt      DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FKAttachmentsTickets FOREIGN KEY (ticketId) REFERENCES Tickets(ticketId),
    CONSTRAINT FKAttachmentsUsers FOREIGN KEY (uploadedBy) REFERENCES Users(userId)
);

-- Notifications depends on Users, Tickets

CREATE TABLE Notifications (
    notificationId    INT IDENTITY(1,1) PRIMARY KEY,
    userId            INT NOT NULL,
    ticketId          INT NULL,          -- some notifications may not relate to a ticket
    message           VARCHAR(255) NOT NULL,
    isRead            BIT NOT NULL DEFAULT 0,
    createdAt         DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FKNotificationsUsers FOREIGN KEY (userId) REFERENCES Users(userId),
    CONSTRAINT FKNotificationsTickets FOREIGN KEY (ticketId) REFERENCES Tickets(ticketId)
);

-- ActivityLogs depends on Users

CREATE TABLE ActivityLogs (
    logId         INT IDENTITY(1,1) PRIMARY KEY,
    userId        INT NOT NULL,
    action        VARCHAR(100) NOT NULL,     -- e.g. "Ticket Created", "Status Changed"
    entityType    VARCHAR(50) NOT NULL,      -- e.g. "Ticket", "User"
    entityId      INT NOT NULL,              -- id of the affected record
    createdAt     DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FKLogsUsers FOREIGN KEY (userId) REFERENCES Users(userId)
);

-- ============================================
-- Seed data (lookup values from the project spec)
-- ============================================

INSERT INTO Roles (roleName) VALUES ('Admin'), ('IT Support Agent'), ('Employee'), ('Manager');

INSERT INTO Categories (categoryName) VALUES
    ('Hardware'), ('Software'), ('Network'), ('Email'), ('Access Request'), ('Other');

INSERT INTO Priorities (priorityName) VALUES ('Low'), ('Medium'), ('High'), ('Critical');

INSERT INTO Statuses (statusName) VALUES
    ('Open'), ('In Progress'), ('Pending'), ('Resolved'), ('Closed');
