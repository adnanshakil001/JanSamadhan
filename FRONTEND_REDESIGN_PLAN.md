# JanSamadhan — Modular Frontend UI/UX Redesign Blueprint (Bite-Sized Phases)

> **Architecture Status**: Core ASP.NET Core 8.0 MVC architecture, EF Core models, 10 controllers, authentication pipelines, CSRF protection, SQLite database, and Docker container are **100% intact and functional** (Build Status: 0 Errors, 0 Warnings).
> 
> **How to use this plan**:
> Each phase below is intentionally **small, atomic, and touches only 1–2 files**. You can run them one by one in OpenCode (`opencode`) or in this chat window without risking token limits or incomplete edits.

---

## Phase Breakdown Overview

| Phase # | Phase Title | Files Modified / Created | Target Skills |
|:---:|---|---|---|
| **Phase 1** | Design Tokens & Global CSS Variables | `wwwroot/css/tokens.css`, `wwwroot/css/site.css` | `impeccable`, `wcag-audit-patterns` |
| **Phase 2** | Master Shell & Navigation Partials | `_Layout.cshtml`, `_SidebarPartial.cshtml`, `_TopNavPartial.cshtml` | `impeccable`, `mobile-native` |
| **Phase 3** | Public Landing Page | `Views/Home/Index.cshtml` | `impeccable`, `emil-design-eng`, `animate` |
| **Phase 4** | Unified Authentication Screens | `Views/Account/Login.cshtml`, `Register`, `AdminLogin`, `TechnicianLogin` | `impeccable`, `frontend-security-coder` |
| **Phase 5** | Citizen Complaint Reporting Flow | `Views/Complaint/Create.cshtml` | `impeccable`, `mobile-native`, `emil-design-eng` |
| **Phase 6** | Citizen Dashboard & Grievance List | `Views/Complaint/Index.cshtml` | `impeccable`, `emil-design-eng` |
| **Phase 7** | Complaint Details & Resolution Stepper | `Views/Complaint/Details.cshtml` | `impeccable`, `emil-design-eng`, `animate` |
| **Phase 8** | Citizen Badges & Nearby Map | `Views/Badge/Index.cshtml`, `Views/Home/Nearby.cshtml` | `impeccable`, `animate` |
| **Phase 9** | Admin Dashboard & Live Density Map | `Views/Admin/Index.cshtml` | `impeccable`, `animation-performance` |
| **Phase 10** | Admin Complaint Review & Assignment | `Views/Admin/Details.cshtml` | `impeccable`, `break-ui` |
| **Phase 11** | Admin Roles & Technician Onboarding | `Views/Role/Index.cshtml`, `Views/Admin/CreateTechnician.cshtml` | `impeccable`, `wcag-audit-patterns` |
| **Phase 12** | Leaderboards & Gamified Scoreboards | `Views/Admin/Scoreboard.cshtml`, `TechnicianScoreboard.cshtml`, `Home/Scoreboard.cshtml` | `impeccable`, `emil-design-eng` |
| **Phase 13** | Fullscreen GIS Operations Map | `Views/Map/AdminMap.cshtml` | `impeccable`, `animation-performance` |
| **Phase 14** | Technician Dashboard & Work Pool | `Views/Technician/Dashboard.cshtml`, `AvailableWork.cshtml` | `mobile-native`, `emil-design-eng` |
| **Phase 15** | Technician Proof-of-Work Resolution | `Views/Technician/Details.cshtml` | `mobile-native`, `emil-design-eng` |
| **Phase 16** | Micro-interactions, Toasts & A11y Audit | `wwwroot/js/site.js`, `_Layout.cshtml`, CSS polish | `wcag-audit-patterns`, `break-ui`, `animate` |

---

## Phase 1: Design Tokens & Global CSS Variables

### Changes to Be Made:
1. Create `wwwroot/css/tokens.css` with CSS Custom Properties:
   - Primary Civic Blue scale (`--color-primary-50` to `900`).
   - Semantic Status tokens with high-contrast text (`--status-pending`, `--status-inprogress`, `--status-fixed`).
   - Neutral Slate scale (`--slate-50` to `950`).
   - Spacing tokens (4px, 8px, 12px, 16px, 24px, 32px), radii (`--radius-sm` through `--radius-pill`), and shadows (`--shadow-sm` through `--shadow-xl`).
2. Update `wwwroot/css/site.css` to import and utilize `tokens.css`.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `wcag-audit-patterns`

TASK: Create the foundational design token system for JanSamadhan in pure CSS.
1. Create `wwwroot/css/tokens.css` defining CSS custom properties:
   - Primary palette: `--color-primary: #2563eb`, `--color-primary-hover: #1d4ed8`, `--color-primary-light: #eff6ff`.
   - Semantic statuses: Pending (Crimson `#ef4444` on `#fef2f2`), In Progress (Amber `#d97706` on `#fffbeb`), Fixed (Emerald `#10b981` on `#ecfdf5`).
   - Slate neutral scale: 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 950.
   - Elevation shadows, border radii (4px, 8px, 12px, 16px, 9999px), and transition timings (`--transition-fast: 150ms cubic-bezier(0.4, 0, 0.2, 1)`).
   - Ensure all color pairings meet WCAG 2.2 AA contrast standards (minimum 4.5:1).
2. Update `wwwroot/css/site.css` to import `tokens.css` and define base body styles and badge utility classes.
Verify with `dotnet build`.
```

---

## Phase 2: Master Shell & Navigation Partials

### Changes to Be Made:
1. Create `Views/Shared/_SidebarPartial.cshtml`:
   - Single dynamic sidebar checking `User.IsInRole()` to render links for Admin, Technician, or Citizen.
   - Includes mobile off-canvas drawer wrapper so sidebars collapse on mobile.
2. Create `Views/Shared/_TopNavPartial.cshtml`:
   - Universal header with breadcrumbs, user avatar pill dropdown, and mobile hamburger button.
3. Refactor `Views/Shared/_Layout.cshtml`:
   - Replace the legacy static navbar with `_SidebarPartial` and `_TopNavPartial`.
   - Preserve `@Antiforgery.GetAndStoreTokens(Context)` and CSRF headers.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `mobile-native`

TASK: Build a responsive dashboard shell with reusable Razor partials.
1. Create `Views/Shared/_SidebarPartial.cshtml`:
   - Reads `User.IsInRole()`:
     - Admin: Dashboard, Map View, Role Management, Technician Board, User Scoreboard.
     - Technician: Assigned Tasks, Available Work Pool, Scoreboard.
     - Citizen: My Complaints, Report Issue, Nearby Issues, Badges, Leaderboard.
   - Include active route highlighting and a mobile off-canvas drawer structure for screens < 992px.
2. Create `Views/Shared/_TopNavPartial.cshtml`:
   - Mobile hamburger toggle button, current page title, user profile avatar pill, and logout shortcut.
3. Refactor `Views/Shared/_Layout.cshtml`:
   - Link `~/css/tokens.css`.
   - Assemble the shell: left sidebar, top header, `@RenderBody()` main content area.
   - Retain CSRF meta tag and jQuery AJAX setup intact.
Verify with `dotnet build`.
```

---

## Phase 3: Public Landing Page

### Changes to Be Made:
1. Revamp `Views/Home/Index.cshtml`:
   - Hero section: clean typography, radial gradient backdrop, status pill, primary/secondary CTAs with micro-hover physics.
   - Stakeholder cards (Citizens, Municipal Staff, Technicians) with modern icon badges and hover elevations.
   - "How It Works" 3-step timeline.
   - Sticky top navigation with backdrop blur (`backdrop-filter: blur(8px)`).

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `emil-design-eng`, `animate`

TASK: Redesign `Views/Home/Index.cshtml` into a modern civic tech landing page.
1. Refactor the hero section:
   - Use clean typography (`letter-spacing: -0.03em`), pill badge ("Civic Redressal Platform"), and dual CTA buttons ("Report a Problem" and "Platform Overview").
   - Add a subtle background grid or radial light gradient.
2. Redesign stakeholder cards and 3-step timeline using CSS variables from `tokens.css`.
3. Add hover lifts (`transform: translateY(-2px)`) with smooth transitions on interactive cards.
4. Ensure sticky header has a frosted glass effect (`backdrop-filter: blur(8px); background: rgba(255,255,255,0.85)`).
Verify with `dotnet build`.
```

---

## Phase 4: Unified Authentication Screens

### Changes to Be Made:
1. Redesign `Views/Account/Login.cshtml`, `Register.cshtml`, `AdminLogin.cshtml`, and `TechnicianLogin.cshtml`:
   - Centered card container with subtle shadow and border.
   - Role switcher tabs at top ("Citizen", "Technician", "Admin") to allow easy portal switching.
   - Floating label form inputs, password reveal toggle button (eye icon), and styled validation error alerts.
   - Ensure `@Html.AntiForgeryToken()` remains in all forms.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `frontend-security-coder`

TASK: Modernize all authentication views in `Views/Account/` (`Login.cshtml`, `Register.cshtml`, `AdminLogin.cshtml`, `TechnicianLogin.cshtml`).
1. Create a clean, centered card layout across all 4 auth pages using `tokens.css`.
2. Add a segmented tab bar at the top of the auth card letting users switch between Citizen, Technician, and Admin logins.
3. Enhance input fields with clean borders, focus rings (`:focus-visible`), and a password show/hide toggle.
4. Keep all form action URLs, field names, and `@Html.AntiForgeryToken()` intact.
Verify with `dotnet build`.
```

---

## Phase 5: Citizen Complaint Reporting Flow

### Changes to Be Made:
1. Redesign `Views/Complaint/Create.cshtml`:
   - 2-column layout: Form fields on left, Leaflet map on right.
   - Interactive Leaflet map: Custom SVG location marker pin, reverse geocoding loading spinner, "Use My Location" button.
   - Drag-and-drop photo upload dropzone with client-side image thumbnail preview.
   - Auto-updating department badge when problem type is selected.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `mobile-native`, `emil-design-eng`

TASK: Modernize `Views/Complaint/Create.cshtml`.
1. Structure into a responsive 2-column layout (stacks to 1 column on mobile):
   - Left: Problem category selector with department preview badge, address input, description, and drag-and-drop file upload zone with live image preview.
   - Right: Sticky Leaflet map picker card with a custom SVG marker pin, address geocoding spinner, and "Current Location" button.
2. Ensure touch targets on mobile are at least 44x44px.
3. Keep all model bindings (`problemType`, `description`, `latitude`, `longitude`, `address`, `photo`) and form submission intact.
Verify with `dotnet build`.
```

---

## Phase 6: Citizen Dashboard & Grievance List

### Changes to Be Made:
1. Redesign `Views/Complaint/Index.cshtml`:
   - 3 KPI stat cards at top (Total Filed, In Progress, Resolved) with soft tinted backgrounds and icons.
   - Filter bar: live search box, category dropdown, and status pills (All, Pending, In Progress, Fixed).
   - Modern grievance list: Responsive cards on mobile, clean table on desktop with status pill badges.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `emil-design-eng`

TASK: Redesign `Views/Complaint/Index.cshtml` (Citizen Dashboard).
1. Add 3 KPI metric cards at the top for quick complaint stats.
2. Add a status tab filter bar (All, Pending, In Progress, Fixed) and a client-side search box.
3. Display complaints as modern cards on mobile screens and a clean striped table on desktop.
4. Replace raw status text with high-contrast semantic badges using tokens from `tokens.css`.
5. Remove redundant inline sidebar HTML and use the layout partials from Phase 2.
Verify with `dotnet build`.
```

---

## Phase 7: Complaint Details & Resolution Stepper

### Changes to Be Made:
1. Redesign `Views/Complaint/Details.cshtml`:
   - 4-stage visual progress stepper (Submitted → Department Assigned → In Progress → Resolved) with completed checkmarks.
   - Chat-style comment thread with distinct avatars and role badges (Citizen vs Admin vs Technician).
   - Citizen 5-star rating component with interactive SVG stars and hover color animations.
   - Photo attachment gallery with thumbnail preview.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `emil-design-eng`, `animate`

TASK: Redesign `Views/Complaint/Details.cshtml`.
1. Add a 4-step horizontal progress tracker at the top showing the complaint's lifecycle stage.
2. Redesign the comment section into a modern chat-style timeline with timestamps, role badges, and image attachment previews.
3. Polish the star rating component: use smooth SVG star icons with interactive hover micro-animations.
4. Clean up inline styling and leverage `tokens.css`. Keep comment posting and rating submission forms intact.
Verify with `dotnet build`.
```

---

## Phase 8: Citizen Badges & Nearby Map

### Changes to Be Made:
1. Redesign `Views/Badge/Index.cshtml`:
   - Gamification dashboard with user level card, XP/points progress bar, and unlocked/locked badge cards.
2. Redesign `Views/Home/Nearby.cshtml`:
   - Full-width interactive Leaflet map showing nearby issues with custom color-coded markers and popup preview cards.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `animate`

TASK: Redesign `Views/Badge/Index.cshtml` and `Views/Home/Nearby.cshtml`.
1. In `Badge/Index.cshtml`: Create gamified badge reward cards with milestone progress bars and unlocked status badge glows.
2. In `Home/Nearby.cshtml`: Overhaul the map interface with category filter pills and interactive popup cards for nearby issues.
3. Remove redundant inline sidebar markup and connect to `tokens.css`.
Verify with `dotnet build`.
```

---

## Phase 9: Admin Dashboard & Live Density Map

### Changes to Be Made:
1. Redesign `Views/Admin/Index.cshtml`:
   - Top KPI metric row (Total Grievances, Resolution Rate %, Pending Bottlenecks, Active Technicians).
   - Live Density Leaflet Map card with status markers (Pending=Red, InProgress=Amber, Fixed=Emerald).
   - Master complaints data table with search, department filter, and status badges.
   - Department workload progress bars and top technician mini-leaderboard.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `animation-performance`

TASK: Redesign the Admin Command Center in `Views/Admin/Index.cshtml`.
1. Create a 4-metric KPI row with subtle icons, large numbers, and percentage badges.
2. Modernize the embedded Leaflet density map card with a status legend and circle markers.
3. Modernize the complaints master table: clean rows, hover highlights, status pills, and action dropdowns.
4. Add department workload bars and top technician cards.
5. Remove duplicate inline `<aside>` markup and duplicate `<style>` tags.
Verify with `dotnet build`.
```

---

## Phase 10: Admin Complaint Review & Assignment

### Changes to Be Made:
1. Redesign `Views/Admin/Details.cshtml`:
   - 2-column layout: Left column = Complaint evidence, citizen details, location map, photos; Right column = Department re-routing, technician assignment dropdown, and priority controls.
   - Activity log / audit trail card showing updates.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `break-ui`

TASK: Redesign `Views/Admin/Details.cshtml`.
1. Structure into a clean 2-column layout:
   - Left: Grievance info, high-resolution photo gallery, map location, citizen contact card.
   - Right: Admin action panel (department assignment dropdown, technician selector, status override).
2. Format the audit history timeline card.
3. Test layout against long addresses and descriptions using `break-ui` principles.
Verify with `dotnet build`.
```

---

## Phase 11: Admin Roles & Technician Onboarding

### Changes to Be Made:
1. Redesign `Views/Role/Index.cshtml`:
   - Modern user management table with role badges and accessible role change action modal/dropdown.
2. Redesign `Views/Admin/CreateTechnician.cshtml`:
   - Clean onboarding form with department picker cards, name, email, password fields, and validation feedback.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `wcag-audit-patterns`

TASK: Redesign `Views/Role/Index.cshtml` and `Views/Admin/CreateTechnician.cshtml`.
1. In `Role/Index.cshtml`: Modernize user table with search box, role filter, and role change controls.
2. In `Admin/CreateTechnician.cshtml`: Modernize the technician registration form with card-based department selection and styled inputs.
3. Ensure keyboard accessibility and proper focus states.
Verify with `dotnet build`.
```

---

## Phase 12: Leaderboards & Gamified Scoreboards

### Changes to Be Made:
1. Redesign `Views/Admin/Scoreboard.cshtml`, `Views/Admin/TechnicianScoreboard.cshtml`, and `Views/Home/Scoreboard.cshtml`:
   - Top 3 Podium Cards (Gold, Silver, Bronze) with avatar initials and score badges.
   - Ranked list table for ranks 4+ with points and resolution counters.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `emil-design-eng`

TASK: Redesign the scoreboard views (`Views/Admin/Scoreboard.cshtml`, `TechnicianScoreboard.cshtml`, `Views/Home/Scoreboard.cshtml`).
1. Create a top-3 podium visual component (1st, 2nd, 3rd) with trophy icons, user names, and highlighted point totals.
2. Create a clean ranking table for remaining participants with rank number badges.
3. Clean up inline styling and connect to `tokens.css`.
Verify with `dotnet build`.
```

---

## Phase 13: Fullscreen GIS Operations Map

### Changes to Be Made:
1. Redesign `Views/Map/AdminMap.cshtml`:
   - Fullscreen GIS map canvas with floating filter controls (Department, Status, Timeframe).
   - Marker clustering and slide-out complaint detail drawer when a marker is clicked.

### Agent Prompt:
```markdown
Skills to apply: `impeccable`, `animation-performance`

TASK: Redesign `Views/Map/AdminMap.cshtml` into a fullscreen GIS command center.
1. Create a full-height map layout with floating card controls for status and department filtering.
2. Enhance Leaflet circle markers with tooltips and a slide-in off-canvas card when a marker is clicked.
3. Ensure map tile loading is smooth with zero layout thrashing.
Verify with `dotnet build`.
```

---

## Phase 14: Technician Dashboard & Work Pool

### Changes to Be Made:
1. Redesign `Views/Technician/Dashboard.cshtml` and `Views/Technician/AvailableWork.cshtml`:
   - Mobile-first cards designed for smartphones (minimum 44x44px touch targets).
   - Quick stat chips: Active Tasks, Completed Today, Points Earned.
   - Task card with 1-tap "Navigate" button (linking directly to Google Maps navigation with lat/lng coordinates).
   - "Claim Job" button on available work pool with confirmation modal.

### Agent Prompt:
```markdown
Skills to apply: `mobile-native`, `emil-design-eng`

TASK: Modernize `Views/Technician/Dashboard.cshtml` and `Views/Technician/AvailableWork.cshtml` for mobile field operations.
1. Design touch-friendly cards for assigned complaints: priority badge, address, elapsed time, and a 1-tap "Navigate" link (`https://www.google.com/maps/search/?api=1&query={lat},{lng}`).
2. In `AvailableWork.cshtml`: Add a clean job pool card list with a 1-tap "Claim Task" action button.
3. Ensure all interactive buttons meet mobile touch target standards (44x44px min).
Verify with `dotnet build`.
```

---

## Phase 15: Technician Proof-of-Work Resolution

### Changes to Be Made:
1. Redesign `Views/Technician/Details.cshtml`:
   - Mobile-optimized photo uploader supporting direct camera capture (`accept="image/*"`).
   - Side-by-side comparison gallery: Citizen's problem photo vs Technician's resolution photo.
   - One-tap status update buttons ("Mark In Progress", "Complete & Upload Proof").

### Agent Prompt:
```markdown
Skills to apply: `mobile-native`, `emil-design-eng`

TASK: Redesign `Views/Technician/Details.cshtml`.
1. Create a prominent proof-of-work upload card with direct camera capture support on mobile.
2. Display before/after photo comparison cards with zoom/lightbox capability.
3. Provide single-tap resolution buttons with loading state feedback.
4. Keep technician photo upload POST action and status change logic intact.
Verify with `dotnet build`.
```

---

## Phase 16: Micro-interactions, Toasts & A11y Audit

### Changes to Be Made:
1. Add lightweight Toast notification container in `_Layout.cshtml` and JS helper in `wwwroot/js/site.js` (`window.showToast(message, type)`).
2. Replace all remaining browser `alert()` popups with toast notifications.
3. Add button click scale physics (`transform: scale(0.98)` on `:active`) and `:focus-visible` focus rings.
4. Conduct WCAG 2.2 AA audit across all views (contrast ratios, `aria-label`, keyboard tab index).
5. Clean up any leftover inline `<style>` blocks.

### Agent Prompt:
```markdown
Skills to apply: `wcag-audit-patterns`, `break-ui`, `animate`, `animation-performance`

TASK: Conduct the final micro-interaction, toast notification, and accessibility pass.
1. Create a dependency-free toast notification container in `_Layout.cshtml` with `window.showToast(msg, type)` in `wwwroot/js/site.js`.
2. Connect all AJAX calls (status updates, comments, claims) to trigger toast alerts instead of raw alerts.
3. Add subtle button press physics (`:active { transform: scale(0.98); }`) and `:focus-visible` outline rings.
4. Verify WCAG 2.2 AA compliance on all text and badge contrast ratios.
5. Stress-test with `break-ui` patterns (long strings, missing photos) and verify `dotnet build` has 0 errors.
```
