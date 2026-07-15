-- ============================================
-- IT Help Desk & Ticketing Management System
-- Database Schema (Week 1 Deliverable)
-- Dialect: SQL Server style (IDENTITY). 
-- For PostgreSQL: replace "INT IDENTITY(1,1)" with "SERIAL"
-- ============================================

-- Lookup tables first (no dependencies)

CREATE TABLE Roles (
    role_id     INT IDENTITY(1,1) PRIMARY KEY,
    role_name   VARCHAR(50) NOT NULL UNIQUE   -- Admin, IT Support Agent, Employee, Manager
);

CREATE TABLE Categories (
    category_id     INT IDENTITY(1,1) PRIMARY KEY,
    category_name   VARCHAR(50) NOT NULL UNIQUE -- Hardware, Software, Network, Email, Access Request, Other
);

CREATE TABLE Priorities (
    priority_id     INT IDENTITY(1,1) PRIMARY KEY,
    priority_name   VARCHAR(20) NOT NULL UNIQUE -- Low, Medium, High, Critical
);

CREATE TABLE Statuses (
    status_id     INT IDENTITY(1,1) PRIMARY KEY,
    status_name   VARCHAR(20) NOT NULL UNIQUE -- Open, In Progress, Pending, Resolved, Closed
);

-- Users depends on Roles

CREATE TABLE Users (
    user_id         INT IDENTITY(1,1) PRIMARY KEY,
    role_id         INT NOT NULL,
    full_name       VARCHAR(100) NOT NULL,
    email           VARCHAR(150) NOT NULL UNIQUE,
    password_hash   VARCHAR(255) NOT NULL,
    is_active       BIT NOT NULL DEFAULT 1,
    created_at      DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (role_id) REFERENCES Roles(role_id)
);

-- Tickets depends on Users, Categories, Priorities, Statuses

CREATE TABLE Tickets (
    ticket_id           INT IDENTITY(1,1) PRIMARY KEY,
    ticket_reference    VARCHAR(20) NOT NULL UNIQUE,     -- e.g. "TCK-2026-00042"
    title               VARCHAR(150) NOT NULL,
    description         VARCHAR(MAX) NOT NULL,
    category_id         INT NOT NULL,
    priority_id         INT NOT NULL,
    status_id           INT NOT NULL,
    created_by          INT NOT NULL,                    -- Employee who submitted it
    assigned_to         INT NULL,                         -- Support agent (nullable until assigned)
    created_at          DATETIME NOT NULL DEFAULT GETDATE(),
    updated_at          DATETIME NULL,
    resolved_at         DATETIME NULL,
    CONSTRAINT FK_Tickets_Categories FOREIGN KEY (category_id) REFERENCES Categories(category_id),
    CONSTRAINT FK_Tickets_Priorities FOREIGN KEY (priority_id) REFERENCES Priorities(priority_id),
    CONSTRAINT FK_Tickets_Statuses FOREIGN KEY (status_id) REFERENCES Statuses(status_id),
    CONSTRAINT FK_Tickets_CreatedBy FOREIGN KEY (created_by) REFERENCES Users(user_id),
    CONSTRAINT FK_Tickets_AssignedTo FOREIGN KEY (assigned_to) REFERENCES Users(user_id)
);

-- TicketComments depends on Tickets, Users

CREATE TABLE TicketComments (
    comment_id      INT IDENTITY(1,1) PRIMARY KEY,
    ticket_id       INT NOT NULL,
    user_id         INT NOT NULL,
    comment_text    VARCHAR(MAX) NOT NULL,
    is_internal     BIT NOT NULL DEFAULT 0,   -- internal note (agents only) vs visible to employee
    created_at      DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Comments_Tickets FOREIGN KEY (ticket_id) REFERENCES Tickets(ticket_id),
    CONSTRAINT FK_Comments_Users FOREIGN KEY (user_id) REFERENCES Users(user_id)
);

-- TicketAttachments depends on Tickets, Users

CREATE TABLE TicketAttachments (
    attachment_id   INT IDENTITY(1,1) PRIMARY KEY,
    ticket_id       INT NOT NULL,
    uploaded_by     INT NOT NULL,
    file_name       VARCHAR(255) NOT NULL,
    file_path       VARCHAR(500) NOT NULL,
    file_size       INT NOT NULL,        -- in bytes
    file_type       VARCHAR(50) NOT NULL,
    uploaded_at     DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Attachments_Tickets FOREIGN KEY (ticket_id) REFERENCES Tickets(ticket_id),
    CONSTRAINT FK_Attachments_Users FOREIGN KEY (uploaded_by) REFERENCES Users(user_id)
);

-- Notifications depends on Users, Tickets

CREATE TABLE Notifications (
    notification_id   INT IDENTITY(1,1) PRIMARY KEY,
    user_id            INT NOT NULL,
    ticket_id          INT NULL,          -- some notifications may not relate to a ticket
    message            VARCHAR(255) NOT NULL,
    is_read            BIT NOT NULL DEFAULT 0,
    created_at         DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (user_id) REFERENCES Users(user_id),
    CONSTRAINT FK_Notifications_Tickets FOREIGN KEY (ticket_id) REFERENCES Tickets(ticket_id)
);

-- ActivityLogs depends on Users

CREATE TABLE ActivityLogs (
    log_id        INT IDENTITY(1,1) PRIMARY KEY,
    user_id       INT NOT NULL,
    action        VARCHAR(100) NOT NULL,     -- e.g. "Ticket Created", "Status Changed"
    entity_type   VARCHAR(50) NOT NULL,      -- e.g. "Ticket", "User"
    entity_id     INT NOT NULL,              -- id of the affected record
    created_at    DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Logs_Users FOREIGN KEY (user_id) REFERENCES Users(user_id)
);

-- ============================================
-- Seed data (lookup values from the project spec)
-- ============================================

INSERT INTO Roles (role_name) VALUES ('Admin'), ('IT Support Agent'), ('Employee'), ('Manager');

INSERT INTO Categories (category_name) VALUES
    ('Hardware'), ('Software'), ('Network'), ('Email'), ('Access Request'), ('Other');

INSERT INTO Priorities (priority_name) VALUES ('Low'), ('Medium'), ('High'), ('Critical');

INSERT INTO Statuses (status_name) VALUES
    ('Open'), ('In Progress'), ('Pending'), ('Resolved'), ('Closed');
