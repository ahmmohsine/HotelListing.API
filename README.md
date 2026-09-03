# HotelListing API

## Project Overview

**HotelListing API** is a production-ready RESTful Web API built with .NET 8, designed to manage hotels and countries data. The project is architected following Clean Architecture principles, leveraging a decoupled Repository + Service pattern to enforce strict separation of concerns, maintainability, and testability.

### Key Features
* **Layered Architecture:** Clear boundary between Data Access (Repository), Business Logic (Service), and Transport (DTOs/Controllers).
* **DTO Separation & Declarative Validation:** Database entities are strictly segregated from API contracts using data transfer objects and DataAnnotations.
* **Asynchronous I/O:** Fully asynchronous operations utilizing `async/await` and `CancellationToken` support throughout the pipeline.
* **RESTful Endpoints:** Complete CRUD operations returning standard HTTP status codes (`200 OK`, `201 Created`, `204 NoContent`, `404 NotFound`).
* **Entity Framework Core:** EF Core with SQL Server integration, utilizing `AsNoTracking()` for optimized read performance.
* **In-Memory & Database Repositories:** Interchangeable repository implementations for testing and database persistence.

# Git Workflow & Commit Standards

This guide outlines the essential Git commands, commit conventions, and best practices for maintaining a clean and traceable project history.

---

## 1. Daily Commands

| Action | Git Command | Description |
| :--- | :--- | :--- |
| **Check Status** | `git status` | Displays modified, staged, or untracked files. |
| **Stage Changes** | `git add .` | Adds all local modifications to the Staging Area. |
| **Create Commit** | `git commit -m "type: description"` | Saves staged changes to the local repository history. |
| **Push Changes** | `git push origin main` | Uploads local commits to the remote repository (GitHub). |
| **Pull Updates** | `git pull origin main` | Fetches and merges updates from the remote repository. |

---

## 2. Commit Message Conventions (Conventional Commits)

Format: `type: concise description in imperative mood`

### Main Commit Types:
* **`feat:`** A new feature (e.g., `feat: add HotelService and CRUD endpoints`)
* **`fix:`** A bug fix (e.g., `fix: correct return type in DeleteAsync`)
* **`refactor:`** Code changes that neither fix a bug nor add a feature (e.g., `refactor: isolate validation logic in service layer`)
* **`docs:`** Documentation changes (e.g., `docs: update Git README`)
* **`style:`** Formatting, white-space, or missing semi-colons (no code logic change)
* **`test:`** Adding or updating unit/integration tests
* **`chore:`** Maintenance tasks, dependency updates (NuGet/NPM)

---

## 3. Branching Workflow

Keep the `main` branch stable by developing features in isolated branches:

```bash
# 1. Create and switch to a feature branch
git checkout -b feat/hotel-repository

# 2. Stage and commit your changes
git add .
git commit -m "feat: implement HotelRepository using EF Core"

# 3. Switch back to main
git checkout main

# 4. Merge the feature branch
git merge feat/hotel-repository

# 5. Delete the local feature branch
git branch -d feat/hotel-repository