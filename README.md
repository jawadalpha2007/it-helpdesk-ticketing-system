# IT Help Desk & Ticketing Management System

Internship project (Week 1 deliverables) — a system for employees to submit IT
issues as tickets, and for IT support agents/admins to track, assign, and resolve them.

## Project status

**Week 1 — Planning & Design (in progress)**
- [x] Requirement gathering
- [x] Database schema design
- [x] ERD diagram
- [x] UI wireframes
- [ ] GitHub repository setup (this repo)

## Repository structure

```
├── database/
│   ├── schema.sql      # Full SQL Server schema (tables, keys, seed data)
│   └── schema.dbml      # Source file for the ERD (paste into dbdiagram.io)
├── docs/
│   └── ERD.png           # Exported entity-relationship diagram
└── wireframes/
    └── WIREFRAMES.md    # Link to Figma wireframes + page descriptions
```

## Database design

The schema centers around a `Tickets` table, linked to lookup tables
(`Categories`, `Priorities`, `Statuses`) and a `Users` table with
role-based access via a `Roles` table (Admin, IT Support Agent, Employee, Manager).

See [`database/schema.sql`](database/schema.sql) for the full script and
[`docs/ERD.png`](docs/ERD.png) for the diagram.

## UI wireframes

Wireframes cover: Login, Register, Dashboard, Ticket List, Create Ticket,
Ticket Details, Notifications, Reports, Profile, and Admin Settings.

See [`wireframes/WIREFRAMES.md`](wireframes/WIREFRAMES.md) for the Figma link.

## Tools used

- **Database**: SQL Server
- **ERD**: dbdiagram.io
- **Wireframes**: Figma
- **Version control**: GitHub
