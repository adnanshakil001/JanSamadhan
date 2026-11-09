# Project: JanSamadhan — Civic Grievance & Municipal Resolution Platform
## Design System & Screen Generation Specification for Google Stitch

---

## 1. Product Overview & Vision
**JanSamadhan** is a modern civic governance platform designed to bridge citizens, city administrators, and field technicians through real-time geospatial reporting and transparent grievance resolution.

- **Vibe & Aesthetic**: High-trust public service tech, modern civic clarity, approachable, clean, accessible, and fast.
- **Visual Influences**: Linear-like data hierarchy, Apple-style subtle depth and tactile feedback, modern GovTech transparency (UK Gov Digital Service + Singapore GovTech).
- **Primary Users**:
  1. **Citizens**: Reporting issues (potholes, water leaks, broken streetlights) in under 60 seconds with photos and map pins.
  2. **Field Technicians**: On-the-ground mobile workers executing tasks, claiming jobs, and uploading before/after proof photos.
  3. **Municipal Admins**: Operations command center managing departments, workload distribution, GIS heat maps, and technician performance.

---

## 2. Design System Tokens & Foundations

### Typography
- **Headlines / Display**: `Plus Jakarta Sans`, Extra Bold (`800`) & Bold (`700`), tight tracking (`-0.03em`).
- **Body / Content**: `Inter`, Regular (`400`) & Medium (`500`), line height `1.5` to `1.6`.
- **Labels / Badges / Code**: `Inter`, SemiBold (`600`), uppercase tracking (`+0.05em`).

### Color Palette (WCAG 2.2 AA Compliant)
- **Primary Brand / Civic Blue**:
  - Main: `#2563eb` (Blue 600)
  - Hover / Pressed: `#1d4ed8` (Blue 700)
  - Soft Tint / Surface: `#eff6ff` (Blue 50)
- **Status Colors (Soft Background + High-Contrast Text)**:
  - **Pending / Urgent**: `#ef4444` (Crimson 500) | Badge: `#fef2f2` bg with `#991b1b` text.
  - **In Progress / Dispatched**: `#f59e0b` (Amber 500) | Badge: `#fffbeb` bg with `#92400e` text.
  - **Resolved / Fixed**: `#10b981` (Emerald 500) | Badge: `#ecfdf5` bg with `#065f46` text.
- **Neutrals & Surfaces**:
  - Page Canvas: `#f8fafc` (Slate 50)
  - Surface White: `#ffffff`
  - Subtle Borders: `#e2e8f0` (Slate 200)
  - Card Divider: `#f1f5f9` (Slate 100)
  - Text Primary: `#0f172a` (Slate 900)
  - Text Secondary: `#64748b` (Slate 500)

### Shape & Elevation
- **Corner Roundness**: `12px` (`ROUND_TWELVE`) for cards, dialogs, and inputs; `9999px` (`ROUND_FULL`) for badges and pills.
- **Shadows**:
  - Card Default: `0 1px 3px 0 rgba(0, 0, 0, 0.05), 0 1px 2px -1px rgba(0, 0, 0, 0.05)`
  - Card Hover: `0 10px 25px -5px rgba(0, 0, 0, 0.06), 0 8px 10px -6px rgba(0, 0, 0, 0.04)`
  - Floating Panels: `0 20px 40px -15px rgba(15, 23, 42, 0.12)`
- **Materials**: Frosted glass top navigation headers (`backdrop-filter: blur(12px); background: rgba(255, 255, 255, 0.85)`).

---

## 3. Key Screens to Generate

### Screen 1: Public Civic Landing & Hero Page (Desktop, 1440px)
- **Sticky Top Bar**:
  - Left: JanSamadhan logo with civic landmark icon (`#2563eb`).
  - Center: Nav links (`Features`, `How It Works`, `Stakeholders`, `Live City Map`).
  - Right: "Citizen Login" outline button, "Admin Portal" text link with shield icon, and "Report a Problem" primary rounded CTA button.
- **Hero Section**:
  - Pill badge: "Official Municipal Grievance Redressal • Open 24/7".
  - Headline: "Empowering Cities through Digital Clarity".
  - Subtitle: "Report infrastructure issues in 60 seconds. Track municipal resolution with real-time geospatial accountability."
  - Dual CTAs: Primary "Report a Problem" with arrow icon, Secondary "Explore City Map".
  - Live Civic Metric Counter row: 
    - `14,820+` Issues Resolved
    - `94.2%` Resolution Rate
    - `< 48 hrs` Average Response Time
- **3-Step Process Cards**:
  1. *Pin & Snap*: Pin problem on the map & take a photo.
  2. *Automated Routing*: System AI assigns to the exact municipal department.
  3. *Verified Resolution*: Technician fixes the issue and uploads photo proof.
- **Footer**: Clean, accessible civic links, emergency helpline numbers, and privacy policy.

---

### Screen 2: Citizen Issue Reporting Flow (Desktop / Tablet, 1200px)
- **Page Header**: Breadcrumbs (`Home / My Complaints / Report Issue`), Title "Report a Civic Grievance".
- **2-Column Grid Layout**:
  - **Left Column (Form Controls)**:
    - Problem Type Grid: 6 visual pill cards with icons (`Pothole`, `Streetlight`, `Water Leakage`, `Garbage Overflow`, `Sewage`, `Traffic Signal`).
    - Department Auto-Assignment Chip: Automatically highlights "Public Works Dept" or "Sanitation Dept" based on selected category.
    - Description Box: Clean text area with helper placeholder and character count.
    - Drag & Drop Photo Upload Zone: Dashed border with camera icon, "Click to browse or drop photos", showing instant thumbnail preview with remove button.
    - Submit Button: Full-width high-contrast blue button with lock icon ("Submit Grievance").
  - **Right Column (Interactive Geospatial Map)**:
    - Card container housing interactive map canvas.
    - Top floating search bar: "Search street, colony, or landmark...".
    - "Use My Current GPS Location" circular floating action button.
    - Custom map pin marker with drop pulse animation.
    - Bottom card overlay: Reverse-geocoded address display card ("Detected: M.G. Road, Near Civic Center, Zone 4").

---

### Screen 3: Citizen Dashboard & Complaint Tracker (Desktop, 1440px)
- **Left Sidebar**:
  - App brand logo, navigation links (`My Complaints`, `Report Issue`, `Nearby Map`, `Badges & Rewards`, `City Leaderboard`).
  - Bottom profile card with user avatar, name, and logout button.
- **Top Summary Metrics**:
  - 3 compact cards:
    - *Pending Review*: `2` (Red accent)
    - *Under Repair*: `1` (Amber accent)
    - *Resolved & Verified*: `8` (Green accent)
- **Complaint Detail View with Resolution Stepper**:
  - Complaint ID header: `#JS-8492 — Broken Streetlight on 5th Avenue`.
  - 4-Stage Horizontal Stepper:
    1. *Submitted* (Completed green checkmark)
    2. *Department Assigned* (Completed green checkmark)
    3. *Technician Dispatched* (Active pulsing amber ring)
    4. *Resolved & Closed* (Upcoming grey)
  - Citizen vs Official Photos Gallery: Side-by-side comparison cards.
  - Interactive Comment Timeline: Chat bubbles with role badges (`Admin Note`, `Technician Update`, `Citizen Comment`).
  - Star Rating Dialog: 5-star interactive rating component with review feedback box ("Rate the resolution quality").

---

### Screen 4: Municipal Admin Command Center (Desktop, 1440px)
- **High-Density Dashboard**:
  - Top Metric Bar: Total Complaints, Active Field Technicians, Open Bottlenecks, Avg Fix Time.
  - Embedded Live Density Map: Geospatial heatmap with color-coded cluster markers (Red, Amber, Green).
- **Master Grievance Table**:
  - Controls: Search input, Department filter dropdown, Status toggle pills, "Export CSV" button.
  - Columns: ID, Created Date, Citizen, Category, Location, Status Badge, Assigned Technician, Actions.
  - Quick Action Dropdown: Reassign Department, Dispatch Technician, Override Status.
- **Sidebar Panels**:
  - Department Workload Distribution (progress bars for Public Works, Sanitation, Electrical, Water Supply).
  - Top 3 Technician Leaderboard widget.

---

### Screen 5: Technician Field Operations App (Mobile Viewport, 390 x 844px)
- **Header**:
  - Technician avatar, Name, Department badge ("Electrical Division"), and Performance Score pill ("1,240 pts • Gold Tier").
- **Segmented Tab Switcher**:
  - Tabs: `Assigned Tasks (3)` | `Work Pool (12)`
- **Task Cards**:
  - High-priority complaint card:
    - Category badge: "Streetlight Malfunction"
    - Urgency tag: "Urgent • 4 hours remaining"
    - Address with distance: "Crossroad 7, Sector B (0.8 km away)"
    - 1-Tap Action: "Navigate in Google Maps" with directional compass icon.
- **Proof-of-Work Resolution Screen**:
  - "Take After Photo" large touch button triggering camera.
  - Before/After photo comparison preview.
  - Single-tap "Mark as Fixed & Submit Proof" primary button.

