---
name: Civic Clarity
colors:
  surface: '#f7f9fb'
  surface-dim: '#d8dadc'
  surface-bright: '#f7f9fb'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f2f4f6'
  surface-container: '#eceef0'
  surface-container-high: '#e6e8ea'
  surface-container-highest: '#e0e3e5'
  on-surface: '#191c1e'
  on-surface-variant: '#45464d'
  inverse-surface: '#2d3133'
  inverse-on-surface: '#eff1f3'
  outline: '#76777d'
  outline-variant: '#c6c6cd'
  surface-tint: '#565e74'
  primary: '#000000'
  on-primary: '#ffffff'
  primary-container: '#131b2e'
  on-primary-container: '#7c839b'
  inverse-primary: '#bec6e0'
  secondary: '#0051d5'
  on-secondary: '#ffffff'
  secondary-container: '#316bf3'
  on-secondary-container: '#fefcff'
  tertiary: '#000000'
  on-tertiary: '#ffffff'
  tertiary-container: '#0b1c30'
  on-tertiary-container: '#75859d'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#dae2fd'
  primary-fixed-dim: '#bec6e0'
  on-primary-fixed: '#131b2e'
  on-primary-fixed-variant: '#3f465c'
  secondary-fixed: '#dbe1ff'
  secondary-fixed-dim: '#b4c5ff'
  on-secondary-fixed: '#00174b'
  on-secondary-fixed-variant: '#003ea8'
  tertiary-fixed: '#d3e4fe'
  tertiary-fixed-dim: '#b7c8e1'
  on-tertiary-fixed: '#0b1c30'
  on-tertiary-fixed-variant: '#38485d'
  background: '#f7f9fb'
  on-background: '#191c1e'
  surface-variant: '#e0e3e5'
  status-fixed: '#10B981'
  status-in-progress: '#F59E0B'
  status-pending: '#EF4444'
  badge-bronze: '#CD7F32'
  badge-silver: '#C0C0C0'
  badge-gold: '#FFD700'
  badge-platinum: '#E5E4E2'
  badge-diamond: '#B9F2FF'
typography:
  display-lg:
    fontFamily: Inter
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 60px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  title-md:
    fontFamily: Inter
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-lg:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-sm:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  label-md:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
    letterSpacing: 0.05em
  data-mono:
    fontFamily: JetBrains Mono
    fontSize: 13px
    fontWeight: '400'
    lineHeight: 18px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 4px
  xs: 8px
  sm: 16px
  md: 24px
  lg: 32px
  xl: 48px
  gutter: 16px
  margin-mobile: 16px
  margin-desktop: 32px
---

## Brand & Style

The design system is anchored in **Corporate Modernism** with a focus on civic responsibility and institutional trust. It balances the authoritative nature of government services with the approachability of a modern consumer app. The aesthetic is "Enterprise-grade yet Citizen-centric," prioritizing clarity, reliability, and high-utility data visualization.

The visual narrative is driven by:
- **Professionalism:** Using a deep slate and navy foundation to establish credibility.
- **Efficiency:** A strict 8px spatial system to handle complex data-heavy dashboards without visual fatigue.
- **Responsiveness:** A mobile-first philosophy that ensures field technicians can operate with one hand while maintaining the density required for administrative oversight.

## Colors

The palette is built on a foundation of **Deep Slate (#0F172A)** and **Professional Blue (#2563EB)** to evoke trust and stability. 

- **Primary & Secondary:** Used for high-level branding, navigation, and primary actions.
- **Status Colors:** High-contrast emerald, amber, and coral are reserved strictly for ticket lifecycles to ensure immediate recognition on maps and lists.
- **Gamification Tiers:** A dedicated set of metallic tokens is used for the five-tier technician reward system, ensuring these "achievement" markers stand out from functional UI elements.
- **Neutral:** A cool-toned slate-gray scale provides a clean backdrop, reducing glare during field use.

## Typography

This design system utilizes **Inter** for all functional roles due to its exceptional legibility in data-dense environments. 

- **Headlines:** Use tighter letter-spacing and heavier weights to maintain authority.
- **Data Display:** For system logs and geospatial coordinates, a secondary monospace font (JetBrains Mono) is introduced to ensure character alignment and readability.
- **Mobile Optimization:** Headline scales are reduced for mobile to prevent awkward text wrapping in ticket descriptions.
- **Labels:** All labels use uppercase styling with increased letter-spacing to distinguish them from interactive body text.

## Layout & Spacing

The system follows a strict **8px grid** (with a 4px baseline for micro-adjustments) to align perfectly with Bootstrap 5 implementation.

- **Layout:** A 12-column fluid grid is used for the Admin Dashboard, while the Mobile interface uses a single-column stacked layout with 16px side margins.
- **Content Density:** In data-heavy views (like the Grievance List), use "Compact" spacing (8px between rows). In citizen-facing landing pages, use "Generous" spacing (32px+) to improve scanability.
- **Breakpoints:**
  - Mobile: < 768px (Single column, bottom-sheet menus).
  - Tablet: 768px - 1024px (2-column grid, collapsed sidebar).
  - Desktop: > 1024px (Fixed sidebar, fluid content area).

## Elevation & Depth

Hierarchy is established through **Tonal Layering** and soft ambient shadows. Since the platform is map-centric, elevation is used to pull interactive elements above the base map layer.

- **Level 0 (Base):** The Map (OpenStreetMap) or background canvas.
- **Level 1 (Surface):** Cards and content containers. Use a subtle 1px border (#E2E8F0) and no shadow.
- **Level 2 (Interactive):** Hover states and active ticket cards. Use an extra-diffused shadow (Y: 4px, Blur: 12px, Opacity: 0.05) to suggest lift.
- **Level 3 (Overlay):** Modals, Popups, and Pin Callouts. Use a high-contrast shadow (Y: 10px, Blur: 25px, Opacity: 0.1) and a slight backdrop blur (4px) to focus citizen attention on the task.

## Shapes

The shape language is **Rounded (0.5rem / 8px)** to soften the institutional feel and make the app more accessible.

- **Standard Elements:** Buttons, Input fields, and small cards use the base 8px radius.
- **Large Components:** Main content containers and "Dual-Photo Proof" cards use `rounded-xl` (1.5rem / 24px) to create a distinct, modern "app-like" container feel.
- **Status Indicators:** Map pins and status badges use pill-shapes (fully rounded) to differentiate them from functional UI blocks.

## Components

- **Buttons:** Primary buttons use the Deep Slate (#0F172A) with white text. "Action" buttons for technicians (e.g., "Submit Proof") use Professional Blue (#2563EB).
- **Cards:** Modern cards must include a "Header-Body-Footer" structure. The footer is reserved for status badges and timestamp metadata.
- **Grievance Pins:** Use a drop-shaped marker with a white inner circle. The marker color must strictly match the status tokens (Emerald, Amber, Coral).
- **Input Fields:** Use a 1px slate-200 border that thickens to 2px Professional Blue on focus. Labels should always be persistent (not floating) for accessibility.
- **Dual-Photo Proof:** A specialized component showing "Before" and "After" photos side-by-side with a central "Verified" badge overlay when the status is 'Fixed'.
- **Gamification Badges:** Circular icons with metallic gradients. These should always be accompanied by a tooltip explaining the point requirement (e.g., "Level 3: 60 pts").